using BlazorSvt.Modules.SmartRateLookup.Contracts;
using BlazorSvt.Modules.SmartRateLookup.Services;
using BlazorSvt.Platform.Api;
using Microsoft.AspNetCore.Mvc;

namespace BlazorSvt.Modules.SmartRateLookup.Api;

/// <summary><c>POST /api/v1/rates/smart-lookup</c>.</summary>
public sealed class SmartRateLookupEndpoint : IApiEndpointContribution
{
    public void Map(RouteGroupBuilder api)
    {
        api.MapPost("/rates/smart-lookup", LookupAsync)
            .WithTags("rates")
            .WithName("smartRateLookup")
            .WithSummary("Умный подбор ставки")
            .WithDescription(
                "Одна наиболее подходящая средневзвешенная ставка 3-го уровня. Если точного совпадения нет — " +
                "подъём продукта до группы, узла назначения до региона, типа транспорта до вида (уровни 1–8), " +
                "затем те же уровни для дополнительных узлов отправления организации DPO. " +
                "Порядок: приоритет типа ставки по профилю → уровень → близость к средней → бо́льшая ставка. " +
                "Ставка не найдена — 200 с found=false. Нет курса для пересчёта — 422 currency_rate_missing.")
            .Produces<SmartRateLookupResponse>()
            .ProducesValidationProblem()
            .ProducesProblem(StatusCodes.Status422UnprocessableEntity);
    }

    private static async Task<IResult> LookupAsync(
        HttpContext context,
        [FromBody] SmartRateLookupRequest request,
        SmartRateLookupService service,
        CancellationToken cancellationToken)
    {
        ApiCallLog.SetOperation(context, "smart_lookup");
        var response = await service.LookupAsync(request, cancellationToken);
        ApiCallLog.SetRowCount(context, response.Found ? 1 : 0);
        return Results.Ok(response);
    }
}
