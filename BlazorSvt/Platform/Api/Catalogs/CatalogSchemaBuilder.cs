using System.Reflection;
using BlazorBootstrap;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Описание справочника для <c>/schema</c>: метаданные DTO + заголовки из настроек грида/карточки.</summary>
public static class CatalogSchemaBuilder
{
    /// <param name="descriptor">Регистрация справочника.</param>
    /// <param name="title">Заголовок справочника на языке запроса.</param>
    /// <param name="listTitles">Свойство list-DTO → заголовок колонки.</param>
    /// <param name="detailTitles">Свойство detail-DTO → (заголовок, группа).</param>
    public static CatalogSchemaResponse Build(
        CatalogApiDescriptor descriptor,
        string title,
        IReadOnlyDictionary<string, string> listTitles,
        IReadOnlyDictionary<string, (string Title, string? Group)> detailTitles)
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(descriptor.ItemType);

        var fields = CatalogFields.For(descriptor.ItemType)
            .Select(f => new CatalogFieldSchema(
                f.Name,
                f.Type,
                listTitles.GetValueOrDefault(f.PropertyName) ?? f.Name,
                f.Nullable,
                f.Filterable,
                f.Filterable,
                f.Operators,
                EnumValues(f.EnumType),
                f.Reference is { } r ? new CatalogReference(r.Catalog, r.Field) : null))
            .ToList();

        var detailFields = descriptor.DetailType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Select(p =>
            {
                var hasTitle = detailTitles.TryGetValue(p.Name, out var t);
                return new CatalogDetailFieldSchema(
                    ApiJson.FieldName(p.Name),
                    CatalogFields.TypeOf(p.PropertyType),
                    hasTitle ? t.Title : ApiJson.FieldName(p.Name),
                    hasTitle ? t.Group : null,
                    CatalogFields.IsNullable(p),
                    EnumValues(CatalogFields.EnumTypeOf(p.PropertyType)));
            })
            .ToList();

        var period = descriptor.Period is { } pr
            ? new CatalogPeriodSchema(ApiJson.FieldName(pr.FromProperty), ApiJson.FieldName(pr.ToProperty))
            : null;

        return new CatalogSchemaResponse(
            descriptor.Code,
            title,
            ApiJson.FieldName(metadata.EntityKeyPropertyName
                ?? throw new InvalidOperationException($"{descriptor.ItemType.Name} has no entity key")),
            period,
            fields,
            detailFields);
    }

    private static IReadOnlyList<CatalogEnumValue>? EnumValues(Type? enumType) =>
        enumType is null
            ? null
            : Enum.GetNames(enumType)
                .Select(n => new CatalogEnumValue(ApiJson.EnumCode(n), enumType.GetDisplayName(n) ?? n))
                .ToList();
}
