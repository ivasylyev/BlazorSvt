namespace BlazorSvt.Platform.Api;

/// <summary>
/// Ожидаемая ошибка API: превращается в ProblemDetails со стабильным <see cref="Code"/>.
/// </summary>
/// <param name="statusCode">HTTP-статус ответа.</param>
/// <param name="code">Код из <see cref="ApiErrorCodes"/> (или модульный).</param>
/// <param name="errors">Ошибки по полям запроса: путь поля → коды/тексты.</param>
public sealed class ApiException(
    int statusCode,
    string code,
    IReadOnlyDictionary<string, string[]>? errors = null,
    params object[] messageArgs)
    : Exception(code)
{
    public int StatusCode { get; } = statusCode;

    public string Code { get; } = code;

    public IReadOnlyDictionary<string, string[]>? Errors { get; } = errors;

    /// <summary>Аргументы для форматирования локализованного текста <c>Api.Error.{code}</c>.</summary>
    public object[] MessageArgs { get; } = messageArgs;

    public static ApiException Validation(IReadOnlyDictionary<string, string[]> errors) =>
        new(StatusCodes.Status400BadRequest, ApiErrorCodes.ValidationFailed, errors);

    public static ApiException NotFound(string code, params object[] messageArgs) =>
        new(StatusCodes.Status404NotFound, code, null, messageArgs);
}
