using System.Text.Json;
using System.Text.Json.Nodes;
using BlazorSvt.Platform.Api.Catalogs;

namespace BlazorSvt.Platform.Api.Mcp;

/// <summary>
/// Аргумент <c>fields</c>: в ответе остаются только перечисленные свойства (меньше токенов в контексте модели).
/// Отбор — по уже полученным данным, запрос к БД не меняется.
/// </summary>
public static class McpFieldSelection
{
    /// <summary>
    /// Проверяет список и возвращает имена в порядке DTO; <c>null</c> — отбора нет (аргумент не задан или с ошибкой).
    /// </summary>
    /// <param name="requested">Значение аргумента; регистр не важен, повторы схлопываются.</param>
    /// <param name="available">Имена полей контракта в порядке DTO (<c>fields[].name</c> или <c>detailFields[].name</c> схемы).</param>
    /// <param name="key">Ключ записи: добавляется всегда, если он есть среди <paramref name="available"/>.</param>
    /// <param name="argument">Имя аргумента для путей ошибок.</param>
    /// <param name="errors">Куда записать ошибки: пустой список — <c>required</c>, неизвестное имя — <c>unknown_field</c> по индексу.</param>
    public static IReadOnlyList<string>? Resolve(
        IReadOnlyList<string>? requested,
        IReadOnlyList<string> available,
        string? key,
        string argument,
        IDictionary<string, string[]> errors)
    {
        if (requested is null)
        {
            return null;
        }

        if (requested.Count == 0)
        {
            errors[argument] = [CatalogValidationCodes.Required];
            return null;
        }

        var byName = available.ToDictionary(n => n, StringComparer.OrdinalIgnoreCase);
        var chosen = new HashSet<string>(StringComparer.Ordinal);
        var valid = true;
        for (var i = 0; i < requested.Count; i++)
        {
            if (byName.TryGetValue(requested[i].Trim(), out var name))
            {
                chosen.Add(name);
            }
            else
            {
                errors[$"{argument}[{i}]"] = [CatalogValidationCodes.UnknownField];
                valid = false;
            }
        }

        if (!valid)
        {
            return null;
        }

        if (key is not null)
        {
            chosen.Add(key);
        }

        return available.Where(chosen.Contains).ToList();
    }

    /// <summary>Объект только с выбранными свойствами (имена — как в JSON-контракте).</summary>
    public static JsonObject Project(JsonElement item, IReadOnlyList<string> names)
    {
        var result = new JsonObject();
        foreach (var name in names)
        {
            if (item.TryGetProperty(name, out var value))
            {
                result[name] = JsonNode.Parse(value.GetRawText());
            }
        }

        return result;
    }
}
