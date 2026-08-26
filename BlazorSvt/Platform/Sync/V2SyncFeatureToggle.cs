using System.Data;
using System.Data.SqlClient;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Читает <c>V2SyncEnabled</c> из <c>dbo.vw_FeatureToggle</c> (легаси FeatureToggle).
/// Fail-closed: нет строки / исключение → синхронизация считается выключенной.
/// </summary>
public sealed class V2SyncFeatureToggle(
    IOptions<DatabaseOptions> databaseOptions,
    ILogger<V2SyncFeatureToggle> logger) : IV2SyncFeatureToggle
{
    internal const string ToggleCode = "V2SyncEnabled";

    private const string IsEnabledSql =
        """
        SELECT CAST(ISNULL((
            SELECT [ft].[ToggleState]
            FROM [dbo].[vw_FeatureToggle] AS [ft]
            WHERE [ft].[Code] = @Code
        ), 0) AS BIT)
        """;

    private readonly string connectionString = databaseOptions.Value.MdmDb;
    private readonly int commandTimeout = databaseOptions.Value.DefaultQueryTimeoutSeconds;

    public async Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default)
    {
        try
        {
            await using var connection = OpenConnection();
            await connection.OpenAsync(cancellationToken);
            var db = new DbConnectionLogDecorator(connection, logger, commandTimeout);

            var parameters = new DynamicParameters();
            parameters.Add("Code", ToggleCode);

            return await db.QuerySingleAsync<bool>(
                IsEnabledSql,
                parameters,
                CommandType.Text,
                cancellationToken);
        }
        catch (Exception ex) when (!OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            logger.LogError(
                ex,
                "SnapshotSync: не удалось прочитать FeatureToggle {Code}; синхронизация считается выключенной.",
                ToggleCode);
            return false;
        }
    }

    private SqlConnection OpenConnection()
    {
#pragma warning disable CS0618 // System.Data.SqlClient — как в остальном коде проекта
        return new SqlConnection(connectionString);
#pragma warning restore CS0618
    }
}
