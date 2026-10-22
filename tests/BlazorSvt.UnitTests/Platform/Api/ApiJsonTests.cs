using System.Text.Json;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Domain.IdsEnum;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Api;

[Trait("Category", "Unit")]
public class ApiJsonTests
{
    private static readonly JsonSerializerOptions Options = Create();

    [Fact]
    public void Serialize_UsesContractFormats()
    {
        var json = JsonSerializer.Serialize(
            new Sample(
                new DateOnly(2026, 10, 20),
                new DateTime(2026, 10, 20, 7, 59, 59, 123, DateTimeKind.Utc),
                Currency.RUB,
                null,
                1234.50m),
            Options);

        json.Should().Be(
            """{"dateValue":"2026-10-20","stamp":"2026-10-20T07:59:59.123Z","currency":"RUB","optional":null,"amount":1234.50}""");
    }

    [Fact]
    public void Serialize_UnspecifiedDateTime_IsTreatedAsServerLocalTime()
    {
        var local = new DateTime(2026, 6, 18, 15, 52, 51, DateTimeKind.Unspecified);
        var expected = DateTime.SpecifyKind(local, DateTimeKind.Local).ToUniversalTime();

        var json = JsonSerializer.Serialize(local, Options);

        json.Should().Be($"\"{expected:yyyy-MM-dd'T'HH:mm:ss.fff'Z'}\"");
    }

    [Fact]
    public void Deserialize_EnumIsCaseInsensitive()
    {
        JsonSerializer.Deserialize<Currency>("\"rub\"", Options).Should().Be(Currency.RUB);
    }

    private static JsonSerializerOptions Create()
    {
        var options = new JsonSerializerOptions();
        ApiJson.Configure(options);
        return options;
    }

    private sealed record Sample(DateOnly DateValue, DateTime Stamp, Currency Currency, string? Optional, decimal Amount);
}
