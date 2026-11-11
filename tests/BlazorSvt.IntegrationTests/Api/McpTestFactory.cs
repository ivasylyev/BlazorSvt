namespace BlazorSvt.IntegrationTests.Api;

/// <summary>Хост СВТ в памяти с включённым <c>Api:McpEnabled</c> (маршрут <c>/api/mcp</c>).</summary>
public sealed class McpTestFactory : ApiTestFactory
{
    protected override bool McpEnabled => true;
}
