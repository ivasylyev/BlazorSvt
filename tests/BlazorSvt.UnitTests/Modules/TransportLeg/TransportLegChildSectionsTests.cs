using System.Globalization;
using BlazorSvt.Modules.TransportLeg;
using BlazorSvt.Modules.TransportLeg.Detail;
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
using TransportLegResources = BlazorSvt.Modules.TransportLeg.Resources.TransportLeg;

namespace BlazorSvt.UnitTests.Modules.TransportLeg;

[Trait("Category", "Unit")]
public class TransportLegChildSectionsTests
{
    private const long TransportLegId = 19297637;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<TransportLegResources> localizer = KeyLocalizer<TransportLegResources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Fact]
    public async Task LegRateSection_ActiveParent_ColumnsInOrder()
    {
        var start = new DateOnly(2024, 3, 1);
        var end = new DateOnly(2024, 12, 31);
        GivenLegRates(new LegRateDto
        {
            TransportLegId = TransportLegId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "LR-1",
            RateCode = "R-1",
            IsDefRate = true,
            RateTypeIdRu = RateTypeRu.Tender,
            RateTypeIdEn = RateTypeEn.Tender,
            TransportTypeIdRu = TransportTypeLevel3Ru.Auto_cont_40,
            TransportTypeIdEn = TransportTypeLevel3En.Auto_cont_40,
            ProductGroupNameRu = "Группа",
            ProductGroupNameEn = "Group",
            ProductNameRu = "Продукт",
            ProductNameEn = "Product",
            StartDate = start,
            EndDate = end,
            TotalCostTon = 10.50m,
            TotalCostTransport = 20.25m,
            CurrencyId = CurrencyRuEn.USD
        });
        var section = new LegRateSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("LegRateDto.Title");
        table.Headers().Should().Equal(
            "LegRateDto.RateCode",
            "LegRateDto.IsDefRate",
            "LegRateDto.RateTypeName",
            "LegRateDto.TransportTypeName",
            "LegRateDto.ProductGroupName",
            "LegRateDto.ProductName",
            "LegRateDto.StartDate",
            "LegRateDto.EndDate",
            "LegRateDto.TotalCostTon",
            "LegRateDto.TotalCostTransport",
            "LegRateDto.Currency");
        table.Cells().Single().Should().Equal(
            "R-1",
            "Common.Yes",
            "Tender",
            "Auto container 40ft",
            "Group",
            "Product",
            Text(start),
            Text(end),
            Text(10.50m),
            Text(20.25m),
            "USD");
    }

    [Fact]
    public async Task LegRateSection_EmptyReferences_RenderEmptyCells()
    {
        GivenLegRates(new LegRateDto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 });
        var section = new LegRateSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "ru", CancellationToken.None);

        table.Cells().Single().Should().Equal(
            "", "Common.No", "", "", "", "", "", "", "", "", "");
    }

    [Fact]
    public async Task LegAverageRateLevel3Section_ActiveParent_ColumnsInOrder()
    {
        GivenAverageRates(new LegAverageRateLevel3Dto
        {
            TransportLegId = TransportLegId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "AR-1",
            AverageRateCode = "A-1",
            IsDefRate = false,
            RateTypeIdEn = RateTypeEn.Indicative,
            TransportTypeIdEn = TransportTypeLevel3En.Auto_C,
            ProductGroupNameEn = "Group",
            ProductNameEn = "Product",
            RateLevel3 = 15.20m,
            EffectiveLoadOfTransportType = 1.500m,
            CurrencyId = CurrencyRuEn.EUR
        });
        var section = new LegAverageRateLevel3Section(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("LegAverageRateLevel3Dto.Title");
        table.Headers().Should().Equal(
            "LegAverageRateLevel3Dto.AverageRateCode",
            "LegAverageRateLevel3Dto.IsDefRate",
            "LegAverageRateLevel3Dto.RateTypeName",
            "LegAverageRateLevel3Dto.TransportTypeName",
            "LegAverageRateLevel3Dto.ProductGroupName",
            "LegAverageRateLevel3Dto.ProductName",
            "LegAverageRateLevel3Dto.StartDate",
            "LegAverageRateLevel3Dto.EndDate",
            "LegAverageRateLevel3Dto.RateLevel3",
            "LegAverageRateLevel3Dto.EffectiveLoadOfTransportType",
            "LegAverageRateLevel3Dto.Currency");
        table.Cells().Single().Should().Equal(
            "A-1",
            "Common.No",
            "Indicative",
            "Tank truck",
            "Group",
            "Product",
            "",
            "",
            Text(15.20m),
            Text(1.500m),
            "EUR");
    }

    [Fact]
    public async Task LegAverageRateLevel3Section_NullDeflator_RendersEmpty()
    {
        GivenAverageRates(new LegAverageRateLevel3Dto
        {
            TransportLegId = TransportLegId,
            Id = 1,
            PrimitiveEntityDataStateId = 1
        });
        var section = new LegAverageRateLevel3Section(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "ru", CancellationToken.None);

        table.Cells().Single().Should().AllBe(string.Empty);
    }

    [Fact]
    public async Task LeadtimeSection_ActiveParent_ColumnsInOrder()
    {
        var start = new DateOnly(2024, 1, 1);
        GivenLeadtimes(new LeadtimeDto
        {
            TransportLegId = TransportLegId,
            Id = 1,
            PrimitiveEntityDataStateId = 1,
            Code = "LT-1",
            StartDate = start,
            ShipmentTypeNameEn = "Truck",
            LeadTimeIsActive = true,
            CommercialLT = 3m,
            SearchTime = 1m,
            LoadTime = 2m,
            TravelTime = 4m,
            DaysWaiting = 5m,
            UnLoadTime = 6m,
            TransportationTime = 7m,
            Distance = 100m,
            Leg1_SearchTime = 1m,
            Leg1_LoadTime = 2m,
            Leg1_TravelTime = 3m,
            Leg1_DaysWaiting = 4m,
            Leg1_TransportationTime = 5m,
            Leg1_Distance = 60m,
            Leg2_TravelTime = 1m,
            Leg2_DaysWaiting = 2m,
            Leg2_UploadTime = 3m,
            Leg2_TransportationTime = 4m,
            Leg2_Distance = 40m
        });
        var section = new LeadtimeSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("LeadtimeDto.Title");
        table.Headers().Should().Equal(
            "LeadtimeDto.Code",
            "LeadtimeDto.StartDate",
            "LeadtimeDto.EndDate",
            "LeadtimeDto.ShipmentTypeName",
            "LeadtimeDto.LeadTimeIsActive",
            "LeadtimeDto.CommercialLT",
            "LeadtimeDto.SearchTime",
            "LeadtimeDto.LoadTime",
            "LeadtimeDto.TravelTime",
            "LeadtimeDto.DaysWaiting",
            "LeadtimeDto.UnLoadTime",
            "LeadtimeDto.TransportationTime",
            "LeadtimeDto.Distance",
            "LeadtimeDto.Leg1_SearchTime",
            "LeadtimeDto.Leg1_LoadTime",
            "LeadtimeDto.Leg1_TravelTime",
            "LeadtimeDto.Leg1_DaysWaiting",
            "LeadtimeDto.Leg1_TransportationTime",
            "LeadtimeDto.Leg1_Distance",
            "LeadtimeDto.Leg2_TravelTime",
            "LeadtimeDto.Leg2_DaysWaiting",
            "LeadtimeDto.Leg2_UploadTime",
            "LeadtimeDto.Leg2_TransportationTime",
            "LeadtimeDto.Leg2_Distance");
        table.Cells().Single().Should().Equal(
            "LT-1",
            Text(start),
            "",
            "Truck",
            "Common.Yes",
            Text(3m),
            Text(1m),
            Text(2m),
            Text(4m),
            Text(5m),
            Text(6m),
            Text(7m),
            Text(100m),
            Text(1m),
            Text(2m),
            Text(3m),
            Text(4m),
            Text(5m),
            Text(60m),
            Text(1m),
            Text(2m),
            Text(3m),
            Text(4m),
            Text(40m));
    }

    [Fact]
    public async Task ArchivedParent_AddsArchiveColumnLast()
    {
        GivenLegRates(
            new LegRateDto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new LegRateDto { TransportLegId = TransportLegId, Id = 2, PrimitiveEntityDataStateId = 2 });
        GivenAverageRates(new LegAverageRateLevel3Dto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 2 });
        GivenLeadtimes(new LeadtimeDto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 });
        var legRate = new LegRateSection(dataService, localizer, platform);
        var average = new LegAverageRateLevel3Section(dataService, localizer, platform);
        var leadtime = new LeadtimeSection(dataService, localizer, platform);

        var legRateTable = await legRate.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);
        var averageTable = await average.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);
        var leadtimeTable = await leadtime.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);

        legRateTable.Headers().Should().HaveCount(12).And.EndWith("LegRateDto.IsArchive");
        legRateTable.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        legRateTable.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        legRateTable.Columns[^1].Name.Should().Be("IsArchive");
        legRateTable.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
        averageTable.Headers().Should().HaveCount(12).And.EndWith("LegAverageRateLevel3Dto.IsArchive");
        averageTable.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        averageTable.Cells().Single()[^1].Should().Be("Common.Archive");
        leadtimeTable.Headers().Should().HaveCount(25).And.EndWith("LeadtimeDto.IsArchive");
        leadtimeTable.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        leadtimeTable.Cells().Single()[^1].Should().Be("Common.Active");
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddTransportLegModule();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<TransportLegDetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(
                typeof(LegRateSection),
                typeof(LegAverageRateLevel3Section),
                typeof(LeadtimeSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Ставки плеча",
        new[]
        {
            "Код ставки", "Дефлятор", "Тип ставки", "Тип транспорта", "Группа продуктов", "Продукт",
            "Период действия с", "Период действия по", "Общая стоимость за тонну",
            "Общая стоимость за транспортное средство", "Валюта-эталон", "Архив"
        },
        "Средневзвешенные ставки 3 уровня",
        new[]
        {
            "Код ставки", "Дефлятор", "Тип ставки", "Тип транспорта", "Группа продуктов", "Продукт",
            "Период действия с", "Период действия по", "Ставка ср.взвеш. (т) в валюте-эталоне",
            "Эффективная загрузка типа транспорта (в тоннах)", "Валюта-эталон", "Архив"
        },
        "Лидтаймы",
        new[]
        {
            "Код Лидтайма", "Период действия с", "Период действия по", "Тип отправки", "Лидтайм действует",
            "Коммерческий лидтайм", "Поиск, сут", "Погрузка, сут", "В пути, сут", "Ожидание, сут",
            "Разгрузка, сут", "Транспортировка, сут", "Расстояние, км",
            "Плечо 1.Поиск, сут", "Плечо 1.Погрузка, сут", "Плечо 1.В пути, сут", "Плечо 1.Ожидание, сут",
            "Плечо 1.Транспортировка", "Плечо 1.Расстояние, км",
            "Плечо 2.В пути, сут", "Плечо 2.Ожидание, сут", "Плечо 2.Разгрузка, сут",
            "Плечо 2.Транспортировка", "Плечо 2.Расстояние, км", "Архив"
        })]
    [InlineData("en", "en-US",
        "Leg rates",
        new[]
        {
            "Rate code", "Deflator", "Rate type", "Type of vehicle", "Product group", "Product",
            "Validity period from", "Validity period till", "Total cost of transportation per ton (excluding VAT)",
            "Total cost of transportation per vehicle (excluding VAT)", "Reference currency", "Archive"
        },
        "Rate Level3",
        new[]
        {
            "Rate code", "Deflator", "Rate type", "Type of vehicle", "Product group", "Product",
            "Validity period from", "Validity period till", "Avg.rate of level 3 in ref.currency",
            "Effective loading of the transport vehicle type (in tons)", "Reference currency", "Archive"
        },
        "Leadtime",
        new[]
        {
            "Leadtime code", "Validity period from", "Validity period till", "Shipment type", "Leadtime is active",
            "Commercial leadtime", "Search, days", "Loading, days", "In transit, days", "Waiting, days",
            "Unloading, days", "Transportation, days", "Distance, km",
            "Leg 1. Search, days", "Leg 1. Loading, days", "Leg 1. In transit, days", "Leg 1. Waiting, days",
            "Leg 1. Transportation", "Leg 1. Distance, km",
            "Leg 2. In transit, days", "Leg 2. Waiting, days", "Leg 2. Unloading, days",
            "Leg 2. Transportation", "Leg 2. Distance, km", "Archive"
        })]
    public async Task Sections_UseResxTitlesAndHeaders(
        string lang,
        string culture,
        string legRateTitle,
        string[] legRateHeaders,
        string averageTitle,
        string[] averageHeaders,
        string leadtimeTitle,
        string[] leadtimeHeaders)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<TransportLegResources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenLegRates(new LegRateDto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 });
            GivenAverageRates(new LegAverageRateLevel3Dto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 });
            GivenLeadtimes(new LeadtimeDto { TransportLegId = TransportLegId, Id = 1, PrimitiveEntityDataStateId = 1 });
            var legRate = new LegRateSection(dataService, resxLocalizer, resxPlatform);
            var average = new LegAverageRateLevel3Section(dataService, resxLocalizer, resxPlatform);
            var leadtime = new LeadtimeSection(dataService, resxLocalizer, resxPlatform);

            var legRateTable = await legRate.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);
            var averageTable = await average.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);
            var leadtimeTable = await leadtime.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            legRate.Title.Should().Be(legRateTitle);
            legRateTable.Headers().Should().Equal(legRateHeaders);
            average.Title.Should().Be(averageTitle);
            averageTable.Headers().Should().Equal(averageHeaders);
            leadtime.Title.Should().Be(leadtimeTitle);
            leadtimeTable.Headers().Should().Equal(leadtimeHeaders);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenLegRates(params LegRateDto[] rows) =>
        dataService
            .GetRowsAsync<LegRateDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<LegRateDto>(rows, rows.Length));

    private void GivenAverageRates(params LegAverageRateLevel3Dto[] rows) =>
        dataService
            .GetRowsAsync<LegAverageRateLevel3Dto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<LegAverageRateLevel3Dto>(rows, rows.Length));

    private void GivenLeadtimes(params LeadtimeDto[] rows) =>
        dataService
            .GetRowsAsync<LeadtimeDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<LeadtimeDto>(rows, rows.Length));

    private static string Text(object? value) =>
        Convert.ToString(value, CultureInfo.CurrentCulture) ?? string.Empty;

    private static TransportLegDetailDto Parent(bool isArchive) => new()
    {
        TransportLegId = TransportLegId,
        IsArchive = isArchive,
        Code = "L",
        CanBeUsed = true,
        TransportKindCode = "TK",
        TransportKindIdRu = TransportKindRu.Mix,
        TransportKindIdEn = TransportKindEn.Mix,
        SearchTimeT = "",
        LoadTimeT = "",
        DaysWaitingT = "",
        TravelTimeT = "",
        UnLoadTimeT = "",
        TransportationTimeT = "",
        Distance = 0,
        Leg1_SearchTime = 0,
        Leg1_LoadTime = 0,
        Leg1_TravelTime = 0,
        Leg1_DaysWaiting = 0,
        Leg1_TransportationTime = 0,
        Leg1_Distance = 0,
        Leg2_UpLoadTime = 0,
        Leg2_TravelTime = 0,
        Leg2_DaysWaiting = 0,
        Leg2_TransportationTime = 0,
        Leg2_Distance = 0,
        NodeFromCode = "",
        NodeFromNameRu = "",
        NodeFromNameEn = "",
        NodeToCode = "",
        NodeToNameRu = "",
        NodeToNameEn = ""
    };

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
