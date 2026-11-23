using System.Globalization;
using BlazorSvt.Modules.TransportRoute;
using BlazorSvt.Modules.TransportRoute.Detail;
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
using TransportRouteResources = BlazorSvt.Modules.TransportRoute.Resources.TransportRoute;

namespace BlazorSvt.UnitTests.Modules.TransportRoute;

[Trait("Category", "Unit")]
public class TransportRouteChildSectionsTests
{
    private const long TransportRouteId = 18133141;

    private readonly IDetailChildDataService dataService = Substitute.For<IDetailChildDataService>();
    private readonly IStringLocalizer<TransportRouteResources> localizer = KeyLocalizer<TransportRouteResources>();
    private readonly IStringLocalizer<PlatformResources> platform = KeyLocalizer<PlatformResources>();

    [Fact]
    public async Task LegSection_ActiveParent_ColumnsInOrder()
    {
        GivenRows(FilledRow());
        var section = new TransportRouteLegSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: false), "en", CancellationToken.None);

        section.Title.Should().Be("TransportRouteLegDto.Title");
        table.Headers.Should().Equal(
            "TransportRouteLegDto.Code",
            "TransportRouteLegDto.Step",
            "TransportRouteLegDto.LegCode",
            "TransportRouteLegDto.NodeFromName",
            "TransportRouteLegDto.ProxyNodeName",
            "TransportRouteLegDto.NodeToName",
            "TransportRouteLegDto.TransportKindName",
            "TransportRouteLegDto.Leg1_TransportTypeName",
            "TransportRouteLegDto.Leg2_TransportTypeName");
        table.Rows.Single().Should().Equal(
            "L-1",
            2.0000m.ToString(CultureInfo.CurrentCulture),
            "LEG-1",
            "From node",
            "Proxy node",
            "To node",
            "Multimodal",
            "Mix: Rail container 40ft",
            "Mix: Sea");
    }

    [Fact]
    public async Task LegSection_ArchivedParent_AddsArchiveColumnLast()
    {
        GivenRows(
            new TransportRouteLegDto { TransportRouteId = TransportRouteId, Id = 1, PrimitiveEntityDataStateId = 1 },
            new TransportRouteLegDto { TransportRouteId = TransportRouteId, Id = 2, PrimitiveEntityDataStateId = 2 });
        var section = new TransportRouteLegSection(dataService, localizer, platform);

        var table = await section.LoadAsync(Parent(isArchive: true), "ru", CancellationToken.None);

        table.Headers.Should().HaveCount(10).And.EndWith("TransportRouteLegDto.IsArchive");
        table.Rows.Select(r => r[^1]).Should().Equal("Common.Active", "Common.Archive");
    }

    [Fact]
    public void Module_RegistersSectionsInGroupOrder()
    {
        var services = new ServiceCollection();
        services.AddTransportRouteModule();

        services
            .Where(d => d.ServiceType == typeof(IDetailChildSection<TransportRouteDetailDto>))
            .Select(d => d.ImplementationType)
            .Should().Equal(typeof(TransportRouteLegSection));
    }

    [Theory]
    [InlineData("ru", "ru-RU",
        "Плечо маршрута",
        new[]
        {
            "Код", "Этап", "Плечо", "Узел отправления", "Узел промежуточный", "Узел назначения",
            "Вид транспорта", "Плечо 1.Тип транспорта", "Плечо 2.Тип транспорта", "Архив"
        })]
    [InlineData("en", "en-US",
        "Route leg",
        new[]
        {
            "Code", "Stage", "Leg", "Departure node", "Proxy node", "Destination node",
            "Means of transportation", "Leg 1 transport type", "Leg 2 transport type", "Archive"
        })]
    public async Task Section_UsesResxTitleAndHeaders(string lang, string culture, string title, string[] headers)
    {
        var previousUiCulture = CultureInfo.CurrentUICulture;
        CultureInfo.CurrentUICulture = CultureInfo.GetCultureInfo(culture);
        try
        {
            var factory = new ResourceManagerStringLocalizerFactory(
                Options.Create(new LocalizationOptions()), NullLoggerFactory.Instance);
            var resxLocalizer = new StringLocalizer<TransportRouteResources>(factory);
            var resxPlatform = new StringLocalizer<PlatformResources>(factory);
            GivenRows(new TransportRouteLegDto { TransportRouteId = TransportRouteId, Id = 1, PrimitiveEntityDataStateId = 1 });
            var section = new TransportRouteLegSection(dataService, resxLocalizer, resxPlatform);

            var table = await section.LoadAsync(Parent(isArchive: true), lang, CancellationToken.None);

            section.Title.Should().Be(title);
            table.Headers.Should().Equal(headers);
        }
        finally
        {
            CultureInfo.CurrentUICulture = previousUiCulture;
        }
    }

    private void GivenRows(params TransportRouteLegDto[] rows) =>
        dataService
            .GetRowsAsync<TransportRouteLegDto>(Arg.Any<object>(), Arg.Any<IReadOnlyCollection<int>>(), Arg.Any<CancellationToken>())
            .Returns(new DetailChildPage<TransportRouteLegDto>(rows, rows.Length));

    private static TransportRouteLegDto FilledRow() => new()
    {
        TransportRouteId = TransportRouteId,
        Id = 1,
        PrimitiveEntityDataStateId = 1,
        Code = "L-1",
        Step = 2.0000m,
        LegCode = "LEG-1",
        NodeFromNameRu = "Откуда",
        NodeFromNameEn = "From node",
        ProxyNodeNameRu = "Промежуточный",
        ProxyNodeNameEn = "Proxy node",
        NodeToNameRu = "Куда",
        NodeToNameEn = "To node",
        TransportKindIdRu = TransportKindRu.Mix,
        TransportKindIdEn = TransportKindEn.Mix,
        Leg1_TransportTypeIdRu = TransportTypeLevel3Ru.Mix_rail_cont_40,
        Leg1_TransportTypeIdEn = TransportTypeLevel3En.Mix_rail_cont_40,
        Leg2_TransportTypeIdRu = TransportTypeLevel3Ru.Mix_sea,
        Leg2_TransportTypeIdEn = TransportTypeLevel3En.Mix_sea
    };

    private static TransportRouteDetailDto Parent(bool isArchive) => new()
    {
        TransportRouteId = TransportRouteId,
        IsArchive = isArchive,
        Code = "R",
        RouteTransportKindIdRu = TransportKindRu.Mix,
        RouteTransportKindIdEn = TransportKindEn.Mix
    };

    private static IStringLocalizer<T> KeyLocalizer<T>()
    {
        var localizer = Substitute.For<IStringLocalizer<T>>();
        localizer[Arg.Any<string>()].Returns(call => new LocalizedString(call.Arg<string>(), call.Arg<string>()));
        return localizer;
    }
}
