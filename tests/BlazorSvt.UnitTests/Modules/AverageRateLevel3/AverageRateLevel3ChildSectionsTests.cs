using System.Globalization;
using BlazorSvt.Modules.AverageRateLevel3;
using BlazorSvt.Modules.AverageRateLevel3.Detail;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Grid.Services;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;
using Microsoft.Extensions.Localization;
using Microsoft.Extensions.Logging.Abstractions;
using Microsoft.Extensions.Options;
using NSubstitute;
using AverageRateLevel3Resources = BlazorSvt.Modules.AverageRateLevel3.Resources.AverageRateLevel3;
using PlatformResources = BlazorSvt.Platform.Resources.Platform;

namespace BlazorSvt.UnitTests.Modules.AverageRateLevel3;

[Trait("Category", "Unit")]
public class AverageRateLevel3ChildSectionsTests
{
    private const long AverageRateLevel3Id = 35906580;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<AverageRateLevel3Resources> localizer = KeyLocalizer<AverageRateLevel3Resources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Fact]
    public async Task RatesSection_ActiveParent_ColumnsInOrder()
    {
        var ton = 1500.25m;
        var transport = 3200.50m;
        GivenRows(FilledRow(ton, transport));
        var section = new RatesOfAverageRateSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("RatesOfAverageRateDto.Title");
        table.Headers().Should().Equal(
            "RatesOfAverageRateDto.Code",
            "RatesOfAverageRateDto.TransportRateCode",
            "RatesOfAverageRateDto.TotalCostTon",
            "RatesOfAverageRateDto.TotalCostTransport");
        table.Cells().Single().Should().Equal(
            "R-1",
            "TR-9",
            Convert.ToString(ton, CultureInfo.CurrentCulture),
            Convert.ToString(transport, CultureInfo.CurrentCulture));
    }

    [Fact]
    public async Task RatesSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenRows(
            new RatesOfAverageRateDto { AverageRateLevel3Id = AverageRateLevel3Id, Id = 1, PrimitiveEntityDataStateId = 1 },
            new RatesOfAverageRateDto { AverageRateLevel3Id = AverageRateLevel3Id, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new RatesOfAverageRateSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);

        table.Headers().Should().HaveCount(5).And.EndWith("RatesOfAverageRateDto.IsArchive");
        table.Columns.Select(c => c.Name).Should().OnlyHaveUniqueItems();
        table.Cells().Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
        table.Columns[^1].Name.Should().Be("IsArchive");
        table.Rows.Select(r => r.IsArchive).Should().Equal(false, true);
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddAverageRateLevel3Module();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<AverageRateLevel3DetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(typeof(RatesOfAverageRateSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Ставки, вошедшие в расчет",
        new[]
        {
            "Код", "Код ставки", "Общая стоимость за тонну", "Общая стоимость за транспортное средство", "Архив"
        })]
    [InlineData("en", "en-US",
        "Rate For Rate",
        new[]
        {
            "Code", "Rate code",
            "Total cost of transportation per ton (excluding VAT)",
            "Total cost of transportation per vehicle (excluding VAT)",
            "Archive"
        })]
    public async Task Section_UsesResxTitleAndHeaders(string lang, string culture, string title, string[] headers)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<AverageRateLevel3Resources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenRows(new RatesOfAverageRateDto { AverageRateLevel3Id = AverageRateLevel3Id, Id = 1, PrimitiveEntityDataStateId = 1 });
            var section = new RatesOfAverageRateSection(dataService, resxLocalizer, resxPlatform);

            var table = await section.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            section.Title.Should().Be(title);
            table.Headers().Should().Equal(headers);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenRows(params RatesOfAverageRateDto[] rows) =>
        dataService
            .GetRowsAsync<RatesOfAverageRateDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<RatesOfAverageRateDto>(rows, rows.Length));

    private static RatesOfAverageRateDto FilledRow(decimal ton, decimal transport) => new()
    {
        AverageRateLevel3Id = AverageRateLevel3Id,
        Id = 1,
        PrimitiveEntityDataStateId = 1,
        Code = "R-1",
        TransportRateCode = "TR-9",
        TotalCostTon = ton,
        TotalCostTransport = transport
    };

    private static AverageRateLevel3DetailDto Parent(bool isArchive) => new()
    {
        AverageRateLevel3Id = AverageRateLevel3Id,
        IsArchive = isArchive,
        Code = 1,
        CurrencyStandard = "RUB",
        TypeCode = "T",
        TypeName = "Type",
        NodeFromCode = "F",
        NodeFromNameRu = "От",
        NodeFromNameEn = "From",
        NodeToCode = "T",
        NodeToNameRu = "До",
        NodeToNameEn = "To",
        TransportKindCode = "K",
        TransportKindNameRu = "Вид",
        TransportKindNameRuEn = "Kind",
        TransportTypeCode = "TT",
        TransportTypeNameRu = "Тип",
        TransportTypeNameRuEn = "Type",
        LegCode = "L"
    };

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
