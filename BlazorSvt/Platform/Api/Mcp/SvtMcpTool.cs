using System.Text.Json;
using System.Text.Json.Nodes;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Infrastructure;
using ModelContextProtocol.Protocol;
using ModelContextProtocol.Server;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary>
/// Инструмент MCP поверх API СВТ: только чтение, схемы входа и выхода заданы явно, ответ — JSON-контракт <see cref="ApiJson"/>.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Сервисы — из области HTTP-запроса (<see cref="HttpContext.RequestServices"/>): там <c>ApiAccessMiddleware</c>
/// уже определил пользователя из <c>X-On-Behalf-Of</c>. Область, которую создаёт SDK, для этого не годится.</item>
/// <item>Успех — <c>structuredContent</c> и тот же JSON текстом (для клиентов без structured output).</item>
/// <item>Ошибка — <c>isError</c> и JSON <c>{ code, message, status, traceId, errors? }</c> только текстом:
/// клиент, сверяющий <c>structuredContent</c> с <c>outputSchema</c>, не споткнётся об ошибку.</item>
/// </list>
/// Регистрация — <see cref="McpModule.AddMcpTool{TTool}"/>.
/// </remarks>
public abstract class SvtMcpTool : McpServerTool
{
    private readonly IHttpContextAccessor httpContextAccessor;
    private readonly Tool tool;
    private readonly IReadOnlyCollection<string> argumentNames;

    /// <param name="inputSchema">JSON Schema аргументов; <c>additionalProperties: false</c>. Имена свойств — допустимые аргументы.</param>
    /// <param name="outputSchema">JSON Schema успешного результата (объект).</param>
    protected SvtMcpTool(
        IHttpContextAccessor httpContextAccessor,
        string name,
        string title,
        string description,
        string inputSchema,
        string outputSchema)
    {
        this.httpContextAccessor = httpContextAccessor;
        var input = JsonDocument.Parse(inputSchema).RootElement.Clone();
        argumentNames = input.TryGetProperty("properties", out var properties)
            ? properties.EnumerateObject().Select(p => p.Name).ToHashSet(StringComparer.Ordinal)
            : [];

        tool = new Tool
        {
            Name = name,
            Title = title,
            Description = description,
            InputSchema = input,
            OutputSchema = JsonDocument.Parse(outputSchema).RootElement.Clone(),
            Annotations = new ToolAnnotations
            {
                Title = title,
                ReadOnlyHint = true,
                DestructiveHint = false,
                IdempotentHint = true,
                OpenWorldHint = false
            }
        };
    }

    public string Name => tool.Name;

    public override Tool ProtocolTool => tool;

    public override IReadOnlyList<object> Metadata => [];

    public sealed override async ValueTask<CallToolResult> InvokeAsync(
        RequestContext<CallToolRequestParams> request,
        CancellationToken cancellationToken = default)
    {
        var http = httpContextAccessor.HttpContext
            ?? throw new InvalidOperationException("MCP tool is invoked outside of an HTTP request.");
        ApiCallLog.SetOperation(http, Name);

        try
        {
            var arguments = new McpArguments(request.Params?.Arguments, argumentNames);
            var result = await ExecuteAsync(new McpToolContext(http, arguments), cancellationToken);
            return Success(http, result);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            throw;
        }
        catch (ApiException ex)
        {
            return Error(http, ex.StatusCode, ex.Code, ex.Errors, ex.MessageArgs);
        }
        catch (AccessDeniedException)
        {
            return Error(http, StatusCodes.Status403Forbidden, ApiErrorCodes.AccessDenied);
        }
        catch (DirectoryUnavailableException)
        {
            return Error(http, StatusCodes.Status503ServiceUnavailable, ApiErrorCodes.ServiceUnavailable);
        }
        catch (Exception ex)
        {
            http.RequestServices.GetRequiredService<ILogger<SvtMcpTool>>()
                .LogError(ex, "Unhandled MCP tool error in {McpTool}", Name);
            return Error(http, StatusCodes.Status500InternalServerError, ApiErrorCodes.InternalError);
        }
    }

    /// <summary>Работа инструмента. Ожидаемые отказы — <see cref="ApiException"/>, как в REST.</summary>
    /// <returns>Объект ответа; сериализуется <see cref="ApiJson"/>.</returns>
    protected abstract Task<object> ExecuteAsync(McpToolContext context, CancellationToken cancellationToken);

    private static CallToolResult Success(HttpContext http, object result)
    {
        var json = JsonSerializer.SerializeToElement(result, ApiJson.Options(http));
        return new CallToolResult
        {
            StructuredContent = json,
            Content = [new TextContentBlock { Text = json.GetRawText() }]
        };
    }

    private static CallToolResult Error(
        HttpContext http,
        int status,
        string code,
        IReadOnlyDictionary<string, string[]>? errors = null,
        params object[] messageArgs)
    {
        ApiCallLog.SetErrorCode(http, code);

        var body = new JsonObject
        {
            ["code"] = code,
            ["message"] = ApiProblems.Message(http, code, messageArgs),
            ["status"] = status,
            ["traceId"] = ApiProblems.TraceId(http)
        };
        if (errors is { Count: > 0 })
        {
            body["errors"] = JsonSerializer.SerializeToNode(errors);
        }

        return new CallToolResult
        {
            IsError = true,
            Content = [new TextContentBlock { Text = body.ToJsonString() }]
        };
    }
}

/// <summary>Вызов инструмента: HTTP-запрос, его сервисы и проверенные аргументы.</summary>
public sealed class McpToolContext(HttpContext httpContext, McpArguments arguments)
{
    public HttpContext HttpContext => httpContext;

    public McpArguments Arguments => arguments;

    public JsonSerializerOptions JsonOptions => ApiJson.Options(httpContext);

    public T GetRequiredService<T>()
        where T : notnull =>
        httpContext.RequestServices.GetRequiredService<T>();
}
