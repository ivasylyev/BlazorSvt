using System.Data;
using System.Data.SqlClient;
using BlazorSvt.Platform.Grid.Models;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Configuration;
using Newtonsoft.Json;

namespace BlazorSvt.IntegrationTests.Infrastructure;

public sealed class DatabaseFixture : IDisposable
{
    public const string ConnectionStringEnvironmentVariable = "BLAZORSVT_TEST_CONNECTION";

    static DatabaseFixture()
    {
        SqlMapper.AddTypeHandler(new SqlDateOnlyTypeHandler());
    }

    public DatabaseFixture()
    {
        ConnectionString = ResolveConnectionString();
        IsAvailable = CanConnect(ConnectionString);
    }

    public string? ConnectionString { get; }

    public bool IsAvailable { get; }

    public void Dispose()
    {
    }

    public static string? ResolveConnectionString()
    {
        var fromEnvironment = Environment.GetEnvironmentVariable(ConnectionStringEnvironmentVariable);
        if (!string.IsNullOrWhiteSpace(fromEnvironment))
        {
            return fromEnvironment;
        }

        var appsettingsPath = Path.Combine(AppContext.BaseDirectory, "appsettings.json");
        if (!File.Exists(appsettingsPath))
        {
            return null;
        }

        var configuration = new ConfigurationBuilder()
            .AddJsonFile(appsettingsPath, optional: false)
            .Build();

        return configuration["Database:MdmDb"];
    }

    private static bool CanConnect(string? connectionString)
    {
        if (string.IsNullOrWhiteSpace(connectionString))
        {
            return false;
        }

        try
        {
#pragma warning disable CS0618
            using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
            connection.Open();
            return true;
        }
        catch
        {
            return false;
        }
    }
}

public static class GridSpTestHelper
{
    private const string GridStoredProcedureName = "v2.GetBlazorGridData";

    public static async Task<(IReadOnlyList<T> Rows, int TotalCount)> ExecuteGetBlazorGridDataAsync<T>(
        string connectionString,
        Type dtoType,
        GridQuery query,
        CancellationToken cancellationToken = default)
    {
        var snapshotQuery = GridSnapshotQuery.For(dtoType);
        var parameters = BuildGridParameters(snapshotQuery, query);

#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync(cancellationToken);

        await using var multi = await connection.QueryMultipleAsync(
            new CommandDefinition(
                GridStoredProcedureName,
                parameters,
                commandType: CommandType.StoredProcedure,
                cancellationToken: cancellationToken));

        var rows = (await multi.ReadAsync<T>()).ToList();
        var countRow = await multi.ReadFirstOrDefaultAsync<TotalCountRow>();
        return (rows, countRow?.TotalCount ?? 0);
    }

    public static async Task<TDetail?> QueryDetailViewAsync<TDetail>(
        string connectionString,
        string viewName,
        string keyColumn,
        object key,
        CancellationToken cancellationToken = default)
    {
#pragma warning disable CS0618
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618
        await connection.OpenAsync(cancellationToken);

        var sql = $"SELECT * FROM {viewName} WHERE {keyColumn} = @Key";
        return await connection.QuerySingleOrDefaultAsync<TDetail>(
            new CommandDefinition(sql, new { Key = key }, cancellationToken: cancellationToken));
    }

    public static GridQuery CreateDefaultQuery(int pageNumber = 1, int pageSize = 10) =>
        new(
            pageNumber,
            pageSize,
            "ru-RU",
            new GridSort(null, "ASC"),
            [new GridFilter("IsArchive", "False", GridFilterOperators.EqualsOperator)]);

    private static DynamicParameters BuildGridParameters(GridSnapshotQuery snapshotQuery, GridQuery query)
    {
        var parameters = new DynamicParameters();
        parameters.Add("PageNumber", query.PageNumber);
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("TableName", snapshotQuery.TableName);
        parameters.Add("AllowedColumnsJson", snapshotQuery.AllowedColumnsJson);
        parameters.Add("SelectList", snapshotQuery.SelectList);
        parameters.Add("SortKey", query.Sort.PropertyName);
        parameters.Add("SortDirection", query.Sort.Direction);
        parameters.Add("FilterJson", JsonConvert.SerializeObject(query.Filters));
        return parameters;
    }

    private sealed class TotalCountRow
    {
        public int TotalCount { get; init; }
    }
}

[CollectionDefinition("Database")]
public sealed class DatabaseCollection : ICollectionFixture<DatabaseFixture>
{
}

public abstract class IntegrationTestBase(DatabaseFixture fixture)
{
    protected DatabaseFixture Fixture { get; } = fixture;

    protected string RequireConnectionString()
    {
        Skip.IfNot(Fixture.IsAvailable,
            "Database is not available. Set BLAZORSVT_TEST_CONNECTION or provide appsettings.json.");
        return Fixture.ConnectionString!;
    }
}
