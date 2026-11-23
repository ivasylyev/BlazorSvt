using System.Data;
using System.Data.SqlClient;
using System.Reflection;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Таблицы дочерних примитивов на карточке: raw SQL к view из <see cref="DetailChildSourceAttribute"/>.
/// </summary>
public class DetailChildDataService(
    IOptions<DatabaseOptions> databaseOptions,
    IOptions<GridOptions> gridOptions,
    ILogger<DetailChildDataService> logger,
    IAccessGuard accessGuard)
    : IDetailChildDataService
{
    private readonly string connectionString = databaseOptions.Value.MdmDb;
    private readonly int queryTimeoutSeconds = databaseOptions.Value.DefaultQueryTimeoutSeconds;
    private readonly int rowLimit = gridOptions.Value.ChildSectionRowLimit;

    public async Task<DetailChildPage<TRow>> GetRowsAsync<TRow>(
        object parentKey,
        IReadOnlyCollection<int> states,
        CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

        var source = typeof(TRow).GetCustomAttribute<DetailChildSourceAttribute>()
            ?? throw new InvalidOperationException(
                $"DTO {typeof(TRow).Name} does not have DetailChildSourceAttribute");

        var where = $"WHERE {source.ParentKeyColumn} = @Key AND PrimitiveEntityDataStateId IN @States";
        var sql = $"""
            SELECT TOP (@Limit) * FROM {source.Name} {where} ORDER BY Id;
            SELECT COUNT(*) FROM {source.Name} {where};
            """;

        var parameters = new DynamicParameters();
        parameters.Add("Key", parentKey);
        parameters.Add("States", states);
        parameters.Add("Limit", rowLimit);

        try
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
            await connection.OpenAsync(cancellationToken);

            var loggingConnection = new DbConnectionLogDecorator(connection, logger, queryTimeoutSeconds);

            await using var multi = await loggingConnection.QueryMultipleAsync(
                sql,
                parameters,
                CommandType.Text,
                cancellationToken);

            var rows = (await multi.ReadAsync<TRow>()).ToList();
            var totalCount = await multi.ReadSingleAsync<int>();

            return new DetailChildPage<TRow>(rows, totalCount);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }
}
