namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>
/// Поле list-DTO ссылается на запись другого справочника API (для <c>/schema</c>: клиент строит выбор значения).
/// </summary>
/// <param name="catalog">Код справочника в API, напр. <c>locations-nodes</c>.</param>
/// <param name="field">Поле целевого справочника, с которым совпадает значение, напр. <c>code</c>.</param>
[AttributeUsage(AttributeTargets.Property)]
public sealed class ApiReferenceAttribute(string catalog, string field) : Attribute
{
    public string Catalog { get; } = catalog;

    public string Field { get; } = field;
}
