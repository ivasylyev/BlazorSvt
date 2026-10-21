using System.Data;
using System.Data.SqlClient;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>
/// Свежесть snapshot для <c>dataAsOf</c>: самое старое <c>v2.SyncState.LastRunUtc</c> среди источников справочника
/// (данные не старее этой отметки).
/// </summary>
public interface ICatalogFreshnessReader
{
    Task<DateTime?> GetDataAsOfAsync(string syncEntity, CancellationToken cancellationToken);
}

public sealed class CatalogFreshnessReader(
    IOptions<DatabaseOptions> options,
    ILogger<CatalogFreshnessReader> logger) : ICatalogFreshnessReader
{
    private const string Sql = "SELECT MIN(LastRunUtc) FROM v2.SyncState WHERE Entity = @Entity";

    public async Task<DateTime?> GetDataAsOfAsync(string syncEntity, CancellationToken cancellationToken)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(options.Value.MdmDb);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(cancellationToken);

        var parameters = new DynamicParameters();
        parameters.Add("Entity", syncEntity);

        var db = new DbConnectionLogDecorator(connection, logger, options.Value.DefaultQueryTimeoutSeconds);
        var value = await db.QuerySingleOrDefaultAsync<DateTime?>(Sql, parameters, CommandType.Text, cancellationToken);

        return value is { } utc ? DateTime.SpecifyKind(utc, DateTimeKind.Utc) : null;
    }
}
