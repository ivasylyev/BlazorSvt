using System.Text.Json;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Api.Mcp;
using FluentAssertions;
using Microsoft.AspNetCore.Http;
using Microsoft.Extensions.DependencyInjection;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;
using NSubstitute;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace BlazorSvt.UnitTests.Platform.Api;

[Trait("Category", "Unit")]
public class McpToolTests
{
    private static readonly string[] Available = ["transportRateId", "nodeFromCode", "nodeToCode", "currencyId"];

    [Fact]
    public void Fields_KeepsListedInDtoOrder_AndAlwaysKey()
    {
        var errors = new Dictionary<string, string[]>();

        var selected = McpFieldSelection.Resolve(["CURRENCYID", "nodeFromCode", "currencyId"], Available, "transportRateId", "fields", errors);

        errors.Should().BeEmpty();
        selected.Should().Equal("transportRateId", "nodeFromCode", "currencyId");
    }

    [Fact]
    public void Fields_NotGiven_NoSelection()
    {
        McpFieldSelection.Resolve(null, Available, "transportRateId", "fields", new Dictionary<string, string[]>())
            .Should().BeNull();
    }

    [Fact]
    public void Fields_UnknownName_ErrorByIndex()
    {
        var errors = new Dictionary<string, string[]>();

        McpFieldSelection.Resolve(["nodeFromCode", "nope"], Available, "transportRateId", "fields", errors).Should().BeNull();

        errors.Should().ContainKey("fields[1]").WhoseValue.Should().Equal(CatalogValidationCodes.UnknownField);
    }

    [Fact]
    public void Fields_EmptyList_Required()
    {
        var errors = new Dictionary<string, string[]>();

        McpFieldSelection.Resolve([], Available, "transportRateId", "fields", errors);

        errors.Should().ContainKey("fields").WhoseValue.Should().Equal(CatalogValidationCodes.Required);
    }

    [Fact]
    public void Project_KeepsOnlySelectedProperties_WithNulls()
    {
        var item = JsonSerializer.SerializeToElement(new { transportRateId = 7, nodeFromCode = (string?)null, nodeToCode = "B1" });

        var projected = McpFieldSelection.Project(item, ["transportRateId", "nodeFromCode"]);

        projected.ToJsonString().Should().Be("""{"transportRateId":7,"nodeFromCode":null}""");
    }

    [Fact]
    public void Arguments_UnknownMissingAndInvalid_CollectedTogether()
    {
        var args = new McpArguments(
            Args("""{ "pageSize": "abc", "profile": "x", "page": "3" }"""),
            ["catalog", "pageSize", "page"]);

        args.GetString("catalog", required: true);
        args.GetInt32("pageSize");
        args.GetInt32("page").Should().Be(3);

        args.Errors.Should().BeEquivalentTo(new Dictionary<string, string[]>
        {
            ["catalog"] = [CatalogValidationCodes.Required],
            ["pageSize"] = [CatalogValidationCodes.InvalidValue],
            ["profile"] = [CatalogValidationCodes.UnknownField]
        });
    }

    [Fact]
    public async Task Tool_ApiException_BecomesErrorResultWithSameCode()
    {
        var tool = new TestTool(Accessor(), _ => throw ApiException.Validation(
            new Dictionary<string, string[]> { ["pageSize"] = [CatalogValidationCodes.OutOfRange] }));

        var result = await tool.InvokeAsync(Request(), CancellationToken.None);

        result.IsError.Should().BeTrue();
        result.StructuredContent.Should().BeNull();
        using var body = ErrorBody(result);
        body.RootElement.GetProperty("code").GetString().Should().Be(ApiErrorCodes.ValidationFailed);
        body.RootElement.GetProperty("status").GetInt32().Should().Be(400);
        body.RootElement.GetProperty("traceId").GetString().Should().NotBeNullOrEmpty();
        body.RootElement.GetProperty("errors").GetProperty("pageSize")[0].GetString().Should().Be(CatalogValidationCodes.OutOfRange);
    }

    [Fact]
    public async Task Tool_AccessDenied_BecomesAccessDenied()
    {
        var tool = new TestTool(Accessor(), _ => throw new AccessDeniedException());

        var result = await tool.InvokeAsync(Request(), CancellationToken.None);

        using var body = ErrorBody(result);
        body.RootElement.GetProperty("code").GetString().Should().Be(ApiErrorCodes.AccessDenied);
        body.RootElement.GetProperty("status").GetInt32().Should().Be(403);
    }

    [Fact]
    public async Task Tool_UnexpectedException_InternalErrorWithoutDetails()
    {
        var tool = new TestTool(Accessor(), _ => throw new InvalidOperationException("secret connection string"));

        var result = await tool.InvokeAsync(Request(), CancellationToken.None);

        result.IsError.Should().BeTrue();
        var text = ((TextContentBlock)result.Content.Single()).Text;
        text.Should().Contain(ApiErrorCodes.InternalError).And.NotContain("secret");
    }

    [Fact]
    public async Task Tool_Success_StructuredContentAndSameTextInApiJson()
    {
        var tool = new TestTool(Accessor(), _ => new { Date = new DateOnly(2026, 10, 20), Missing = (string?)null });

        var result = await tool.InvokeAsync(Request(), CancellationToken.None);

        result.IsError.Should().NotBe(true);
        var text = ((TextContentBlock)result.Content.Single()).Text;
        text.Should().Be("""{"date":"2026-10-20","missing":null}""");
        result.StructuredContent!.Value.GetRawText().Should().Be(text);
    }

    [Fact]
    public void Tool_DeclaresReadOnlyAnnotations()
    {
        var annotations = new TestTool(Accessor(), _ => new { }).ProtocolTool.Annotations!;

        annotations.ReadOnlyHint.Should().BeTrue();
        annotations.DestructiveHint.Should().BeFalse();
        annotations.IdempotentHint.Should().BeTrue();
        annotations.OpenWorldHint.Should().BeFalse();
    }

    [Theory]
    [InlineData("initialize", "mcp_initialize")]
    [InlineData("server/discover", "mcp_discover")]
    [InlineData("tools/list", "mcp_tools_list")]
    [InlineData("tools/call", "mcp_tools_call")]
    [InlineData("ping", "mcp_ping")]
    [InlineData("notifications/initialized", "mcp_notification")]
    [InlineData("resources/list", "mcp_other")]
    [InlineData(null, "mcp_other")]
    public void Inspector_ProtocolMethod_MapsToLogOperation(string? method, string operation)
    {
        McpRequestInspector.OperationFor(method).Should().Be(operation);
    }

    private static IDictionary<string, JsonElement> Args(string json) =>
        JsonSerializer.Deserialize<Dictionary<string, JsonElement>>(json)!;

    private static IHttpContextAccessor Accessor()
    {
        var services = new ServiceCollection()
            .AddLogging()
            .Configure<HttpJsonOptions>(o => ApiJson.Configure(o.SerializerOptions))
            .BuildServiceProvider();
        return new HttpContextAccessor { HttpContext = new DefaultHttpContext { RequestServices = services } };
    }

    private static RequestContext<CallToolRequestParams> Request() =>
        new(Substitute.For<McpServer>(), new JsonRpcRequest { Method = "tools/call" }, new CallToolRequestParams { Name = "test_tool" });

    private static JsonDocument ErrorBody(CallToolResult result) =>
        JsonDocument.Parse(((TextContentBlock)result.Content.Single()).Text);

    private sealed class TestTool(IHttpContextAccessor accessor, Func<McpToolContext, object> body) : SvtMcpTool(
        accessor,
        "test_tool",
        "Test",
        "Test tool",
        """{ "type": "object", "properties": {}, "additionalProperties": false }""",
        """{ "type": "object" }""")
    {
        protected override Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken) =>
            Task.FromResult(body(context));
    }
}
