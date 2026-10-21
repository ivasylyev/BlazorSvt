using BlazorSvt.Modules.SmartRateLookup.Contracts;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.SmartRateLookup.Domain;
using BlazorSvt.Modules.SmartRateLookup.Resources;
using BlazorSvt.Modules.SmartRateLookup.Services;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api;
using FluentAssertions;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;

namespace BlazorSvt.UnitTests.Modules.SmartRateLookup;

[Trait("Category", "Unit")]
public class SmartRateLookupServiceTests
{
    private const long Rub = 32694;
    private const long Usd = 32623;

    private readonly ISmartRateLookupRepository repository = Substitute.For<ISmartRateLookupRepository>();
    private readonly IAccessGuard accessGuard = Substitute.For<IAccessGuard>();

    [Fact]
    public async Task Lookup_WhenRequiredFieldsMissing_Returns400PerField()
    {
        var act = () => Create().LookupAsync(new SmartRateLookupRequest { NodeFromCode = "A1" }, default);

        var exception = (await act.Should().ThrowAsync<ApiException>()).Which;
        exception.StatusCode.Should().Be(400);
        exception.Errors.Should().ContainKeys("nodeToCode", "productCode", "transportTypeCode", "currencyCode")
            .And.NotContainKey("nodeFromCode");
    }

    [Fact]
    public async Task Lookup_WhenCodesOrProfileUnknown_Returns400PerField()
    {
        repository.ResolveAsync(default!, default!, default!, default!, default!, default!, default)
            .ReturnsForAnyArgs(new LookupReferences { NodeFromId = 1, CurrencyId = Rub, ProfileExists = false });

        var act = () => Create().LookupAsync(Request(profile: "Нет такого"), default);

        var exception = (await act.Should().ThrowAsync<ApiException>()).Which;
        exception.Errors.Should().ContainKeys("nodeToCode", "productCode", "transportTypeCode", "profile")
            .And.NotContainKeys("nodeFromCode", "currencyCode");
        exception.Errors!["profile"].Should().Equal(SmartRateLookupService.NotFoundCode);
    }

    [Fact]
    public async Task Lookup_DefaultsDateToServerTodayAndProfileToCrm()
    {
        SetupReferences();
        var time = Substitute.For<TimeProvider>();
        time.LocalTimeZone.Returns(TimeZoneInfo.Utc);
        time.GetUtcNow().Returns(new DateTimeOffset(2026, 10, 20, 12, 0, 0, TimeSpan.Zero));

        var response = await Create(time).LookupAsync(Request(), default);

        response.Found.Should().BeFalse();
        response.Request.Date.Should().Be(new DateOnly(2026, 10, 20));
        response.Request.Profile.Should().Be(SmartRateLookupService.DefaultProfile);
        await repository.Received(1).GetCandidatesAsync(
            Arg.Any<IReadOnlyList<LookupStep>>(), new DateOnly(2026, 10, 20), "Импорт в CRM", Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Lookup_WhenSameCurrency_ReturnsRateWithoutConversion()
    {
        SetupReferences();
        SetupCandidates(Candidate(currencyId: Rub, value: 1000m));

        var response = await Create().LookupAsync(Request(), default);

        response.Found.Should().BeTrue();
        response.Rate!.Value.Should().Be(1000m);
        response.Rate.ConversionRate.Should().Be(1m);
        response.Rate.ConversionDate.Should().BeNull();
        response.Match!.Level.Should().Be(1);
        response.Match.DepartureNodeReplaced.Should().BeFalse();
        await repository.DidNotReceiveWithAnyArgs().ConvertCurrencyAsync(default, default, default, default, default);
    }

    [Fact]
    public async Task Lookup_WhenOtherCurrency_ConvertsAtRequestDate()
    {
        SetupReferences();
        SetupCandidates(Candidate(currencyId: Usd, value: 100m));
        repository.ConvertCurrencyAsync(100m, Usd, Rub, new DateOnly(2026, 9, 25), Arg.Any<CancellationToken>())
            .Returns(8490.57m);

        var response = await Create().LookupAsync(Request(date: new DateOnly(2026, 9, 25)), default);

        response.Rate!.Value.Should().Be(8490.57m);
        response.Rate.OriginalValue.Should().Be(100m);
        response.Rate.OriginalCurrencyCode.Should().Be("USD");
        response.Rate.CurrencyCode.Should().Be("RUB");
        response.Rate.ConversionRate.Should().Be(84.9057m);
        response.Rate.ConversionDate.Should().Be(new DateOnly(2026, 9, 25));
    }

    [Fact]
    public async Task Lookup_WhenNoExchangeRate_Returns422()
    {
        SetupReferences();
        SetupCandidates(Candidate(currencyId: Usd, value: 100m));
        repository.ConvertCurrencyAsync(default, default, default, default, default).ReturnsForAnyArgs((decimal?)null);

        var act = () => Create().LookupAsync(Request(), default);

        var exception = (await act.Should().ThrowAsync<ApiException>()).Which;
        exception.StatusCode.Should().Be(422);
        exception.Code.Should().Be(ApiErrorCodes.CurrencyRateMissing);
    }

    [Fact]
    public async Task Lookup_WhenFoundByDpoNode_ReportsReplacement()
    {
        SetupReferences(new DpoNode(901, "D901"));
        SetupCandidates(Candidate(currencyId: Rub, value: 1m, nodeFrom: 901));

        var response = await Create().LookupAsync(Request(), default);

        response.Match!.DepartureNodeReplaced.Should().BeTrue();
        response.Match.DepartureNodeCode.Should().Be("D901");
        response.Match.Description.Should().Contain("D901");
    }

    [Fact]
    public async Task Lookup_ChecksReadAccess()
    {
        accessGuard.When(g => g.Ensure(AccessAction.Read)).Do(_ => throw new AccessDeniedException());

        var act = () => Create().LookupAsync(Request(), default);

        await act.Should().ThrowAsync<AccessDeniedException>();
        await repository.DidNotReceiveWithAnyArgs().ResolveAsync(default!, default!, default!, default!, default!, default!, default);
    }

    private SmartRateLookupService Create(TimeProvider? time = null)
    {
        var factory = new ResourceManagerStringLocalizerFactory(
            Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
        return new SmartRateLookupService(
            repository,
            accessGuard,
            new StringLocalizer<SmartRateLookupText>(factory),
            time ?? TimeProvider.System);
    }

    private static SmartRateLookupRequest Request(DateOnly? date = null, string? profile = null) =>
        new()
        {
            NodeFromCode = "A1",
            NodeToCode = "B1",
            ProductCode = "P1",
            TransportTypeCode = "Auto_20",
            CurrencyCode = "RUB",
            Date = date,
            Profile = profile
        };

    private void SetupReferences(params DpoNode[] dpo) =>
        repository.ResolveAsync(default!, default!, default!, default!, default!, default!, default)
            .ReturnsForAnyArgs(new LookupReferences
            {
                NodeFromId = 100,
                NodeToId = 200,
                RegionNodeToId = 300,
                ProductId = 10,
                ProductGroupId = 50,
                TransportTypeId = 7,
                TransportKindId = 70,
                CurrencyId = Rub,
                ProfileExists = true,
                DpoNodes = dpo
            });

    private void SetupCandidates(params CandidateRow[] rows) =>
        repository.GetCandidatesAsync(default!, default, default!, default).ReturnsForAnyArgs(rows);

    private static CandidateRow Candidate(long currencyId, decimal value, long nodeFrom = 100) =>
        new()
        {
            AverageRateLevel3Id = 555,
            NodeFromId = nodeFrom,
            NodeToId = 200,
            ProductId = 10,
            ProductGroupId = 50,
            TransportTypeId = 7,
            TransportKindId = 70,
            RateTypeId = 1,
            RateTypePriority = 1,
            StartDate = new DateTime(2026, 9, 1),
            EndDate = new DateTime(2026, 12, 31),
            RateLevel3 = value,
            CurrencyId = currencyId,
            CurrencyCode = currencyId == Usd ? "USD" : "RUB",
            RateTypeCode = "6",
            RateTypeName = "Фактическая",
            TransportTypeCode = "Auto_20",
            TransportTypeName = "Авто",
            NodeFromCode = "A1",
            NodeToCode = "B1"
        };
}
