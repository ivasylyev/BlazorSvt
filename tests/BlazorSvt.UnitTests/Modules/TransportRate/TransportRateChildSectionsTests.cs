using System.Globalization;
using BlazorSvt.Modules.TransportRate;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;
using TransportRateResources = BlazorSvt.Modules.TransportRate.Resources.TransportRate;

namespace BlazorSvt.UnitTests.Modules.TransportRate;

[Trait("Category", "Unit")]
public class TransportRateChildSectionsTests
{
    private const long TransportRateId = 39010792;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<TransportRateResources> localizer = KeyLocalizer<TransportRateResources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Fact]
    public async Task TenderSection_ActiveParent_ColumnsInOrder()
    {
        GivenTenderRows(new TransportRateTenderServicePackDto
        {
            TransportRateId = TransportRateId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "P-1",
            TenderServicePack = "Pack",
            LotRate = 12.50m,
            LotCurrencyId = CurrencyRuEn.RUB
        });
        var section = new TransportRateTenderServicePackSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("TransportRateTenderServicePackDto.Title");
        table.Headers().Should().Equal(
            "TransportRateTenderServicePackDto.Code",
            "TransportRateTenderServicePackDto.TenderServicePack",
            "TransportRateTenderServicePackDto.LotRate",
            "TransportRateTenderServicePackDto.LotCurrency");
        table.Cells().Single().Should().Equal(
            "P-1",
            "Pack",
            Convert.ToString(12.50m, CultureInfo.CurrentCulture),
            "RUB");
    }

    [Fact]
    public async Task TenderSection_EmptyReferences_RenderEmptyCells()
    {
        GivenTenderRows(new TransportRateTenderServicePackDto
        {
            TransportRateId = TransportRateId,
            Id = 1,
            PrimitiveEntityDataStateId = 1
        });
        var section = new TransportRateTenderServicePackSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "ru", CancellationToken.None);

        table.Cells().Single().Should().AllBe(string.Empty);
    }

    [Fact]
    public async Task TenderSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenTenderRows(
            new TransportRateTenderServicePackDto { TransportRateId = TransportRateId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new TransportRateTenderServicePackDto { TransportRateId = TransportRateId, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new TransportRateTenderServicePackSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "en", CancellationToken.None);

        table.Headers().Should().HaveCount(5).And.EndWith("TransportRateTenderServicePackDto.IsArchive");
        table.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        table.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        table.Columns[^1].Name.Should().Be("IsArchive");
        table.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
    }

    [Fact]
    public async Task CoefficientSection_ActiveParent_ColumnsInOrder()
    {
        var start = new DateOnly(2024, 1, 15);
        var end = new DateOnly(2024, 12, 31);
        GivenCoefficientRows(new TransportRateCoefficientDto
        {
            TransportRateId = TransportRateId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "C-1",
            CoefficientRateCode = "R-1",
            StartDate = start,
            EndDate = end,
            CoefficientName = "Deflator",
            SourceRateCode = "S-1",
            SourceRateKindName = "Обычная"
        });
        var section = new TransportRateCoefficientSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("TransportRateCoefficientDto.Title");
        table.Headers().Should().Equal(
            "TransportRateCoefficientDto.Code",
            "TransportRateCoefficientDto.CoefficientRateCode",
            "TransportRateCoefficientDto.StartDate",
            "TransportRateCoefficientDto.EndDate",
            "TransportRateCoefficientDto.CoefficientName",
            "TransportRateCoefficientDto.SourceRateCode",
            "TransportRateCoefficientDto.SourceRateKindName");
        table.Cells().Single().Should().Equal(
            "C-1",
            "R-1",
            Convert.ToString(start, CultureInfo.CurrentCulture),
            Convert.ToString(end, CultureInfo.CurrentCulture),
            "Deflator",
            "S-1",
            "Обычная");
    }

    [Fact]
    public async Task CoefficientSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenCoefficientRows(
            new TransportRateCoefficientDto { TransportRateId = TransportRateId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new TransportRateCoefficientDto { TransportRateId = TransportRateId, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new TransportRateCoefficientSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);

        table.Headers().Should().HaveCount(8).And.EndWith("TransportRateCoefficientDto.IsArchive");
        table.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        table.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        table.Columns[^1].Name.Should().Be("IsArchive");
        table.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddTransportRateModule();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<TransportRateDetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(
                typeof(TransportRateTenderServicePackSection),
                typeof(TransportRateCoefficientSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Тендерный пакет услуг",
        new[] { "Код", "Тендерный пакет услуг", "Ставка лота", "Валюта лота", "Архив" },
        "Применённые коэффициенты",
        new[] { "Код", "Код ставки с коэфициентом", "Начало периода", "Окончание периода", "Коэффициент", "Исходная ставка", "Вид исходной ставки", "Архив" })]
    [InlineData("en", "en-US",
        "Tender service pack",
        new[] { "Code", "Tender service pack", "Lot rate", "Lot currency", "Archive" },
        "Applied coefficients",
        new[] { "Code", "Rate code with coefficient", "Start of period", "End of period", "Coefficient", "Source rate", "Source rate kind", "Archive" })]
    public async Task Sections_UseResxTitlesAndHeaders(
        string lang,
        string culture,
        string tenderTitle,
        string[] tenderHeaders,
        string coefficientTitle,
        string[] coefficientHeaders)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<TransportRateResources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenTenderRows(new TransportRateTenderServicePackDto { TransportRateId = TransportRateId, Id = 1, PrimitiveEntityDataStateId = 1 });
            GivenCoefficientRows(new TransportRateCoefficientDto { TransportRateId = TransportRateId, Id = 1, PrimitiveEntityDataStateId = 1 });
            var tender = new TransportRateTenderServicePackSection(dataService, resxLocalizer, resxPlatform);
            var coefficient = new TransportRateCoefficientSection(dataService, resxLocalizer, resxPlatform);

            var tenderTable = await tender.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);
            var coefficientTable = await coefficient.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            tender.Title.Should().Be(tenderTitle);
            tenderTable.Headers().Should().Equal(tenderHeaders);
            coefficient.Title.Should().Be(coefficientTitle);
            coefficientTable.Headers().Should().Equal(coefficientHeaders);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenTenderRows(params TransportRateTenderServicePackDto[] rows) =>
        dataService
            .GetRowsAsync<TransportRateTenderServicePackDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<TransportRateTenderServicePackDto>(rows, rows.Length));

    private void GivenCoefficientRows(params TransportRateCoefficientDto[] rows) =>
        dataService
            .GetRowsAsync<TransportRateCoefficientDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<TransportRateCoefficientDto>(rows, rows.Length));

    private static TransportRateDetailDto Parent(bool isArchive) => new()
    {
        TransportRateId = TransportRateId,
        IsArchive = isArchive,
        Code = 1,
        AverageRateCode = "",
        CalcType = "",
        CurrencyStandard = "",
        TypeCode = "",
        TypeName = "",
        NodeFromCode = "",
        NodeFromNameRu = "",
        NodeFromNameEn = "",
        NodeToCode = "",
        NodeToNameRu = "",
        NodeToNameEn = "",
        TransportKindCode = "",
        TransportKindNameRu = "",
        TransportKindNameRuEn = "",
        TransportTypeCode = "",
        TransportTypeNameRu = "",
        TransportTypeNameRuEn = "",
        LegCode = ""
    };

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
