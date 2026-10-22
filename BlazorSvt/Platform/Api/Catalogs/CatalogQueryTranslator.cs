using System.Globalization;
using System.Text.Json;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Коды ошибок полей запроса (значения в <c>errors</c> ProblemDetails).</summary>
public static class CatalogValidationCodes
{
    public const string Required = "required";
    public const string UnknownField = "unknown_field";
    public const string NotFilterable = "not_filterable";
    public const string NotSortable = "not_sortable";
    public const string OperatorNotAllowed = "operator_not_allowed";
    public const string InvalidValue = "invalid_value";
    public const string ValueTooShort = "value_too_short";
    public const string OutOfRange = "out_of_range";
    public const string NotSupported = "not_supported";
}

/// <summary>
/// Запрос API → <see cref="GridQuery"/> для <c>v2.GetBlazorGridData</c>. Чистая функция.
/// </summary>
/// <remarks>
/// Отсекает то, что SP молча проигнорировала бы (неизвестное поле, неподдерживаемый оператор,
/// невалидное значение, короткий текст для полнотекстового поиска), — клиент получает 400,
/// а не «фильтр не сработал». SP не меняется.
/// </remarks>
public static class CatalogQueryTranslator
{
    public const int DefaultPageSize = 100;
    public const int MaxPageSize = 1000;

    /// <summary>SP ищет текст через <c>CONTAINS</c>, только если очищенное значение длиннее 2 символов.</summary>
    public const int MinTextLength = 3;

    private const string IsArchiveProperty = "IsArchive";
    private static readonly char[] FullTextNoise = ['"', '*', '(', ')', '&', '|'];

    public static GridQuery Translate(
        Type itemType,
        ICatalogQueryRequest request,
        CatalogPeriod? period,
        int page,
        int pageSize)
    {
        var errors = new Dictionary<string, string[]>();

        if (page < 1)
        {
            errors["page"] = [CatalogValidationCodes.OutOfRange];
        }

        if (pageSize is < 1 or > MaxPageSize)
        {
            errors["pageSize"] = [CatalogValidationCodes.OutOfRange];
        }

        var fields = CatalogFields.For(itemType)
            .ToDictionary(f => f.Name, StringComparer.OrdinalIgnoreCase);

        var filters = new List<GridFilter>();
        var requestFilters = request.Filters ?? [];
        for (var i = 0; i < requestFilters.Count; i++)
        {
            var filter = TranslateFilter(requestFilters[i], $"filters[{i}]", fields, errors);
            if (filter is not null)
            {
                filters.Add(filter);
            }
        }

        if (request.AsOf is { } asOf)
        {
            if (period is null)
            {
                errors["asOf"] = [CatalogValidationCodes.NotSupported];
            }
            else
            {
                var date = FormatDate(asOf);
                filters.Add(new GridFilter(period.FromProperty, date, GridFilterOperators.LessThanOrEqualsOperator));
                filters.Add(new GridFilter(period.ToProperty, date, GridFilterOperators.GreaterThanOrEqualsOperator));
            }
        }

        var hasArchiveField = fields.Values.Any(f => f.PropertyName == IsArchiveProperty && f.Filterable);
        var filtersArchive = filters.Any(f => f.PropertyName == IsArchiveProperty);
        if (!request.IncludeArchived && hasArchiveField && !filtersArchive)
        {
            filters.Add(new GridFilter(IsArchiveProperty, "False", GridFilterOperators.EqualsOperator));
        }

        var sort = TranslateSort(request.Sort, fields, errors);

        if (errors.Count > 0)
        {
            throw ApiException.Validation(errors);
        }

        return new GridQuery(page, pageSize, sort, filters);
    }

    private static GridFilter? TranslateFilter(
        CatalogFilterRequest? filter,
        string path,
        IReadOnlyDictionary<string, CatalogField> fields,
        Dictionary<string, string[]> errors)
    {
        if (filter is null || string.IsNullOrWhiteSpace(filter.Field))
        {
            errors[$"{path}.field"] = [CatalogValidationCodes.Required];
            return null;
        }

        if (!fields.TryGetValue(filter.Field, out var field))
        {
            errors[$"{path}.field"] = [CatalogValidationCodes.UnknownField];
            return null;
        }

        if (!field.Filterable)
        {
            errors[$"{path}.field"] = [CatalogValidationCodes.NotFilterable];
            return null;
        }

        var op = filter.Operator?.Trim().ToLowerInvariant();
        if (string.IsNullOrEmpty(op))
        {
            errors[$"{path}.operator"] = [CatalogValidationCodes.Required];
            return null;
        }

        if (!field.Operators.Contains(op))
        {
            errors[$"{path}.operator"] = [CatalogValidationCodes.OperatorNotAllowed];
            return null;
        }

        if (filter.Value is not { } value || value.ValueKind is JsonValueKind.Null or JsonValueKind.Undefined)
        {
            errors[$"{path}.value"] = [CatalogValidationCodes.Required];
            return null;
        }

        var (sqlValue, error) = ToSqlValue(field, value);
        if (error is not null)
        {
            errors[$"{path}.value"] = [error];
            return null;
        }

        return new GridFilter(field.PropertyName, sqlValue, CatalogOperators.ToGridOperator(op));
    }

    private static GridSort TranslateSort(
        CatalogSortRequest? sort,
        IReadOnlyDictionary<string, CatalogField> fields,
        Dictionary<string, string[]> errors)
    {
        if (sort is null || string.IsNullOrWhiteSpace(sort.Field))
        {
            return new GridSort(null, "ASC");
        }

        string? propertyName = null;
        if (!fields.TryGetValue(sort.Field, out var field))
        {
            errors["sort.field"] = [CatalogValidationCodes.UnknownField];
        }
        else if (!field.Filterable)
        {
            errors["sort.field"] = [CatalogValidationCodes.NotSortable];
        }
        else
        {
            propertyName = field.PropertyName;
        }

        var direction = sort.Direction?.Trim().ToLowerInvariant() switch
        {
            null or "" or "asc" => "ASC",
            "desc" => "DESC",
            _ => null
        };
        if (direction is null)
        {
            errors["sort.direction"] = [CatalogValidationCodes.InvalidValue];
        }

        return new GridSort(propertyName, direction ?? "ASC");
    }

    private static (string? Value, string? Error) ToSqlValue(CatalogField field, JsonElement value)
    {
        var text = value.ValueKind switch
        {
            JsonValueKind.String => value.GetString(),
            JsonValueKind.Number => value.GetRawText(),
            JsonValueKind.True => "true",
            JsonValueKind.False => "false",
            _ => null
        };

        if (text is null)
        {
            return (null, CatalogValidationCodes.InvalidValue);
        }

        switch (field.Type)
        {
            case CatalogFieldTypes.String:
                return CleanFullText(text).Length < MinTextLength
                    ? (null, CatalogValidationCodes.ValueTooShort)
                    : (text, null);

            case CatalogFieldTypes.Integer:
                return long.TryParse(text, NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out var number)
                    ? (number.ToString(CultureInfo.InvariantCulture), null)
                    : (null, CatalogValidationCodes.InvalidValue);

            case CatalogFieldTypes.Number:
                return decimal.TryParse(
                        text,
                        NumberStyles.AllowLeadingSign | NumberStyles.AllowDecimalPoint | NumberStyles.AllowExponent,
                        CultureInfo.InvariantCulture,
                        out var dec)
                    ? (dec.ToString(CultureInfo.InvariantCulture), null)
                    : (null, CatalogValidationCodes.InvalidValue);

            case CatalogFieldTypes.Boolean:
                return bool.TryParse(text, out var flag)
                    ? (flag ? "True" : "False", null)
                    : (null, CatalogValidationCodes.InvalidValue);

            case CatalogFieldTypes.Date:
            case CatalogFieldTypes.DateTime:
                return TryParseDate(text, out var date)
                    ? (FormatDate(date), null)
                    : (null, CatalogValidationCodes.InvalidValue);

            case CatalogFieldTypes.Enum:
                var member = Enum.GetNames(field.EnumType!)
                    .FirstOrDefault(n => string.Equals(ApiJson.EnumCode(n), text, StringComparison.OrdinalIgnoreCase));
                if (member is null)
                {
                    return (null, CatalogValidationCodes.InvalidValue);
                }

                var id = Convert.ToInt64(Enum.Parse(field.EnumType!, member), CultureInfo.InvariantCulture);
                return (id.ToString(CultureInfo.InvariantCulture), null);

            default:
                return (null, CatalogValidationCodes.InvalidValue);
        }
    }

    /// <summary>Повторяет очистку значения в <c>v2.GetBlazorGridData</c> перед <c>CONTAINS</c>.</summary>
    public static string CleanFullText(string value)
    {
        var chars = value.Replace("'", "''").Select(c => FullTextNoise.Contains(c) ? ' ' : c).ToArray();
        return new string(chars).Trim(' ');
    }

    private static bool TryParseDate(string text, out DateOnly date) =>
        DateOnly.TryParseExact(text, "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out date)
        && date.Year >= 1753;

    private static string FormatDate(DateOnly date) => date.ToString("yyyy-MM-dd", CultureInfo.InvariantCulture);
}
