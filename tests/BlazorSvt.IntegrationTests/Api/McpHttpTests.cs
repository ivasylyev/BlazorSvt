using System.Net;
using System.Text;
using System.Text.Json;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.TransportRate.Detail;
using BlazorSvt.Modules.TransportRate.List;
using BlazorSvt.Platform.Domain.IdsEnum;
using BlazorSvt.Platform.Grid.Models;
using FluentAssertions;
using ModelContextProtocol.Client;
using ModelContextProtocol.Protocol;
using NSubstitute;

namespace BlazorSvt.IntegrationTests.Api;

/// <summary>MCP-надстройка <c>/api/mcp</c>: доступ, флаг, инструменты через клиент SDK. Без БД.</summary>
[Trait("Category", "Integration")]
public sealed class McpHttpTests : IClassFixture<McpTestFactory>, IClassFixture<ApiTestFactory>
{
    private const string McpUrl = "/api/mcp";
    private const string PingBody = """{ "jsonrpc": "2.0", "id": 1, "method": "ping" }""";

    private readonly McpTestFactory factory;
    private readonly ApiTestFactory disabledFactory;

    public McpHttpTests(McpTestFactory factory, ApiTestFactory disabledFactory)
    {
        this.factory = factory;
        this.disabledFactory = disabledFactory;
        factory.TransportRates.ClearReceivedCalls();
    }

    [Fact]
    public async Task Disabled_TrustedClient_Returns404McpDisabled()
    {
        var response = await disabledFactory.CreateApiClient().PostAsync(McpUrl, Json(PingBody));

        await ShouldBeProblem(response, HttpStatusCode.NotFound, "mcp_disabled");
    }

    [Fact]
    public async Task Disabled_UntrustedClient_Returns403()
    {
        var response = await disabledFactory.CreateApiClient(caller: @"TEST\someone").PostAsync(McpUrl, Json(PingBody));

        await ShouldBeProblem(response, HttpStatusCode.Forbidden, "client_not_trusted");
    }

    [Theory]
    [InlineData(@"TEST\someone", ApiTestFactory.RegisteredUser, HttpStatusCode.Forbidden, "client_not_trusted")]
    [InlineData(ApiTestFactory.TrustedClient, null, HttpStatusCode.BadRequest, "on_behalf_of_required")]
    [InlineData(ApiTestFactory.TrustedClient, "ivanov", HttpStatusCode.BadRequest, "on_behalf_of_invalid")]
    [InlineData(ApiTestFactory.TrustedClient, ApiTestFactory.UserWithoutRoles, HttpStatusCode.Forbidden, "user_has_no_roles")]
    public async Task Enabled_AccessChecks_SameProblemsAsApi(string caller, string? onBehalfOf, HttpStatusCode status, string code)
    {
        var response = await factory.CreateApiClient(caller, onBehalfOf).PostAsync(McpUrl, Json(PingBody));

        await ShouldBeProblem(response, status, code);
    }

    [Fact]
    public async Task Enabled_Batch_Returns400InvalidRequest()
    {
        var response = await factory.CreateApiClient().PostAsync(McpUrl, Json($"[{PingBody}]"));

        await ShouldBeProblem(response, HttpStatusCode.BadRequest, "invalid_request");
    }

    [Fact]
    public async Task ToolsList_ExactlyFiveReadOnlyTools_NoProfileInLookup()
    {
        await using var client = await ConnectAsync();

        var tools = await client.ListToolsAsync();

        tools.Select(t => t.Name).Should().BeEquivalentTo(
            "list_catalogs", "get_catalog_schema", "search_catalog", "get_catalog_item", "smart_rate_lookup");
        tools.Should().OnlyContain(t => t.ProtocolTool.Annotations!.ReadOnlyHint == true);
        var lookup = tools.Single(t => t.Name == "smart_rate_lookup").ProtocolTool.InputSchema;
        lookup.GetProperty("properties").TryGetProperty("profile", out _).Should().BeFalse();
        client.ServerInstructions.Should().Contain("list_catalogs").And.Contain("25");
    }

    [Theory]
    [InlineData("2025-06-18")]
    [InlineData("2025-11-25")]
    public async Task InitializeHandshakeClient_ServedStatelessly(string protocolVersion)
    {
        await using var client = await ConnectAsync(protocolVersion);

        var tools = await client.ListToolsAsync();
        var result = await client.CallToolAsync("list_catalogs", new Dictionary<string, object?>());

        tools.Should().HaveCount(5);
        Structured(result).GetProperty("items").GetArrayLength().Should().BeGreaterThan(0);
    }

    [Fact]
    public async Task ListCatalogs_SameCatalogs_WithoutExport()
    {
        await using var client = await ConnectAsync();

        var result = await client.CallToolAsync("list_catalogs", new Dictionary<string, object?>());

        var items = Structured(result).GetProperty("items").EnumerateArray().ToList();
        items.Select(i => i.GetProperty("code").GetString()).Should().Contain(["transport-rates", "locations-nodes"]);
        items.SelectMany(i => i.GetProperty("capabilities").EnumerateArray().Select(c => c.GetString()))
            .Should().NotContain("export");
    }

    [Fact]
    public async Task SearchCatalog_DefaultPage25_FieldsKeepKey()
    {
        factory.TransportRates.GetPageAsync(Arg.Any<GridQuery>(), Arg.Any<CancellationToken>())
            .Returns(new GridPage<TransportRateDto>([Rate()], 1));

        await using var client = await ConnectAsync();
        var result = await client.CallToolAsync("search_catalog", new Dictionary<string, object?>
        {
            ["catalog"] = "transport-rates",
            ["filters"] = JsonSerializer.SerializeToElement(new[] { new { field = "currencyId", @operator = "eq", value = "RUB" } }),
            ["fields"] = new[] { "nodeFromCode" }
        });

        var root = Structured(result);
        root.GetProperty("totalCount").GetInt32().Should().Be(1);
        var item = root.GetProperty("items")[0];
        item.EnumerateObject().Select(p => p.Name).Should().Equal("transportRateId", "nodeFromCode");
        item.GetProperty("nodeFromCode").GetString().Should().Be("A1");
        ((TextContentBlock)result.Content.Single()).Text.Should().Be(root.GetRawText());

        await factory.TransportRates.Received(1).GetPageAsync(
            Arg.Is<GridQuery>(q => q.PageSize == 25 && q.Filters.Any(f => f.PropertyName == "CurrencyId" && f.Value == "32694")),
            Arg.Any<CancellationToken>());
    }

    [Fact]
    public async Task SearchCatalog_PageSizeOverLimitAndUnknownField_AllErrorsInOneResult()
    {
        await using var client = await ConnectAsync();

        var result = await client.CallToolAsync("search_catalog", new Dictionary<string, object?>
        {
            ["catalog"] = "transport-rates",
            ["pageSize"] = 26,
            ["fields"] = new[] { "nope" }
        });

        using var error = ToolError(result, "validation_failed");
        var errors = error.RootElement.GetProperty("errors");
        errors.GetProperty("pageSize")[0].GetString().Should().Be("out_of_range");
        errors.GetProperty("fields[0]")[0].GetString().Should().Be("unknown_field");
        await factory.TransportRates.DidNotReceiveWithAnyArgs().GetPageAsync(default!, default);
    }

    [Fact]
    public async Task SearchCatalog_UnknownArgument_ValidationFailed()
    {
        await using var client = await ConnectAsync();

        var result = await client.CallToolAsync("search_catalog", new Dictionary<string, object?>
        {
            ["catalog"] = "transport-rates",
            ["limit"] = 5
        });

        using var error = ToolError(result, "validation_failed");
        error.RootElement.GetProperty("errors").GetProperty("limit")[0].GetString().Should().Be("unknown_field");
    }

    [Fact]
    public async Task GetCatalogItem_UnknownCatalogAndMissingItem_ApiCodes()
    {
        factory.TransportRates.FindDetailAsync(Arg.Any<object>(), Arg.Any<CancellationToken>())
            .Returns((TransportRateDetailDto?)null);
        await using var client = await ConnectAsync();

        var noCatalog = await client.CallToolAsync("get_catalog_item", new Dictionary<string, object?> { ["catalog"] = "unknown", ["id"] = 1 });
        var noItem = await client.CallToolAsync("get_catalog_item", new Dictionary<string, object?> { ["catalog"] = "transport-rates", ["id"] = 42 });

        using var catalogError = ToolError(noCatalog, "catalog_not_found");
        using var itemError = ToolError(noItem, "item_not_found");
        itemError.RootElement.GetProperty("status").GetInt32().Should().Be(404);
    }

    [Fact]
    public async Task SmartRateLookup_SameServiceAsHttp_RunsWithOnBehalfUserRights()
    {
        SetupLookupReferences();
        factory.SmartLookup.GetCandidatesAsync(default!, default, default!, default).ReturnsForAnyArgs([]);
        await using var client = await ConnectAsync();

        var result = await client.CallToolAsync("smart_rate_lookup", LookupArguments());

        // Сервис подбора сам проверяет IAccessGuard: успех значит, что инструмент видит пользователя из X-On-Behalf-Of.
        var root = Structured(result);
        root.GetProperty("found").GetBoolean().Should().BeFalse();
        root.GetProperty("request").GetProperty("profile").GetString().Should().Be("Импорт в CRM");
    }

    [Fact]
    public async Task SmartRateLookup_NoExchangeRate_CurrencyRateMissing()
    {
        SetupLookupReferences();
        factory.SmartLookup.GetCandidatesAsync(default!, default, default!, default).ReturnsForAnyArgs([UsdCandidate()]);
        factory.SmartLookup.ConvertCurrencyAsync(default, default, default, default, default).ReturnsForAnyArgs((decimal?)null);
        await using var client = await ConnectAsync();

        var result = await client.CallToolAsync("smart_rate_lookup", LookupArguments());

        using var error = ToolError(result, "currency_rate_missing");
        error.RootElement.GetProperty("status").GetInt32().Should().Be(422);
    }

    [Fact]
    public async Task SmartRateLookup_ProfileArgument_Rejected()
    {
        await using var client = await ConnectAsync();
        var arguments = LookupArguments();
        arguments["profile"] = "Импорт в CRM";

        var result = await client.CallToolAsync("smart_rate_lookup", arguments);

        using var error = ToolError(result, "validation_failed");
        error.RootElement.GetProperty("errors").GetProperty("profile")[0].GetString().Should().Be("unknown_field");
    }

    private async Task<McpClient> ConnectAsync(string? protocolVersion = null)
    {
        var http = factory.CreateApiClient();
        var transport = new HttpClientTransport(
            new HttpClientTransportOptions
            {
                Endpoint = new Uri(http.BaseAddress!, McpUrl),
                TransportMode = HttpTransportMode.StreamableHttp
            },
            http,
            loggerFactory: null,
            ownsHttpClient: true);
        var options = protocolVersion is null ? null : new McpClientOptions { ProtocolVersion = protocolVersion };
        return await McpClient.CreateAsync(transport, options, loggerFactory: null, CancellationToken.None);
    }

    private static JsonElement Structured(CallToolResult result)
    {
        result.IsError.Should().NotBe(true, ((TextContentBlock)result.Content.Single()).Text);
        return result.StructuredContent!.Value;
    }

    private static JsonDocument ToolError(CallToolResult result, string code)
    {
        result.IsError.Should().BeTrue();
        result.StructuredContent.Should().BeNull();
        var json = JsonDocument.Parse(((TextContentBlock)result.Content.Single()).Text);
        json.RootElement.GetProperty("code").GetString().Should().Be(code);
        json.RootElement.GetProperty("message").GetString().Should().NotBeNullOrEmpty();
        json.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
        return json;
    }

    private static Dictionary<string, object?> LookupArguments() =>
        new()
        {
            ["nodeFromCode"] = "A1",
            ["nodeToCode"] = "B1",
            ["productCode"] = "P1",
            ["transportTypeCode"] = "Auto_20",
            ["currencyCode"] = "RUB",
            ["date"] = "2026-10-20"
        };

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
