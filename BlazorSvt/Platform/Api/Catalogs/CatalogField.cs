using System.Collections.Concurrent;
using System.Reflection;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Тип поля в контракте API.</summary>
public static class CatalogFieldTypes
{
    public const string String = "string";
    public const string Integer = "integer";
    public const string Number = "number";
    public const string Boolean = "boolean";
    public const string Date = "date";
    public const string DateTime = "dateTime";
    public const string Enum = "enum";
}

/// <summary>Операторы фильтра в контракте API.</summary>
public static class CatalogOperators
{
    public const string Contains = "contains";
    public const string Eq = "eq";
    public const string Ne = "ne";
    public const string Lt = "lt";
    public const string Le = "le";
    public const string Gt = "gt";
    public const string Ge = "ge";

    public static readonly IReadOnlyList<string> Comparison = [Eq, Ne, Lt, Le, Gt, Ge];

    /// <summary>
    /// Что реально исполняет <c>v2.GetBlazorGridData</c>: текст — только полнотекстовый «содержит»,
    /// флаги — равенство; enum — равенство/неравенство кода.
    /// </summary>
    public static IReadOnlyList<string> For(string fieldType) =>
        fieldType switch
        {
            CatalogFieldTypes.String => [Contains],
            CatalogFieldTypes.Boolean => [Eq],
            CatalogFieldTypes.Enum => [Eq, Ne],
            _ => Comparison
        };

    public static string ToGridOperator(string apiOperator) =>
        apiOperator switch
        {
            Contains => GridFilterOperators.ContainsOperator,
            Eq => GridFilterOperators.EqualsOperator,
            Ne => GridFilterOperators.NotEqualsOperator,
            Lt => GridFilterOperators.LessThanOperator,
            Le => GridFilterOperators.LessThanOrEqualsOperator,
            Gt => GridFilterOperators.GreaterThanOperator,
            Ge => GridFilterOperators.GreaterThanOrEqualsOperator,
            _ => throw new ArgumentOutOfRangeException(nameof(apiOperator), apiOperator, null)
        };
}

/// <summary>Поле list-DTO в терминах API (имя, тип, допустимые операторы).</summary>
public sealed record CatalogField(
    string Name,
    string PropertyName,
    string Type,
    Type? EnumType,
    bool Nullable,
    bool Filterable,
    ApiReferenceAttribute? Reference)
{
    public IReadOnlyList<string> Operators => Filterable ? CatalogOperators.For(Type) : [];
}

/// <summary>Поля list-DTO справочника; кэш по типу DTO.</summary>
public static class CatalogFields
{
    private static readonly ConcurrentDictionary<Type, IReadOnlyList<CatalogField>> Cache = new();

    public static IReadOnlyList<CatalogField> For(Type itemType) => Cache.GetOrAdd(itemType, Build);

    public static string TypeOf(Type clrType)
    {
        var type = System.Nullable.GetUnderlyingType(clrType) ?? clrType;
        if (type.IsEnum) return CatalogFieldTypes.Enum;
        if (type == typeof(string)) return CatalogFieldTypes.String;
        if (type == typeof(bool)) return CatalogFieldTypes.Boolean;
        if (type == typeof(DateOnly)) return CatalogFieldTypes.Date;
        if (type == typeof(DateTime)) return CatalogFieldTypes.DateTime;
        if (type == typeof(decimal) || type == typeof(double) || type == typeof(float)) return CatalogFieldTypes.Number;
        if (type == typeof(long) || type == typeof(int) || type == typeof(short) || type == typeof(byte)) return CatalogFieldTypes.Integer;
        return CatalogFieldTypes.String;
    }

    public static bool IsNullable(PropertyInfo property) =>
        System.Nullable.GetUnderlyingType(property.PropertyType) is not null
        || (!property.PropertyType.IsValueType
            && new NullabilityInfoContext().Create(property).ReadState == NullabilityState.Nullable);

    public static Type? EnumTypeOf(Type clrType)
    {
        var type = System.Nullable.GetUnderlyingType(clrType) ?? clrType;
        return type.IsEnum ? type : null;
    }

    private static IReadOnlyList<CatalogField> Build(Type itemType)
    {
        var metadata = GridColumnMetadataBuilder.GetMetadata(itemType);
        var properties = itemType
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .ToDictionary(p => p.Name);

        return metadata.Columns
            .Where(c => c.IncludeInSelect)
            .Select(c =>
            {
                var property = properties[c.PropertyName];
                return new CatalogField(
                    ApiJson.FieldName(c.PropertyName),
                    c.PropertyName,
                    TypeOf(property.PropertyType),
                    EnumTypeOf(property.PropertyType),
                    IsNullable(property),
                    c.Filterable,
                    property.GetCustomAttribute<ApiReferenceAttribute>());
            })
            .ToList();
    }
}
