using System.ComponentModel;
using System.Text.Json;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Условие отбора. Условия объединяются только через И.</summary>
public sealed class CatalogFilterRequest
{
    /// <summary>Имя поля из <c>/schema</c>.</summary>
    public string? Field { get; init; }

    /// <summary><c>contains</c> (текст), <c>eq</c>, <c>ne</c>, <c>lt</c>, <c>le</c>, <c>gt</c>, <c>ge</c> — см. <c>operators</c> поля в <c>/schema</c>.</summary>
    public string? Operator { get; init; }

    /// <summary>Строка, число или boolean. Даты — <c>yyyy-MM-dd</c>, enum — код из <c>/schema</c>.</summary>
    public JsonElement? Value { get; init; }
}

public sealed class CatalogSortRequest
{
    public string? Field { get; init; }

    /// <summary><c>asc</c> (по умолчанию) или <c>desc</c>.</summary>
    public string? Direction { get; init; }
}

/// <summary>Общая часть поиска и выгрузки.</summary>
public interface ICatalogQueryRequest
{
    IReadOnlyList<CatalogFilterRequest>? Filters { get; }

    CatalogSortRequest? Sort { get; }

    DateOnly? AsOf { get; }

    bool IncludeArchived { get; }
}

public sealed class CatalogSearchRequest : ICatalogQueryRequest
{
    public IReadOnlyList<CatalogFilterRequest>? Filters { get; init; }

    public CatalogSortRequest? Sort { get; init; }

    /// <summary>Номер страницы, с 1.</summary>
    [DefaultValue(1)]
    public int Page { get; init; } = 1;

    /// <summary>Размер страницы, 1–1000.</summary>
    [DefaultValue(CatalogQueryTranslator.DefaultPageSize)]
    public int PageSize { get; init; } = CatalogQueryTranslator.DefaultPageSize;

    /// <summary>Записи, действующие на дату (начало ≤ дата ≤ конец). Только для справочников с периодом.</summary>
    public DateOnly? AsOf { get; init; }

    /// <summary>По умолчанию архивные записи не возвращаются.</summary>
    public bool IncludeArchived { get; init; }
}

public enum CatalogExportKind
{
    /// <summary>Колонки грида.</summary>
    Short,

    /// <summary>Поля карточки.</summary>
    Full
}

public sealed class CatalogExportRequest : ICatalogQueryRequest
{
    [DefaultValue(CatalogExportKind.Short)]
    public CatalogExportKind Kind { get; init; } = CatalogExportKind.Short;

    public IReadOnlyList<CatalogFilterRequest>? Filters { get; init; }

    public CatalogSortRequest? Sort { get; init; }

    public DateOnly? AsOf { get; init; }

    public bool IncludeArchived { get; init; }
}

/// <param name="Items">Строки страницы (тот же DTO, что у грида).</param>
/// <param name="TotalCount">Число строк под фильтром.</param>
/// <param name="DataAsOf">Время последней синхронизации snapshot с легаси (UTC); <c>null</c> — неизвестно.</param>
public sealed record CatalogSearchResponse<TItem>(
    IReadOnlyList<TItem> Items,
    int TotalCount,
    DateTime? DataAsOf);

public sealed record CatalogInfo(
    string Code,
    string TitleRu,
    string TitleEn,
    string Domain,
    IReadOnlyList<string> Capabilities);

public sealed record CatalogListResponse(IReadOnlyList<CatalogInfo> Items);

public sealed record CatalogEnumValue(string Code, string Title);

public sealed record CatalogReference(string Catalog, string Field);

public sealed record CatalogPeriodSchema(string From, string To);

public sealed record CatalogFieldSchema(
    string Name,
    string Type,
    string Title,
    bool Nullable,
    bool Filterable,
    bool Sortable,
    IReadOnlyList<string> Operators,
    IReadOnlyList<CatalogEnumValue>? EnumValues,
    CatalogReference? Reference);

public sealed record CatalogDetailFieldSchema(
    string Name,
    string Type,
    string Title,
    string? Group,
    bool Nullable,
    IReadOnlyList<CatalogEnumValue>? EnumValues);

public sealed record CatalogSchemaResponse(
    string Code,
    string Title,
    string EntityKey,
    CatalogPeriodSchema? Period,
    IReadOnlyList<CatalogFieldSchema> Fields,
    IReadOnlyList<CatalogDetailFieldSchema> DetailFields);
