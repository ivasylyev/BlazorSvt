using System.Security.Cryptography;
using System.Text.Json;
using Microsoft.Net.Http.Headers;

namespace BlazorSvt.Platform.Api;

/// <summary>Общие HTTP-приёмы API: ETag для редко меняющихся ответов.</summary>
public static class ApiHttp
{
    /// <summary>
    /// JSON с ETag: при совпадении <c>If-None-Match</c> — 304 без тела.
    /// <c>Cache-Control: no-cache</c> — клиент кэширует, но перепроверяет.
    /// </summary>
    public static IResult JsonWithETag<T>(HttpContext context, T value)
    {
        var options = ApiJson.Options(context);
        var bytes = JsonSerializer.SerializeToUtf8Bytes(value, options);
        var etag = $"\"{Convert.ToHexString(SHA256.HashData(bytes))[..32]}\"";

        context.Response.Headers[HeaderNames.ETag] = etag;
        context.Response.Headers[HeaderNames.CacheControl] = "no-cache";

        var ifNoneMatch = context.Request.Headers[HeaderNames.IfNoneMatch].ToString();
        if (!string.IsNullOrEmpty(ifNoneMatch)
            && ifNoneMatch.Split(',').Any(t => t.Trim() == etag || t.Trim() == "*"))
        {
            return Results.StatusCode(StatusCodes.Status304NotModified);
        }

        return Results.Bytes(bytes, "application/json; charset=utf-8");
    }
}
