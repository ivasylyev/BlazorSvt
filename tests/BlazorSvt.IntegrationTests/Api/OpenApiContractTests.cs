using System.Text.Encodings.Web;
using System.Text.Json;
using System.Text.Json.Nodes;
using FluentAssertions;

namespace BlazorSvt.IntegrationTests.Api;

/// <summary>
/// Контракт для Атласа: <c>docs/api/svt-api-v1.openapi.json</c> должен совпадать с тем, что отдаёт приложение.
/// Осознанное изменение API — перегенерировать снимок: <c>UPDATE_OPENAPI_SNAPSHOT=1 dotnet test --filter OpenApiContractTests</c>.
/// </summary>
[Trait("Category", "Integration")]
public sealed class OpenApiContractTests(ApiTestFactory factory) : IClassFixture<ApiTestFactory>
{
    private const string UpdateVariable = "UPDATE_OPENAPI_SNAPSHOT";
    private static readonly string SnapshotRelativePath = Path.Combine("docs", "api", "svt-api-v1.openapi.json");

    private static readonly JsonSerializerOptions Indented = new()
    {
        WriteIndented = true,
        Encoder = JavaScriptEncoder.UnsafeRelaxedJsonEscaping
    };

    [Fact]
    public async Task OpenApiDocument_MatchesCommittedSnapshot()
    {
        var response = await factory.CreateApiClient(onBehalfOf: null).GetAsync("/api/openapi/v1.json");
        response.EnsureSuccessStatusCode();
        var actual = Normalize(await response.Content.ReadAsStringAsync());

        var snapshotPath = Path.Combine(FindRepositoryRoot(), SnapshotRelativePath);
        if (Environment.GetEnvironmentVariable(UpdateVariable) == "1")
        {
            Directory.CreateDirectory(Path.GetDirectoryName(snapshotPath)!);
            await File.WriteAllTextAsync(snapshotPath, actual);
        }

        File.Exists(snapshotPath).Should().BeTrue($"нет снимка контракта {SnapshotRelativePath}; создать: {UpdateVariable}=1");
        var expected = Normalize(await File.ReadAllTextAsync(snapshotPath));

        actual.Should().Be(
            expected,
            $"контракт API изменился. Если это осознанно — {UpdateVariable}=1 и согласовать изменение с Атласом");
    }

    [Fact]
    public async Task EveryOperation_HasOperationId()
    {
        var response = await factory.CreateApiClient(onBehalfOf: null).GetAsync("/api/openapi/v1.json");
        var document = JsonNode.Parse(await response.Content.ReadAsStringAsync())!;

        var operations = document["paths"]!.AsObject()
            .SelectMany(path => path.Value!.AsObject().Select(op => (Path: path.Key, Method: op.Key, Op: op.Value!)))
            .Where(x => x.Op is JsonObject)
            .ToList();

        operations.Should().NotBeEmpty();
        operations.Should().OnlyContain(x => x.Op["operationId"] != null, "у каждой операции должен быть operationId для генерации клиента");
        operations.Select(x => x.Op["operationId"]!.GetValue<string>()).Should().OnlyHaveUniqueItems();
        operations.Select(x => x.Path).Should().OnlyContain(p => p.StartsWith("/api/v1/"));
    }

    private static string Normalize(string json) =>
        JsonSerializer.Serialize(JsonNode.Parse(json), Indented).ReplaceLineEndings("\n") + "\n";

    private static string FindRepositoryRoot()
    {
        for (var dir = new DirectoryInfo(AppContext.BaseDirectory); dir is not null; dir = dir.Parent)
        {
            if (File.Exists(Path.Combine(dir.FullName, "BlazorSvt.sln")))
            {
                return dir.FullName;
            }
        }

        throw new InvalidOperationException("BlazorSvt.sln not found above " + AppContext.BaseDirectory);
    }
}
