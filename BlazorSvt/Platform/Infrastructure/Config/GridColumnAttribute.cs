namespace BlazorSvt.Platform.Infrastructure.Config;

/// <summary>
/// Колонка list-DTO для read-модели. <see cref="GridColumnMetadataBuilder"/>
/// собирает из атрибутов параметры <c>v2.GetBlazorGridData</c>
/// (<c>@AllowedColumnsJson</c>, <c>@SelectList</c>).
/// </summary>
/// <remarks>
/// <para>
/// <see cref="SqlColumn"/> — имя колонки в snapshot, если оно отличается от
/// свойства DTO. Типичный случай: локализованные enum-свойства
/// (<c>RateTypeIdRu</c> / <c>RateTypeIdEn</c>) оба мапятся на одну SQL-колонку
/// (<c>RateTypeId</c>); фильтр идёт по ID, отображение — по enum Display.
/// </para>
/// <para>
/// Ровно одно свойство DTO должно иметь <see cref="IsEntityKey"/> = true
/// (бизнес-ключ snapshot, напр. <c>TransportRateId</c>).
/// </para>
/// </remarks>
[AttributeUsage(AttributeTargets.Property)]
public sealed class GridColumnAttribute : Attribute
{
    /// <summary>Тип колонки для whitelist фильтров; если null — выводится из CLR-типа.</summary>
    public GridColumnType? ColumnType { get; init; }

    /// <summary>Колонка snapshot; по умолчанию = имя свойства DTO.</summary>
    public string? SqlColumn { get; init; }

    /// <summary>Преобразование в SELECT; <see cref="GridSelectTransform.Auto"/> — CastAsDate для DateOnly.</summary>
    public GridSelectTransform SelectTransform { get; init; } = GridSelectTransform.Auto;

    /// <summary>Полное выражение SELECT вместо автосборки (редко нужно).</summary>
    public string? SelectExpression { get; init; }

    /// <summary>Участвует ли колонка в фильтрации (@AllowedColumnsJson).</summary>
    public bool Filterable { get; init; } = true;

    /// <summary>Включать ли колонку в @SelectList grid.</summary>
    public bool IncludeInSelect { get; init; } = true;

    /// <summary>Бизнес-ключ сущности (ровно одно свойство на DTO).</summary>
    public bool IsEntityKey { get; init; }
}
