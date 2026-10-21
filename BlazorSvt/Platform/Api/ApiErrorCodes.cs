namespace BlazorSvt.Platform.Api;

/// <summary>
/// Стабильные коды ошибок API (поле <c>code</c> в ProblemDetails). Часть контракта:
/// клиент ветвится по коду, а не по тексту. Тексты — <c>Api.Error.{code}</c> в Platform.resx.
/// </summary>
public static class ApiErrorCodes
{
    public const string InvalidRequest = "invalid_request";
    public const string ValidationFailed = "validation_failed";
    public const string Unauthenticated = "unauthenticated";
    public const string ClientNotTrusted = "client_not_trusted";
    public const string OnBehalfOfRequired = "on_behalf_of_required";
    public const string OnBehalfOfInvalid = "on_behalf_of_invalid";
    public const string UserNotRegistered = "user_not_registered";
    public const string UserHasNoRoles = "user_has_no_roles";
    public const string AccessDenied = "access_denied";
    public const string NotFound = "not_found";
    public const string CatalogNotFound = "catalog_not_found";
    public const string ItemNotFound = "item_not_found";
    public const string CurrencyRateMissing = "currency_rate_missing";
    public const string ServiceUnavailable = "service_unavailable";
    public const string InternalError = "internal_error";
}
