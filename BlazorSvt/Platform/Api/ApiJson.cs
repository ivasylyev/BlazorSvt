using System.Globalization;
using System.Text.Json;
using System.Text.Json.Serialization;
using Microsoft.Extensions.Options;
using HttpJsonOptions = Microsoft.AspNetCore.Http.Json.JsonOptions;

namespace BlazorSvt.Platform.Api;

/// <summary>
/// JSON-контракт API: camelCase, enum — строковые коды (имена членов: <c>RUB</c>, <c>Tender</c>), <c>null</c> не опускается,
/// <see cref="DateOnly"/> — <c>yyyy-MM-dd</c>, <see cref="DateTime"/> — ISO 8601 UTC с <c>Z</c>.
/// </summary>
/// <remarks>
/// Настраивает только minimal API (<c>ConfigureHttpJsonOptions</c>): контроллеры и Blazor не затрагиваются.
/// </remarks>
public static class ApiJson
{
    public static readonly JsonNamingPolicy Naming = JsonNamingPolicy.CamelCase;

    public static void Configure(JsonSerializerOptions options)
    {
        options.PropertyNamingPolicy = Naming;
        options.DictionaryKeyPolicy = null;
        options.DefaultIgnoreCondition = JsonIgnoreCondition.Never;
        options.PropertyNameCaseInsensitive = true;
        options.Converters.Add(new JsonStringEnumConverter());
        options.Converters.Add(new UtcDateTimeJsonConverter());
        options.Converters.Add(new NullableUtcDateTimeJsonConverter());
    }

    public static JsonSerializerOptions Options(HttpContext context) =>
        context.RequestServices.GetRequiredService<IOptions<HttpJsonOptions>>().Value.SerializerOptions;

    /// <summary>Код значения enum в контракте — имя члена enum (как его пишет сериализатор).</summary>
    public static string EnumCode(string memberName) => memberName;

    /// <summary>Имя поля в контракте по имени свойства DTO.</summary>
    public static string FieldName(string propertyName) => Naming.ConvertName(propertyName);
}

/// <summary>
/// <see cref="DateTime"/> в UTC с суффиксом <c>Z</c>.
/// <see cref="DateTimeKind.Unspecified"/> (значения из БД легаси) трактуется как время сервера.
/// </summary>
public sealed class UtcDateTimeJsonConverter : JsonConverter<DateTime>
{
    private const string Format = "yyyy-MM-dd'T'HH:mm:ss.fff'Z'";

    public override DateTime Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options)
    {
        var text = reader.GetString();
        if (!DateTime.TryParse(
                text,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AdjustToUniversal | DateTimeStyles.AssumeUniversal,
                out var value))
        {
            throw new JsonException($"Invalid date-time '{text}'.");
        }

        return DateTime.SpecifyKind(value, DateTimeKind.Utc);
    }

    public override void Write(Utf8JsonWriter writer, DateTime value, JsonSerializerOptions options) =>
        writer.WriteStringValue(ToUtc(value).ToString(Format, CultureInfo.InvariantCulture));

    public static DateTime ToUtc(DateTime value) =>
        value.Kind switch
        {
            DateTimeKind.Utc => value,
            DateTimeKind.Local => value.ToUniversalTime(),
            _ => DateTime.SpecifyKind(value, DateTimeKind.Local).ToUniversalTime()
        };
}

public sealed class NullableUtcDateTimeJsonConverter : JsonConverter<DateTime?>
{
    private readonly UtcDateTimeJsonConverter inner = new();

    public override DateTime? Read(ref Utf8JsonReader reader, Type typeToConvert, JsonSerializerOptions options) =>
        reader.TokenType == JsonTokenType.Null ? null : inner.Read(ref reader, typeof(DateTime), options);

    public override void Write(Utf8JsonWriter writer, DateTime? value, JsonSerializerOptions options)
    {
        if (value is null)
        {
            writer.WriteNullValue();
            return;
        }

        inner.Write(writer, value.Value, options);
    }
}
