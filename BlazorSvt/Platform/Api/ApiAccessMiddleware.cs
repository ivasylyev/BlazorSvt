using System.Diagnostics;
using System.Text.RegularExpressions;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Api.Mcp;
using BlazorSvt.Platform.Infrastructure;
using Microsoft.Extensions.Options;
using Microsoft.Net.Http.Headers;
using Serilog;
using Serilog.Context;

namespace BlazorSvt.Platform.Api;

/// <summary>
/// Вход в <c>/api</c>: доверенный клиент → пользователь «от имени» → права → обработка ошибок.
/// </summary>
/// <remarks>
/// <list type="bullet">
/// <item>Вызывающий (Windows-учётка) должен быть в <see cref="ApiOptions.TrustedClients"/>, иначе 403 на весь <c>/api</c>.
/// Браузер пользователя не в списке — это и защита от CSRF.</item>
/// <item><c>/api/v1/*</c> и <c>/api/mcp</c>: обязателен <c>X-On-Behalf-Of: DOMAIN\user</c>; права — только роли этого пользователя
/// (<c>dbo.User</c>/<c>dbo.UserRole</c>), не служебной учётки.</item>
/// <item><c>/api/mcp</c> при выключенном <see cref="ApiOptions.McpEnabled"/> — 404 <c>mcp_disabled</c> (после проверки клиента).
/// Пакет JSON-RPC (массив) — 400 <c>invalid_request</c> до SDK.</item>
/// <item>Лог: <c>UserLogin</c> = конечный пользователь, <c>ApiClient</c> = служебная учётка, <c>Channel</c> = Api или Mcp.</item>
/// <item>Development + <c>Auth:IgnoreAccessControl</c>: проверка доверенного клиента и ролей пропускается.</item>
/// </list>
/// </remarks>
public sealed partial class ApiAccessMiddleware(
    RequestDelegate next,
    IOptionsMonitor<ApiOptions> options,
    IHostApplicationLifetime lifetime,
    ILogger<ApiAccessMiddleware> logger)
{
    public const string OnBehalfOfHeader = "X-On-Behalf-Of";
    public const string SunsetValue = "Fri, 01 Jan 2099 00:00:00 GMT";

    [GeneratedRegex(@"^[^\\/\s]+\\[^\\/\s]+$")]
    private static partial Regex LoginFormat();

    public async Task InvokeAsync(
        HttpContext context,
        UserAccessSynchronizer synchronizer,
        CurrentUser currentUser,
        IDiagnosticContext diagnostics,
        McpEndpointState mcp)
    {
        context.Response.OnStarting(() =>
        {
            var headers = context.Response.Headers;
            if (!headers.ContainsKey(HeaderNames.CacheControl))
            {
                headers[HeaderNames.CacheControl] = "no-store";
            }

            headers["Sunset"] = SunsetValue;
            return Task.CompletedTask;
        });

        if (lifetime.ApplicationStopping.IsCancellationRequested)
        {
            context.Response.Headers[HeaderNames.RetryAfter] = "30";
            await ApiProblems.WriteAsync(context, StatusCodes.Status503ServiceUnavailable, ApiErrorCodes.ServiceUnavailable);
            return;
        }

        var client = context.User.Identity?.IsAuthenticated == true ? context.User.Identity.Name : null;
        diagnostics.Set("ApiClient", client);

        using (LogContext.PushProperty("ApiClient", client))
        {
            if (!synchronizer.IsBypassActive)
            {
                if (string.IsNullOrWhiteSpace(client))
                {
                    await ApiProblems.WriteAsync(context, StatusCodes.Status401Unauthorized, ApiErrorCodes.Unauthenticated);
                    return;
                }

                if (!IsTrusted(client))
                {
                    logger.LogWarning("API call from untrusted client {ApiClient}", client);
                    await ApiProblems.WriteAsync(context, StatusCodes.Status403Forbidden, ApiErrorCodes.ClientNotTrusted);
                    return;
                }
            }

            var isMcp = McpModule.IsMcp(context.Request.Path);
            if (isMcp && !mcp.Enabled)
            {
                await ApiProblems.WriteAsync(context, StatusCodes.Status404NotFound, ApiErrorCodes.McpDisabled);
                return;
            }

            if (!isMcp && !context.Request.Path.StartsWithSegments(ApiModule.V1Prefix, StringComparison.OrdinalIgnoreCase))
            {
                await RunAsync(context);
                return;
            }

            var onBehalfOf = context.Request.Headers[OnBehalfOfHeader].ToString().Trim();
            if (string.IsNullOrEmpty(onBehalfOf))
            {
                await ApiProblems.WriteAsync(context, StatusCodes.Status400BadRequest, ApiErrorCodes.OnBehalfOfRequired);
                return;
            }

            if (!LoginFormat().IsMatch(onBehalfOf))
            {
                await ApiProblems.WriteAsync(context, StatusCodes.Status400BadRequest, ApiErrorCodes.OnBehalfOfInvalid);
                return;
            }

            diagnostics.Set("UserLogin", onBehalfOf);
            using (LogContext.PushProperty("UserLogin", onBehalfOf))
            {
                var started = Stopwatch.GetTimestamp();
                try
                {
                    if (isMcp && !await AcceptMcpMessageAsync(context))
                    {
                        return;
                    }

                    await synchronizer.SynchronizeOnBehalfAsync(onBehalfOf, context.RequestAborted);

                    if (currentUser.State == AccessState.Denied)
                    {
                        var code = currentUser.DenialReason == AccessDenialReasons.NoUser
                            ? ApiErrorCodes.UserNotRegistered
                            : ApiErrorCodes.UserHasNoRoles;
                        await ApiProblems.WriteAsync(context, StatusCodes.Status403Forbidden, code);
                        return;
                    }

                    await RunAsync(context);
                }
                finally
                {
                    // Файловый шаблон лога выводит только текст сообщения — ключевые поля вызова держим в нём.
                    // MCP отдаёт ошибку инструмента с HTTP 200 — исход различает только код.
                    logger.LogInformation(
                        "{ApiChannel} {ApiOperation} {Catalog} for {UserLogin} via {ApiClient}: {StatusCode} {ApiErrorCode}, rows {ApiRowCount}, {ElapsedMs} ms",
                        isMcp ? "MCP" : "API",
                        ApiCallLog.Operation(context) ?? context.Request.Path.Value,
                        ApiCallLog.Catalog(context),
                        onBehalfOf,
                        client,
                        context.Response.StatusCode,
                        ApiCallLog.ErrorCode(context) ?? "-",
                        ApiCallLog.RowCount(context),
                        (long)Stopwatch.GetElapsedTime(started).TotalMilliseconds);
                }
            }
        }
    }

    /// <summary>Пакет JSON-RPC отклоняется: журнал и <see cref="ApiCallLog"/> рассчитаны на один вызов на запрос.</summary>
    private static async Task<bool> AcceptMcpMessageAsync(HttpContext context)
    {
        if (!HttpMethods.IsPost(context.Request.Method))
        {
            ApiCallLog.SetOperation(context, McpRequestInspector.OperationFor(null));
            return true;
        }

        var message = await McpRequestInspector.InspectAsync(context.Request, context.RequestAborted);
        if (message.IsBatch)
        {
            ApiCallLog.SetOperation(context, "mcp_batch");
            await ApiProblems.WriteAsync(context, StatusCodes.Status400BadRequest, ApiErrorCodes.InvalidRequest);
            return false;
        }

        ApiCallLog.SetOperation(context, McpRequestInspector.OperationFor(message.Method));
        return true;
    }

    private bool IsTrusted(string client) =>
        options.CurrentValue.TrustedClients.Any(c => string.Equals(c?.Trim(), client, StringComparison.OrdinalIgnoreCase));

    private async Task RunAsync(HttpContext context)
    {
        try
        {
            await next(context);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, context.RequestAborted))
        {
            logger.LogInformation("API request aborted by client");
            if (!context.Response.HasStarted)
            {
                context.Response.StatusCode = 499;
            }
        }
        catch (ApiException ex) when (!context.Response.HasStarted)
        {
            await ApiProblems.WriteAsync(context, ex);
        }
        catch (AccessDeniedException) when (!context.Response.HasStarted)
        {
            await ApiProblems.WriteAsync(context, StatusCodes.Status403Forbidden, ApiErrorCodes.AccessDenied);
        }
        catch (DirectoryUnavailableException) when (!context.Response.HasStarted)
        {
            await ApiProblems.WriteAsync(context, StatusCodes.Status503ServiceUnavailable, ApiErrorCodes.ServiceUnavailable);
        }
        catch (BadHttpRequestException ex) when (!context.Response.HasStarted)
        {
            logger.LogInformation(ex, "Bad API request");
            await ApiProblems.WriteAsync(context, ex.StatusCode, ApiErrorCodes.InvalidRequest);
        }
        catch (Exception ex) when (!context.Response.HasStarted)
        {
            logger.LogError(ex, "Unhandled API error");
            await ApiProblems.WriteAsync(context, StatusCodes.Status500InternalServerError, ApiErrorCodes.InternalError);
        }
    }
}
