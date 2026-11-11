using System.Globalization;
using System.Text.Json;
using BlazorSvt.Platform.Api.Catalogs;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary>
/// Аргументы вызова инструмента. SDK входную схему не проверяет, поэтому проверка здесь:
/// лишний аргумент — <c>unknown_field</c>, пропущенный обязательный — <c>required</c>, не тот тип — <c>invalid_value</c>.
/// Ошибки копятся и уходят одним <c>validation_failed</c>, чтобы модель исправила всё за один шаг.
/// </summary>
public sealed class McpArguments
{
    private readonly IDictionary<string, JsonElement> values;
    private readonly Dictionary<string, string[]> errors = new();

    public McpArguments(IDictionary<string, JsonElement>? values, IReadOnlyCollection<string> allowed)
    {
        this.values = values ?? new Dictionary<string, JsonElement>();
        foreach (var name in this.values.Keys.Where(k => !allowed.Contains(k)))
        {
            errors[name] = [CatalogValidationCodes.UnknownField];
        }
    }

    public IReadOnlyDictionary<string, string[]> Errors => errors;

    public string? GetString(string name, bool required = false)
    {
        if (!TryGet(name, required, out var value))
        {
            return null;
        }

        var text = value.ValueKind == JsonValueKind.String ? value.GetString()?.Trim() : null;
        if (text is null)
        {
            return Invalid<string>(name);
        }

        if (text.Length == 0 && required)
        {
            errors[name] = [CatalogValidationCodes.Required];
            return null;
        }

        return text.Length == 0 ? null : text;
    }

    public int? GetInt32(string name) =>
        GetInt64(name) is { } number
            ? number is >= int.MinValue and <= int.MaxValue ? (int)number : InvalidValue<int>(name)
            : null;

    /// <summary>Целое; строка из цифр тоже принимается — модели нередко присылают числа строкой.</summary>
    public long? GetInt64(string name, bool required = false)
    {
        if (!TryGet(name, required, out var value))
        {
            return null;
        }

        if (value.ValueKind == JsonValueKind.Number && value.TryGetInt64(out var number))
        {
            return number;
        }

        if (value.ValueKind == JsonValueKind.String
            && long.TryParse(value.GetString(), NumberStyles.AllowLeadingSign, CultureInfo.InvariantCulture, out number))
        {
            return number;
        }

        return InvalidValue<long>(name);
    }

    public bool? GetBoolean(string name)
    {
        if (!TryGet(name, required: false, out var value))
        {
            return null;
        }

        return value.ValueKind switch
        {
            JsonValueKind.True => true,
            JsonValueKind.False => false,
            _ => InvalidValue<bool>(name)
        };
    }

    /// <summary>Дата <c>yyyy-MM-dd</c>.</summary>
    public DateOnly? GetDate(string name)
    {
        if (!TryGet(name, required: false, out var value))
        {
            return null;
        }

        return value.ValueKind == JsonValueKind.String
               && DateOnly.TryParseExact(value.GetString(), "yyyy-MM-dd", CultureInfo.InvariantCulture, DateTimeStyles.None, out var date)
            ? date
            : InvalidValue<DateOnly>(name);
    }

    public IReadOnlyList<string>? GetStringList(string name)
    {
        if (!TryGet(name, required: false, out var value))
        {
            return null;
        }

        if (value.ValueKind != JsonValueKind.Array
            || value.EnumerateArray().Any(e => e.ValueKind != JsonValueKind.String))
        {
            return Invalid<IReadOnlyList<string>>(name);
        }

        return value.EnumerateArray().Select(e => e.GetString()!).ToList();
    }

    /// <summary>Объект или массив в типе контракта API (фильтры, сортировка) — тем же сериализатором, что у REST.</summary>
    public T? GetObject<T>(string name, JsonSerializerOptions options)
        where T : class
    {
        if (!TryGet(name, required: false, out var value))
        {
            return null;
        }

        try
        {
            return value.Deserialize<T>(options) ?? Invalid<T>(name);
        }
        catch (JsonException)
        {
            return Invalid<T>(name);
        }
    }

    /// <summary>Ошибки типов: без них нельзя собрать запрос, поэтому они уходят раньше проверок контракта.</summary>
    public void ThrowIfInvalid()
    {
        if (errors.Count > 0)
        {
            throw ApiException.Validation(errors);
        }
    }

    private bool TryGet(string name, bool required, out JsonElement value)
    {
        if (values.TryGetValue(name, out value)
            && value.ValueKind is not (JsonValueKind.Null or JsonValueKind.Undefined))
        {
            return true;
        }

        if (required)
        {
            errors[name] = [CatalogValidationCodes.Required];
        }

        return false;
    }

    private T? Invalid<T>(string name)
        where T : class
    {
        errors[name] = [CatalogValidationCodes.InvalidValue];
        return null;
    }

    private T? InvalidValue<T>(string name)
        where T : struct
    {
        errors[name] = [CatalogValidationCodes.InvalidValue];
        return null;
    }
}
