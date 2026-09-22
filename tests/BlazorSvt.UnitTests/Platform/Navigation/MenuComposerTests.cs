using System.Globalization;
using System.Resources;
using BlazorSvt.Import;
using BlazorSvt.Modules.AverageRateLevel3;
using BlazorSvt.Modules.HomeRatePivot;
using BlazorSvt.Modules.LocationsNodes;
using BlazorSvt.Modules.ParityRates;
using BlazorSvt.Modules.TransportLeg;
using BlazorSvt.Modules.TransportRate;
using BlazorSvt.Platform.UI.Navigation;
using FluentAssertions;
using Microsoft.Extensions.DependencyInjection;

namespace BlazorSvt.UnitTests.Platform.Navigation;

[Trait("Category", "Unit")]
public class MenuComposerTests
{
    [Fact]
    public void All_ListsDomainsInMenuOrder()
    {
        CatalogDomains.All.Select(domain => domain.Domain).Should().Equal(
            CatalogDomain.Rates,
            CatalogDomain.Routes,
            CatalogDomain.Products,
            CatalogDomain.Warehouses,
            CatalogDomain.MK,
            CatalogDomain.Cbd,
            CatalogDomain.Other);

        Enum.GetValues<CatalogDomain>().Should().Equal(CatalogDomains.All.Select(domain => domain.Domain));
    }

    [Fact]
    public void Compose_OrdersDomainsAndSortsCaptionsAlphabetically()
    {
        var contributions = new[]
        {
            new CatalogMenuContribution(CatalogDomain.Other, "aux", "k-other", VisibleToEditor: true),
            new CatalogMenuContribution(CatalogDomain.Rates, "rates-b", "k-b", VisibleToEditor: true),
            new CatalogMenuContribution(CatalogDomain.Rates, "rates-a", "k-a", VisibleToEditor: false),
            new CatalogMenuContribution(CatalogDomain.Cbd, "cbd", "k-cbd", VisibleToEditor: true)
        };

        var labels = new Dictionary<string, string>
        {
            ["k-other"] = "Beta",
            ["k-b"] = "Bravo",
            ["k-a"] = "Alpha",
            ["k-cbd"] = "Channel"
        };

        var groups = MenuComposer.Compose(
            contributions,
            key => labels[key],
            CultureInfo.GetCultureInfo("en-US"));

        groups.Select(group => group.Domain).Should().Equal(
            CatalogDomain.Rates,
            CatalogDomain.Cbd,
            CatalogDomain.Other);
        groups[0].Items.Select(item => item.Text).Should().Equal("Alpha", "Bravo");
        groups[0].Items.Select(item => item.VisibleToEditor).Should().Equal(false, true);
        groups[0].TextResourceKey.Should().Be("HeaderMenu.Domain.Rates");
        groups[0].HintResourceKey.Should().Be("HeaderMenu.Domain.Rates.Hint");
    }

    [Fact]
    public void Compose_SortsRussianCaptionsByCurrentCulture()
    {
        var contributions = new[]
        {
            new CatalogMenuContribution(CatalogDomain.Rates, "transportrate", "rates", VisibleToEditor: true),
            new CatalogMenuContribution(CatalogDomain.Rates, "averageratelevel3", "average", VisibleToEditor: true),
            new CatalogMenuContribution(CatalogDomain.Rates, "parityrates", "parity", VisibleToEditor: true)
        };

        var labels = new Dictionary<string, string>
        {
            ["rates"] = "Ставки",
            ["average"] = "Средневзвешенные ставки",
            ["parity"] = "Паритетные ставки"
        };

        var groups = MenuComposer.Compose(
            contributions,
            key => labels[key],
            CultureInfo.GetCultureInfo("ru-RU"));

        groups.Single().Items.Select(item => item.Text).Should().Equal(
            "Паритетные ставки",
            "Средневзвешенные ставки",
            "Ставки");
    }

    [Fact]
    public void Compose_TieBreaksEqualCaptionsByUrl()
    {
        var contributions = new[]
        {
            new CatalogMenuContribution(CatalogDomain.Rates, "b-rate", "same", VisibleToEditor: true),
            new CatalogMenuContribution(CatalogDomain.Rates, "a-rate", "same", VisibleToEditor: true)
        };

        var groups = MenuComposer.Compose(
            contributions,
            _ => "Ставки",
            CultureInfo.GetCultureInfo("ru-RU"));

        groups.Single().Items.Select(item => item.Url).Should().Equal("a-rate", "b-rate");
    }

    [Fact]
    public void Compose_OmitsDomainsWithoutContributions()
    {
        var contributions = new[]
        {
            new CatalogMenuContribution(CatalogDomain.Routes, "transportleg", "legs", VisibleToEditor: true)
        };

        var groups = MenuComposer.Compose(
            contributions,
            key => key,
            CultureInfo.GetCultureInfo("en-US"));

        groups.Should().ContainSingle();
        groups[0].Domain.Should().Be(CatalogDomain.Routes);
    }

    [Theory]
    [InlineData("en-US", "HeaderMenu.Domain.Rates", "Rates")]
    [InlineData("en-US", "HeaderMenu.Domain.Rates.Hint", "Rates")]
    [InlineData("en-US", "HeaderMenu.Domain.Routes", "Routes")]
    [InlineData("en-US", "HeaderMenu.Domain.Routes.Hint", "Routes")]
    [InlineData("en-US", "HeaderMenu.Domain.Products", "Products")]
    [InlineData("en-US", "HeaderMenu.Domain.Products.Hint", "Products")]
    [InlineData("en-US", "HeaderMenu.Domain.Warehouses", "Warehouses")]
    [InlineData("en-US", "HeaderMenu.Domain.Warehouses.Hint", "Warehouses and restrictions")]
    [InlineData("en-US", "HeaderMenu.Domain.MK", "MK")]
    [InlineData("en-US", "HeaderMenu.Domain.MK.Hint", "Channel matrix")]
    [InlineData("en-US", "HeaderMenu.Domain.Cbd", "CBD")]
    [InlineData("en-US", "HeaderMenu.Domain.Cbd.Hint", "CBD integration")]
    [InlineData("en-US", "HeaderMenu.Domain.Other", "Auxiliary")]
    [InlineData("en-US", "HeaderMenu.Domain.Other.Hint", "Auxiliary catalogs")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Rates", "Ставки")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Rates.Hint", "Ставки")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Routes", "Маршруты")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Routes.Hint", "Маршруты")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Products", "Продукты")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Products.Hint", "Продукты")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Warehouses", "Склады")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Warehouses.Hint", "Склады и ограничения")]
    [InlineData("ru-RU", "HeaderMenu.Domain.MK", "МК")]
    [InlineData("ru-RU", "HeaderMenu.Domain.MK.Hint", "Матрица каналов")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Cbd", "ЦБД")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Cbd.Hint", "Интеграция с ЦБД")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Other", "Вспомогательные")]
    [InlineData("ru-RU", "HeaderMenu.Domain.Other.Hint", "Вспомогательные справочники")]
    public void DomainResources_MatchCatalog(string cultureName, string key, string expected)
    {
        var value = Resources.GetString(key, CultureInfo.GetCultureInfo(cultureName));

        value.Should().Be(expected);
    }

    [Fact]
    public void RegisteredCatalogs_AppearInLocalizedAlphabeticalOrder()
    {
        var services = new ServiceCollection();
        services.AddImportModule();
        services.AddHomeRatePivotModule();
        services.AddTransportRateModule();
        services.AddParityRatesModule();
        services.AddAverageRateLevel3Module();
        services.AddTransportLegModule();
        services.AddLocationsNodesModule();

        var contributions = services.BuildServiceProvider().GetServices<CatalogMenuContribution>().ToList();

        contributions.Should().BeEquivalentTo(
        [
            new CatalogMenuContribution(CatalogDomain.Rates, "transportrate", "HeaderMenu.TransportRate", true),
            new CatalogMenuContribution(CatalogDomain.Rates, "averageratelevel3", "HeaderMenu.AverageRateLevel3", true),
            new CatalogMenuContribution(CatalogDomain.Rates, "parityrates", "HeaderMenu.ParityRates", true),
            new CatalogMenuContribution(CatalogDomain.Routes, "transportleg", "HeaderMenu.TransportLeg", true),
            new CatalogMenuContribution(CatalogDomain.Routes, "locationsnodes", "HeaderMenu.LocationsNodes", true)
        ]);

        var ru = MenuComposer.Compose(
            contributions,
            key => Resources.GetString(key, CultureInfo.GetCultureInfo("ru-RU"))!,
            CultureInfo.GetCultureInfo("ru-RU"));
        ru.Select(group => group.Domain).Should().Equal(CatalogDomain.Rates, CatalogDomain.Routes);
        ru[0].Items.Select(item => item.Text).Should().Equal(
            "Паритетные ставки",
            "Средневзвешенные ставки",
            "Ставки");
        ru[1].Items.Select(item => item.Text).Should().Equal("Местоположения-Узлы", "Транспортные плечи");

        var en = MenuComposer.Compose(
            contributions,
            key => Resources.GetString(key, CultureInfo.GetCultureInfo("en-US"))!,
            CultureInfo.GetCultureInfo("en-US"));
        en[0].Items.Select(item => item.Text).Should().Equal("Average Rate", "Parity rates", "Rates");
        en[1].Items.Select(item => item.Text).Should().Equal("Locations-Nodes", "Transport Legs");
    }

    private static readonly ResourceManager Resources = new(
        "BlazorSvt.Platform.Resources.Platform",
        typeof(BlazorSvt.Platform.Resources.Platform).Assembly);
}
