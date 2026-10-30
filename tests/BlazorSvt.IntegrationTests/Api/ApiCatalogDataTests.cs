using System.Globalization;
using System.Net;
using System.Text;
using System.Text.Json;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Api;

/// <summary>
/// HTTP API поверх живой <c>mdm</c>: поиск с фильтрами, карточка, пустой результат.
/// Skip, если БД недоступна (<c>BLAZORSVT_TEST_CONNECTION</c> / appsettings).
/// </summary>
[Trait("Category", "Integration")]
public sealed class ApiCatalogDataTests : IClassFixture<ApiDataTestFactory>
{
    private static readonly string[] CatalogCodes =
    [
        "transport-legs",
        "transport-rates",
        "average-rates-level3",
        "parity-rates",
        "locations-nodes",
        "rate-types",
        "currencies"
    ];

    private static readonly (string Catalog, string EntityKey)[] CatalogKeys =
    [
        ("transport-legs", "transportLegId"),
        ("transport-rates", "transportRateId"),
        ("average-rates-level3", "averageRateLevel3Id"),
        ("parity-rates", "parityRatesId"),
        ("locations-nodes", "locationsNodesId"),
        ("rate-types", "rateTypeId"),
        ("currencies", "currencyId")
    ];

    private readonly ApiDataTestFactory factory;

    public ApiCatalogDataTests(ApiDataTestFactory factory) => this.factory = factory;

    [SkippableFact]
    public async Task Catalogs_ListsAllRegisteredCodes()
    {
        RequireDatabase();
        var response = await factory.CreateApiClient().GetAsync("/api/v1/catalogs");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var codes = json.RootElement.GetProperty("items").EnumerateArray()
            .Select(c => c.GetProperty("code").GetString())
            .ToArray();
        codes.Should().BeEquivalentTo(CatalogCodes);
    }

    [SkippableTheory]
    [MemberData(nameof(AllCatalogKeys))]
    public async Task Search_FirstPage_ReturnsRows(string catalog, string entityKey)
    {
        RequireDatabase();
        var (status, root) = await SearchAsync(catalog, """{ "page": 1, "pageSize": 10 }""");

        status.Should().Be(HttpStatusCode.OK);
        root.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);
        var items = root.GetProperty("items").EnumerateArray().ToList();
        items.Should().NotBeEmpty().And.HaveCountLessThanOrEqualTo(10);
        foreach (var item in items)
        {
            item.TryGetProperty(entityKey, out _).Should().BeTrue($"item must have {entityKey}");
        }
    }

    [SkippableTheory]
    [MemberData(nameof(AllCatalogKeys))]
    public async Task Item_ByIdFromSearch_ReturnsCard(string catalog, string entityKey)
    {
        RequireDatabase();
        var (_, search) = await SearchAsync(catalog, """{ "page": 1, "pageSize": 1 }""");
        var id = search.GetProperty("items")[0].GetProperty(entityKey).GetInt64();

        var response = await factory.CreateApiClient().GetAsync($"/api/v1/catalogs/{catalog}/items/{id}");
        response.StatusCode.Should().Be(HttpStatusCode.OK, await response.Content.ReadAsStringAsync());

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        json.RootElement.GetProperty(entityKey).GetInt64().Should().Be(id);
    }

    [SkippableFact]
    public async Task Item_UnknownId_Returns404()
    {
        RequireDatabase();
        var response = await factory.CreateApiClient()
            .GetAsync("/api/v1/catalogs/transport-rates/items/0");

        await ShouldBeProblem(response, HttpStatusCode.NotFound, "item_not_found");
    }

    [SkippableFact]
    public async Task Search_ParityRates_BusinessFilters_ReturnsRows()
    {
        RequireDatabase();
        var (status, root) = await SearchAsync("parity-rates", """
            {
              "filters": [
                { "field": "startDate", "operator": "ge", "value": "2026-01-01" },
                { "field": "relevanceIdRu", "operator": "eq", "value": "Month" },
                { "field": "nodeFromNameRu", "operator": "contains", "value": "ЦИНДАО" },
                { "field": "productGroupNameRu", "operator": "contains", "value": "Полиэфиры" }
              ],
              "page": 1,
              "pageSize": 100
            }
            """);

        status.Should().Be(HttpStatusCode.OK);
        root.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);

        foreach (var item in root.GetProperty("items").EnumerateArray())
        {
            DateOnly.Parse(item.GetProperty("startDate").GetString()!, CultureInfo.InvariantCulture)
                .Should().BeOnOrAfter(new DateOnly(2026, 1, 1));
            item.GetProperty("relevanceIdRu").GetString().Should().Be("Month");
            item.GetProperty("nodeFromNameRu").GetString().Should().ContainEquivalentOf("ЦИНДАО");
            item.GetProperty("productGroupNameRu").GetString().Should().ContainEquivalentOf("Полиэфиры");
        }
    }

    [SkippableFact]
    public async Task Search_TransportRates_CostAndCurrency_ReturnsRows()
    {
        RequireDatabase();
        var (status, root) = await SearchAsync("transport-rates", """
            {
              "filters": [
                { "field": "totalCostTon", "operator": "eq", "value": 100 },
                { "field": "currencyId", "operator": "eq", "value": "USD" }
              ],
              "page": 1,
              "pageSize": 100
            }
            """);

        status.Should().Be(HttpStatusCode.OK);
        root.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);

        foreach (var item in root.GetProperty("items").EnumerateArray())
        {
            item.GetProperty("totalCostTon").GetDecimal().Should().Be(100m);
            item.GetProperty("currencyId").GetString().Should().Be("USD");
        }
    }

    [SkippableFact]
    public async Task Search_LocationsNodes_NameCountryCreated_ReturnsRows()
    {
        RequireDatabase();
        var (status, root) = await SearchAsync("locations-nodes", """
            {
              "filters": [
                { "field": "nameRu", "operator": "contains", "value": "Москва" },
                { "field": "countryNameRu", "operator": "contains", "value": "Российская" },
                { "field": "creationDate", "operator": "ge", "value": "2019-01-01" }
              ],
              "page": 1,
              "pageSize": 100
            }
            """);

        status.Should().Be(HttpStatusCode.OK);
        root.GetProperty("totalCount").GetInt32().Should().BeGreaterThan(0);

        foreach (var item in root.GetProperty("items").EnumerateArray())
        {
            item.GetProperty("nameRu").GetString().Should().ContainEquivalentOf("Москва");
            item.GetProperty("countryNameRu").GetString().Should().ContainEquivalentOf("Российская");
            DateOnly.FromDateTime(item.GetProperty("creationDate").GetDateTime())
                .Should().BeOnOrAfter(new DateOnly(2019, 1, 1));
        }
    }

    [SkippableFact]
    public async Task Search_NonMatchingTextFilter_ReturnsZeroRows()
    {
        RequireDatabase();
        var (status, root) = await SearchAsync("locations-nodes", """
            {
              "filters": [
                { "field": "nameRu", "operator": "contains", "value": "йцукенгшщз" }
              ],
              "page": 1,
              "pageSize": 100
            }
            """);

        status.Should().Be(HttpStatusCode.OK);
        root.GetProperty("totalCount").GetInt32().Should().Be(0);
        root.GetProperty("items").GetArrayLength().Should().Be(0);
    }

    [SkippableFact]
    public async Task Schema_TransportLegs_ExposesEntityKeyAndNodeOperators()
    {
        RequireDatabase();
        var response = await factory.CreateApiClient().GetAsync("/api/v1/catalogs/transport-legs/schema");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;
        root.GetProperty("entityKey").GetString().Should().Be("transportLegId");
        var fields = root.GetProperty("fields").EnumerateArray()
            .ToDictionary(f => f.GetProperty("name").GetString()!);
        fields["nodeFromNameRu"].GetProperty("operators").EnumerateArray()
            .Select(o => o.GetString()).Should().Equal("contains");
    }

    public static TheoryData<string, string> AllCatalogKeys()
    {
        var data = new TheoryData<string, string>();
        foreach (var (catalog, key) in CatalogKeys)
        {
            data.Add(catalog, key);
        }

        return data;
    }

    private void RequireDatabase() =>
        Skip.IfNot(
            factory.IsDatabaseAvailable,
            "Database is not available. Set BLAZORSVT_TEST_CONNECTION or provide appsettings.json.");

    private async Task<(HttpStatusCode Status, JsonElement Root)> SearchAsync(string catalog, string body)
    {
        var requestUri = $"/api/v1/catalogs/{catalog}/items/search";
        var response = await factory.CreateApiClient()
            .PostAsync(requestUri, Json(body));
        var text = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(HttpStatusCode.OK, text);
        using var doc = JsonDocument.Parse(text);
        return (response.StatusCode, doc.RootElement.Clone());
    }

    private static StringContent Json(string body) => new(body, Encoding.UTF8, "application/json");

    private static async Task ShouldBeProblem(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(status, body);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        using var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("code").GetString().Should().Be(code);
    }
}
