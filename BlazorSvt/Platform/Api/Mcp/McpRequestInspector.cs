using System.Text.Json;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary>
/// Что пришло в POST <c>/api/mcp</c> до передачи в SDK: пакет JSON-RPC (запрещён) и метод (для журнала).
/// Тело читается с буферизацией и возвращается SDK нетронутым.
/// </summary>
public static class McpRequestInspector
{
    public sealed record Result(bool IsBatch, string? Method);

    public static async Task<Result> InspectAsync(HttpRequest request, CancellationToken cancellationToken)
    {
        request.EnableBuffering();
        try
        {
            using var document = await JsonDocument.ParseAsync(request.Body, cancellationToken: cancellationToken);
            var root = document.RootElement;
            return root.ValueKind switch
            {
                JsonValueKind.Array => new Result(true, null),
                JsonValueKind.Object when root.TryGetProperty("method", out var method)
                                         && method.ValueKind == JsonValueKind.String
                    => new Result(false, method.GetString()),
                _ => new Result(false, null)
            };
        }
        catch (JsonException)
        {
            // Битый JSON разбирает и отклоняет SDK (ошибка JSON-RPC Parse error).
            return new Result(false, null);
        }
        finally
        {
            request.Body.Position = 0;
        }
    }

    /// <summary>
    /// Операция в журнале для служебных методов протокола. Вызов инструмента потом
    /// перезаписывает её именем инструмента (<c>search_catalog</c> и т.п.).
    /// </summary>
    public static string OperationFor(string? method) =>
        method switch
        {
            "initialize" => "mcp_initialize",
            "server/discover" => "mcp_discover",
            "tools/list" => "mcp_tools_list",
            "tools/call" => "mcp_tools_call",
            "ping" => "mcp_ping",
            not null when method.StartsWith("notifications/", StringComparison.Ordinal) => "mcp_notification",
            _ => "mcp_other"
        };
}
