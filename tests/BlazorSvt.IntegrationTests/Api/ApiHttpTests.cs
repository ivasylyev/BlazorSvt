using System.Net;
using System.Net.Http.Headers;
using System.Net.Http.Json;
using System.Text;
using System.Text.Json;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Api;

/// <summary>HTTP-контракт <c>/api</c>: доступ, заголовки, ошибки, сериализация. Без БД.</summary>
[Trait("Category", "Integration")]
public sealed class ApiHttpTests : IClassFixture<ApiTestFactory>
{
    private const string SearchUrl = "/api/v1/catalogs/transport-rates/items/search";
    private readonly ApiTestFactory factory;

    public ApiHttpTests(ApiTestFactory factory)
    {
        this.factory = factory;
        factory.TransportRates.ClearReceivedCalls();
    }

    [Fact]
    public async Task Anonymous_Returns401()
    {
        var response = await factory.CreateApiClient(caller: null).GetAsync("/api/v1/catalogs");

        await ShouldBeProblem(response, HttpStatusCode.Unauthorized, "unauthenticated");
    }

    [Fact]
    public async Task UntrustedClient_Returns403_OnWholeApi()
    {
        var client = factory.CreateApiClient(caller: @"TEST\someone");

        await ShouldBeProblem(await client.GetAsync("/api/v1/catalogs"), HttpStatusCode.Forbidden, "client_not_trusted");
        await ShouldBeProblem(await client.GetAsync("/api/openapi/v1.json"), HttpStatusCode.Forbidden, "client_not_trusted");
    }

    [Theory]
    [InlineData(null, HttpStatusCode.BadRequest, "on_behalf_of_required")]
    [InlineData("ivanov", HttpStatusCode.BadRequest, "on_behalf_of_invalid")]
    [InlineData(@"TEST\stranger", HttpStatusCode.Forbidden, "user_not_registered")]
    [InlineData(ApiTestFactory.UserWithoutRoles, HttpStatusCode.Forbidden, "user_has_no_roles")]
    public async Task OnBehalfOf_IsRequiredAndChecked(string? onBehalfOf, HttpStatusCode status, string code)
    {
        var response = await factory.CreateApiClient(onBehalfOf: onBehalfOf).GetAsync("/api/v1/catalogs");

        await ShouldBeProblem(response, status, code);
    }

    [Fact]
    public async Task OpenApi_ForTrustedClient_WithoutOnBehalfOf()
    {
        var response = await factory.CreateApiClient(onBehalfOf: null).GetAsync("/api/openapi/v1.json");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        (await response.Content.ReadAsStringAsync()).Should().Contain("\"operationId\": \"smartRateLookup\"");
    }

    [Fact]
    public async Task Catalogs_ReturnsListWithContractHeaders()
    {
        var client = factory.CreateApiClient();
        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/catalogs");
        request.Headers.Add("X-Correlation-ID", "atlas-123");

        var response = await client.SendAsync(request);

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.GetValues("Sunset").Should().Equal("Fri, 01 Jan 2099 00:00:00 GMT");
        response.Headers.GetValues("X-Correlation-ID").Should().Equal("atlas-123");
        response.Headers.ETag.Should().NotBeNull();

        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var codes = json.RootElement.GetProperty("items").EnumerateArray()
            .Select(c => c.GetProperty("code").GetString()).ToArray();
        codes.Should().Contain(["transport-rates", "average-rates-level3", "transport-legs", "locations-nodes"]);
    }

    [Fact]
    public async Task Catalogs_WithMatchingETag_Returns304()
    {
        var client = factory.CreateApiClient();
        var first = await client.GetAsync("/api/v1/catalogs");

        using var request = new HttpRequestMessage(HttpMethod.Get, "/api/v1/catalogs");
        request.Headers.IfNoneMatch.Add(first.Headers.ETag!);
        var second = await client.SendAsync(request);

        second.StatusCode.Should().Be(HttpStatusCode.NotModified);
    }

    [Fact]
    public async Task Schema_DescribesKeyPeriodAndReferences()
    {
        var response = await factory.CreateApiClient().GetAsync("/api/v1/catalogs/transport-rates/schema");

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        using var json = JsonDocument.Parse(await response.Content.ReadAsStringAsync());
        var root = json.RootElement;
        root.GetProperty("entityKey").GetString().Should().Be("transportRateId");
        root.GetProperty("period").GetProperty("from").GetString().Should().Be("startDate");

        var fields = root.GetProperty("fields").EnumerateArray().ToDictionary(f => f.GetProperty("name").GetString()!);
        fields["nodeFromCode"].GetProperty("reference").GetProperty("catalog").GetString().Should().Be("locations-nodes");
        fields["nodeFromCode"].GetProperty("operators").EnumerateArray().Select(o => o.GetString()).Should().Equal("contains");
        fields["currencyId"].GetProperty("type").GetString().Should().Be("enum");
        fields["currencyId"].GetProperty("enumValues").EnumerateArray()
            .Select(v => v.GetProperty("code").GetString()).Should().Contain("RUB");
    }

    [Fact]
    public async Task Search_ReturnsPageInContractFormat_NoStore()
    {
        factory.TransportRates.GetPageAsync(Arg.Any<GridQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GridPage<TransportRateDto>([Rate()], 1));
        factory.Freshness.GetDataAsOfAsync("TransportRate", Arg.Any<CancellationToken>())
            .Returns(new DateTime(2026, 10, 20, 7, 0, 0, DateTimeKind.Utc));

        var response = await factory.CreateApiClient().PostAsync(SearchUrl, Json("""
            { "filters": [ { "field": "currencyId", "operator": "eq", "value": "RUB" } ], "asOf": "2026-10-20", "pageSize": 10 }
            """));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        response.Headers.CacheControl!.NoStore.Should().BeTrue();
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"totalCount\":1")
            .And.Contain("\"dataAsOf\":\"2026-10-20T07:00:00.000Z\"")
            .And.Contain("\"currencyId\":\"RUB\"")
            .And.Contain("\"startDate\":\"2026-10-01\"")
            .And.Contain("\"nodeFromNameEn\":null");

        await factory.TransportRates.Received(1).GetPageAsync(
            Arg.Is<GridQuery>(q =>
                q.PageSize == 10
                && q.Filters.Any(f => f.PropertyName == "CurrencyId" && f.Value == "32694")
                && q.Filters.Any(f => f.PropertyName == "StartDate" && f.Value == "2026-10-20")
                && q.Filters.Any(f => f.PropertyName == "IsArchive" && f.Value == "False")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task Search_InvalidRequest_Returns400PerField()
    {
        var response = await factory.CreateApiClient().PostAsync(SearchUrl, Json("""
            { "filters": [ { "field": "nodeFromCode", "operator": "contains", "value": "ab" }, { "field": "nope", "operator": "eq", "value": 1 } ], "pageSize": 5000 }
            """));

        using var json = await ShouldBeProblem(response, HttpStatusCode.BadRequest, "validation_failed");
        var errors = json.RootElement.GetProperty("errors");
        errors.GetProperty("filters[0].value")[0].GetString().Should().Be("value_too_short");
        errors.GetProperty("filters[1].field")[0].GetString().Should().Be("unknown_field");
        errors.GetProperty("pageSize")[0].GetString().Should().Be("out_of_range");
        await factory.TransportRates.DidNotReceiveWithAnyArgs().GetPageAsync(default!, default);
    }

    [Fact]
    public async Task Search_MalformedJson_Returns400WithCode()
    {
        var response = await factory.CreateApiClient().PostAsync(SearchUrl, Json("{ \"filters\": [ "));

        await ShouldBeProblem(response, HttpStatusCode.BadRequest, "invalid_request");
    }

    [Fact]
    public async Task Item_NotFound_Returns404()
    {
        factory.TransportRates.FindDetailAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns((TransportRateDetailDto?)null);

        var response = await factory.CreateApiClient().GetAsync("/api/v1/catalogs/transport-rates/items/42");

        await ShouldBeProblem(response, HttpStatusCode.NotFound, "item_not_found");
    }

    [Fact]
    public async Task UnknownCatalog_Returns404()
    {
        var response = await factory.CreateApiClient().GetAsync("/api/v1/catalogs/unknown/schema");

        await ShouldBeProblem(response, HttpStatusCode.NotFound, "catalog_not_found");
    }

    [Fact]
    public async Task SmartLookup_MissingFields_Returns400()
    {
        var response = await factory.CreateApiClient().PostAsync("/api/v1/rates/smart-lookup", Json("{}"));

        using var json = await ShouldBeProblem(response, HttpStatusCode.BadRequest, "validation_failed");
        json.RootElement.GetProperty("errors").EnumerateObject().Select(p => p.Name)
            .Should().Contain(["nodeFromCode", "nodeToCode", "productCode", "transportTypeCode", "currencyCode"]);
    }

    [Fact]
    public async Task SmartLookup_NothingFound_Returns200FoundFalse()
    {
        SetupLookupReferences();
        factory.SmartLookup.GetCandidatesAsync(default!, default, default!, default).ReturnsForAnyArgs([]);

        var response = await factory.CreateApiClient().PostAsync("/api/v1/rates/smart-lookup", Json(LookupBody));

        response.StatusCode.Should().Be(HttpStatusCode.OK);
        var body = await response.Content.ReadAsStringAsync();
        body.Should().Contain("\"found\":false").And.Contain("\"rate\":null").And.Contain("\"profile\":\"Импорт в CRM\"");
    }

    [Fact]
    public async Task SmartLookup_NoExchangeRate_Returns422()
    {
        SetupLookupReferences();
        factory.SmartLookup.GetCandidatesAsync(default!, default, default!, default).ReturnsForAnyArgs([UsdCandidate()]);
        factory.SmartLookup.ConvertCurrencyAsync(default, default, default, default, default).ReturnsForAnyArgs((decimal?)null);

        var response = await factory.CreateApiClient().PostAsync("/api/v1/rates/smart-lookup", Json(LookupBody));

        await ShouldBeProblem(response, HttpStatusCode.UnprocessableEntity, "currency_rate_missing");
    }

    private const string LookupBody = """
        { "nodeFromCode": "A1", "nodeToCode": "B1", "productCode": "P1", "transportTypeCode": "Auto_20", "currencyCode": "RUB", "date": "2026-10-20" }
        """;

    private void SetupLookupReferences() =>
        factory.SmartLookup.ResolveAsync(default!, default!, default!, default!, default!, default!, default)
            .ReturnsForAnyArgs(new LookupReferences
            {
                NodeFromId = 100,
                NodeToId = 200,
                ProductId = 10,
                TransportTypeId = 7,
                TransportKindId = 70,
                CurrencyId = (long)CurrencyRuEn.RUB,
                ProfileExists = true
            });

    private static CandidateRow UsdCandidate() =>
        new()
        {
            AverageRateLevel3Id = 1,
            NodeFromId = 100,
            NodeToId = 200,
            ProductId = 10,
            TransportTypeId = 7,
            TransportKindId = 70,
            RateTypePriority = 1,
            StartDate = new DateTime(2026, 10, 1),
            EndDate = new DateTime(2026, 12, 31),
            RateLevel3 = 100,
            CurrencyId = (long)CurrencyRuEn.USD,
            CurrencyCode = "USD"
        };

    private static TransportRateDto Rate()
    {
        var rate = (TransportRateDto)Activator.CreateInstance(typeof(TransportRateDto))!;
        rate.TransportRateId = 7;
        rate.CurrencyId = CurrencyRuEn.RUB;
        rate.StartDate = new DateOnly(2026, 10, 1);
        rate.EndDate = new DateOnly(2026, 12, 31);
        rate.NodeFromCode = "A1";
        return rate;
    }

    private static StringContent Json(string body) => new(body, Encoding.UTF8, "application/json");

    private static async Task<JsonDocument> ShouldBeProblem(HttpResponseMessage response, HttpStatusCode status, string code)
    {
        var body = await response.Content.ReadAsStringAsync();
        response.StatusCode.Should().Be(status, body);
        response.Content.Headers.ContentType!.MediaType.Should().Be("application/problem+json");
        var json = JsonDocument.Parse(body);
        json.RootElement.GetProperty("code").GetString().Should().Be(code);
        json.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
        return json;
    }
}
