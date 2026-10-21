namespace BlazorSvt.Modules.SmartRateLookup.Contracts;

/// <summary>Запрос умного подбора ставки (аналог запроса CRM в <c>dbo.TransportRateCRM</c>).</summary>
public sealed class SmartRateLookupRequest
{
    /// <summary>Код узла отправления.</summary>
    public string? NodeFromCode { get; init; }

    /// <summary>Код узла назначения.</summary>
    public string? NodeToCode { get; init; }

    /// <summary>Код продукта (МТР).</summary>
    public string? ProductCode { get; init; }

    /// <summary>Код типа транспорта 3-го уровня.</summary>
    public string? TransportTypeCode { get; init; }

    /// <summary>Код валюты ответа, напр. <c>RUB</c>.</summary>
    public string? CurrencyCode { get; init; }

    /// <summary>Дата действия ставки и курса пересчёта. По умолчанию — сегодня (время сервера, Москва).</summary>
    public DateOnly? Date { get; init; }

    /// <summary>Профиль приоритетов типов ставок (<c>OperationName</c>). По умолчанию «Импорт в CRM».</summary>
    public string? Profile { get; init; }
}

/// <param name="Found"><c>false</c> — ставка не найдена ни на одном шаге (это не ошибка).</param>
/// <param name="Rate">Найденная ставка; <c>null</c>, если не найдена.</param>
/// <param name="Match">Как найдена ставка; <c>null</c>, если не найдена.</param>
/// <param name="Request">Разрешённые параметры запроса (названия, группа продукта, вид транспорта, дата, профиль).</param>
public sealed record SmartRateLookupResponse(
    bool Found,
    SmartRateLookupRate? Rate,
    SmartRateLookupMatch? Match,
    SmartRateLookupResolvedRequest Request);

/// <param name="AverageRateLevel3Id">Ключ средневзвешенной ставки (справочник <c>average-rates-level3</c>).</param>
/// <param name="Value">Ставка в запрошенной валюте.</param>
/// <param name="CurrencyCode">Запрошенная валюта.</param>
/// <param name="OriginalValue">Ставка в валюте справочника.</param>
/// <param name="OriginalCurrencyCode">Валюта ставки в справочнике.</param>
/// <param name="ConversionRate">Коэффициент пересчёта (<c>value / originalValue</c>); 1 без пересчёта.</param>
/// <param name="ConversionDate">Дата курса; <c>null</c> без пересчёта.</param>
/// <param name="EffectiveLoad">Эффективная загрузка типа транспорта.</param>
/// <param name="RateTypePriority">Приоритет типа ставки в профиле (меньше — важнее).</param>
/// <param name="TransportTypeCode">Фактический тип транспорта ставки (может отличаться от запрошенного при подъёме до вида).</param>
public sealed record SmartRateLookupRate(
    long AverageRateLevel3Id,
    decimal Value,
    string CurrencyCode,
    decimal OriginalValue,
    string OriginalCurrencyCode,
    decimal ConversionRate,
    DateOnly? ConversionDate,
    decimal EffectiveLoad,
    DateOnly StartDate,
    DateOnly EndDate,
    string RateTypeCode,
    string RateTypeName,
    decimal RateTypePriority,
    string TransportTypeCode,
    string TransportTypeName,
    string NodeFromCode,
    string NodeToCode,
    string? ProductCode,
    string? ProductGroupCode);

/// <param name="Level">Уровень поиска 1–8: 1 — точное совпадение, далее всё более общие условия.</param>
/// <param name="ProductLiftedToGroup">Продукт поднят до группы продуктов.</param>
/// <param name="DestinationLiftedToRegion">Узел назначения поднят до региона.</param>
/// <param name="TransportTypeLiftedToKind">Тип транспорта поднят до вида транспорта.</param>
/// <param name="DepartureNodeReplaced">Узел отправления заменён дополнительным узлом организации DPO.</param>
/// <param name="DepartureNodeCode">Узел отправления, по которому найдена ставка.</param>
/// <param name="Description">Пояснение для пользователя (язык — по Accept-Language).</param>
public sealed record SmartRateLookupMatch(
    int Level,
    bool ProductLiftedToGroup,
    bool DestinationLiftedToRegion,
    bool TransportTypeLiftedToKind,
    bool DepartureNodeReplaced,
    string? DepartureNodeCode,
    string Description);

public sealed record SmartRateLookupResolvedRequest(
    string NodeFromCode,
    string? NodeFromName,
    string NodeToCode,
    string? NodeToName,
    string ProductCode,
    string? ProductName,
    string? ProductGroupCode,
    string? ProductGroupName,
    string TransportTypeCode,
    string? TransportTypeName,
    string? TransportKindCode,
    string CurrencyCode,
    DateOnly Date,
    string Profile);
