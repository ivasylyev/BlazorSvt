using BlazorSvt.Modules.SmartRateLookup.Contracts;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.SmartRateLookup.Domain;
using BlazorSvt.Modules.SmartRateLookup.Resources;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Catalogs;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.SmartRateLookup.Services;

/// <summary>
/// Умный подбор одной ставки: если точного совпадения нет — поиск с подъёмом продукта до группы,
/// узла назначения до региона, типа транспорта до вида и заменой узла отправления на узлы DPO.
/// Эталон — <c>CalcTransportRateCRM</c> (поведение на проде).
/// </summary>
public sealed class SmartRateLookupService(
    ISmartRateLookupRepository repository,
    IAccessGuard accessGuard,
    IStringLocalizer<SmartRateLookupText> text,
    TimeProvider timeProvider)
{
    public const string DefaultProfile = "Импорт в CRM";
    public const string NotFoundCode = "not_found";

    public async Task<SmartRateLookupResponse> LookupAsync(SmartRateLookupRequest request, CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

        var input = Normalize(request);
        var refs = await repository.ResolveAsync(
            input.NodeFromCode,
            input.NodeToCode,
            input.ProductCode,
            input.TransportTypeCode,
            input.CurrencyCode,
            input.Profile,
            cancellationToken);

        var resolved = Resolve(refs, input);

        var steps = LookupStepBuilder.Build(resolved);
        var rows = await repository.GetCandidatesAsync(steps, input.Date, input.Profile, cancellationToken);
        var rowsById = rows.GroupBy(r => r.AverageRateLevel3Id).ToDictionary(g => g.Key, g => g.First());

        var echo = new SmartRateLookupResolvedRequest(
            input.NodeFromCode,
            refs.NodeFromName,
            input.NodeToCode,
            refs.NodeToName,
            input.ProductCode,
            refs.ProductName,
            refs.ProductGroupCode,
            refs.ProductGroupName,
            input.TransportTypeCode,
            refs.TransportTypeName,
            refs.TransportKindCode,
            input.CurrencyCode,
            input.Date,
            input.Profile);

        var best = SmartRateRanker.Pick(steps, rowsById.Values.Select(r => r.ToCandidate()).ToList());
        if (best is null)
        {
            return new SmartRateLookupResponse(false, null, null, echo);
        }

        var row = rowsById[best.Candidate.AverageRateLevel3Id];
        var rate = await BuildRateAsync(row, refs.CurrencyId!.Value, input, cancellationToken);
        return new SmartRateLookupResponse(true, rate, BuildMatch(best.Step), echo);
    }

    private async Task<SmartRateLookupRate> BuildRateAsync(
        CandidateRow row,
        long currencyId,
        NormalizedRequest input,
        CancellationToken cancellationToken)
    {
        var value = row.RateLevel3;
        var conversionRate = 1m;
        DateOnly? conversionDate = null;

        if (row.CurrencyId != currencyId && row.RateLevel3 != 0)
        {
            var converted = await repository.ConvertCurrencyAsync(row.RateLevel3, row.CurrencyId, currencyId, input.Date, cancellationToken)
                ?? throw new ApiException(
                    StatusCodes.Status422UnprocessableEntity,
                    ApiErrorCodes.CurrencyRateMissing);

            value = converted;
            conversionRate = Math.Round(converted / row.RateLevel3, 6);
            conversionDate = input.Date;
        }

        return new SmartRateLookupRate(
            row.AverageRateLevel3Id,
            value,
            input.CurrencyCode,
            row.RateLevel3,
            row.CurrencyCode,
            conversionRate,
            conversionDate,
            row.EffectiveLoad,
            DateOnly.FromDateTime(row.StartDate),
            DateOnly.FromDateTime(row.EndDate),
            row.RateTypeCode,
            row.RateTypeName,
            row.RateTypePriority,
            row.TransportTypeCode,
            row.TransportTypeName,
            row.NodeFromCode,
            row.NodeToCode,
            row.ProductCode,
            row.ProductGroupCode);
    }

    private SmartRateLookupMatch BuildMatch(LookupStep step)
    {
        var description = text[$"Match.Level{step.Level}"].Value;
        if (step.ReplacedDepartureNode is { } dpo)
        {
            description = text["Match.DepartureReplaced", dpo.NodeCode ?? dpo.NodeId.ToString()].Value + " " + description;
        }

        return new SmartRateLookupMatch(
            step.Level,
            step.Lifts.HasFlag(LookupLifts.ProductToGroup),
            step.Lifts.HasFlag(LookupLifts.DestinationToRegion),
            step.Lifts.HasFlag(LookupLifts.TransportTypeToKind),
            step.ReplacedDepartureNode is not null,
            step.ReplacedDepartureNode?.NodeCode,
            description);
    }

    private NormalizedRequest Normalize(SmartRateLookupRequest request)
    {
        var errors = new Dictionary<string, string[]>();

        string Required(string? value, string field)
        {
            var trimmed = value?.Trim();
            if (string.IsNullOrEmpty(trimmed))
            {
                errors[field] = [CatalogValidationCodes.Required];
                return string.Empty;
            }

            return trimmed;
        }

        var normalized = new NormalizedRequest(
            Required(request.NodeFromCode, "nodeFromCode"),
            Required(request.NodeToCode, "nodeToCode"),
            Required(request.ProductCode, "productCode"),
            Required(request.TransportTypeCode, "transportTypeCode"),
            Required(request.CurrencyCode, "currencyCode"),
            request.Date ?? DateOnly.FromDateTime(timeProvider.GetLocalNow().DateTime),
            string.IsNullOrWhiteSpace(request.Profile) ? DefaultProfile : request.Profile.Trim());

        if (errors.Count > 0)
        {
            throw ApiException.Validation(errors);
        }

        return normalized;
    }

    private static ResolvedLookupInput Resolve(LookupReferences refs, NormalizedRequest input)
    {
        var errors = new Dictionary<string, string[]>();
        if (refs.NodeFromId is null) errors["nodeFromCode"] = [NotFoundCode];
        if (refs.NodeToId is null) errors["nodeToCode"] = [NotFoundCode];
        if (refs.ProductId is null) errors["productCode"] = [NotFoundCode];
        if (refs.TransportTypeId is null || refs.TransportKindId is null) errors["transportTypeCode"] = [NotFoundCode];
        if (refs.CurrencyId is null) errors["currencyCode"] = [NotFoundCode];
        if (!refs.ProfileExists) errors["profile"] = [NotFoundCode];

        if (errors.Count > 0)
        {
            throw ApiException.Validation(errors);
        }

        return new ResolvedLookupInput(
            refs.NodeFromId!.Value,
            refs.NodeToId!.Value,
            refs.RegionNodeToId,
            refs.ProductId!.Value,
            refs.ProductGroupId,
            refs.TransportTypeId!.Value,
            refs.TransportKindId!.Value,
            refs.DpoNodes);
    }

    private sealed record NormalizedRequest(
        string NodeFromCode,
        string NodeToCode,
        string ProductCode,
        string TransportTypeCode,
        string CurrencyCode,
        DateOnly Date,
        string Profile);
}
