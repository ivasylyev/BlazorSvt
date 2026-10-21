using BlazorBootstrap;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Реализация <see cref="IGridDataService{TItem,TDetailItem}"/>.
/// Метаданные list/detail берутся из атрибутов DTO через <see cref="GridColumnMetadataBuilder"/>.
/// </summary>
/// <remarks>
/// Таймауты: <c>DefaultQueryTimeoutSeconds</c> — grid/detail;
/// <c>ReportQueryTimeoutSeconds</c> — Excel-батчи (могут быть тяжелее).
/// Detail — raw SQL к view из <see cref="DetailSourceAttribute"/> (не snapshot).
/// </remarks>
public class GridDataService<TItem, TDetailItem>(
    IOptions<DatabaseOptions> options,
    ILogger<GridDataService<TItem, TDetailItem>> logger,
    IGridQueryFactory<TItem> gridQueryFactory,
    IAccessGuard accessGuard)
    : IGridDataService<TItem, TDetailItem>
{
    private readonly string connectionString = options.Value.MdmDb;
    private readonly int defaultQueryTimeoutSeconds = options.Value.DefaultQueryTimeoutSeconds;
    private readonly int reportQueryTimeoutSeconds = options.Value.ReportQueryTimeoutSeconds;

    public Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetShortReportBatchAsync(gridQueryFactory.Create(request, pageNumber, pageSize), cancellationToken);

    public async Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(GridQuery query, CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

        var (data, _) = await ExecuteStoredProcedureAsync(
            query,
            cancellationToken,
            reportQueryTimeoutSeconds);

        return data;
    }

    public Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken) =>
        GetFullReportBatchAsync(gridQueryFactory.Create(request, pageNumber, pageSize), cancellationToken);

    public async Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(GridQuery query, CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(cancellationToken);

        var parameters = BuildExportParameters(typeof(TItem), typeof(TDetailItem), query);

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, reportQueryTimeoutSeconds);

        var data = (await loggingConnection.QueryAsync<TDetailItem>(
            ExportBlazorGridDetailProcedureName,
            parameters,
            CommandType.StoredProcedure,
            cancellationToken)).ToList();

        return data;
    }

    public async Task<TDetailItem> GetDetailDataAsync(object key)
    {
        var result = await FindDetailAsync(key, CancellationToken.None);
        if (result is not null)
        {
            return result;
        }

        logger.LogWarning(
            "Detail {DetailSource} not found for key {Key}",
            DetailSourceName,
            key);

        throw new Exception($"{DetailSourceName} with {key} returns no data");
    }

    public async Task<TDetailItem?> FindDetailAsync(object key, CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

        try
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
            await connection.OpenAsync(cancellationToken);

            var parameters = new DynamicParameters();
            parameters.Add("Key", key);

            var sql = $"SELECT * FROM {DetailSourceName} WHERE {DetailSourceKeyColumn} = @Key";

            var loggingConnection = new DbConnectionLogDecorator(connection, logger, defaultQueryTimeoutSeconds);

            return await loggingConnection.QuerySingleOrDefaultAsync<TDetailItem?>(
                sql,
                parameters,
                CommandType.Text,
                cancellationToken);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }

    public async Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request)
    {
        var page = await GetPageAsync(gridQueryFactory.Create(request), request.CancellationToken);

        return new GridDataProviderResult<TItem>
        {
            Data = page.Items,
            TotalCount = page.TotalCount
        };
    }

    public async Task<GridPage<TItem>> GetPageAsync(GridQuery query, CancellationToken cancellationToken)
    {
        accessGuard.Ensure(AccessAction.Read);

        var (data, totalCount) = await ExecuteStoredProcedureAsync(
            query,
            cancellationToken,
            defaultQueryTimeoutSeconds);

        return new GridPage<TItem>(data, totalCount);
    }

    private async Task<(IReadOnlyList<TItem>, int)> ExecuteStoredProcedureAsync(
        GridQuery query,
        CancellationToken cancellationToken,
        int commandTimeoutSeconds)
    {
        try
        {
#pragma warning disable CS0618 // Type or member is obsolete
            await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
            await connection.OpenAsync(cancellationToken);

            var parameters = BuildGridParameters(typeof(TItem), query);

            var loggingConnection = new DbConnectionLogDecorator(connection, logger, commandTimeoutSeconds);

            await using var multi = await loggingConnection.QueryMultipleAsync(
                GridStoredProcedureName,
                parameters,
                CommandType.StoredProcedure,
                cancellationToken);

            var data = (await multi.ReadAsync<TItem>()).ToList();
            var count = await ReadCountAsync(multi);

            return (data, count);
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex, cancellationToken))
        {
            throw new OperationCanceledException(cancellationToken);
        }
    }

    private static DynamicParameters BuildExportParameters(
        Type listDtoType,
        Type detailDtoType,
        GridQuery query)
    {
        var snapshotQuery = GridSnapshotQuery.For(listDtoType);
        var detailSource = detailDtoType.GetCustomAttribute<DetailSourceAttribute>()
            ?? throw new InvalidOperationException(
                $"DTO {detailDtoType.Name} does not have DetailSourceAttribute");

        var parameters = new DynamicParameters();

        parameters.Add("PageNumber", query.PageNumber);
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("TableName", snapshotQuery.TableName);
        parameters.Add("AllowedColumnsJson", snapshotQuery.AllowedColumnsJson);
        parameters.Add("SelectList", snapshotQuery.KeysOnlySelectList);
        parameters.Add("DetailViewName", detailSource.Name);
        parameters.Add("EntityKeyColumn", detailSource.KeyColumn);
        parameters.Add("SortKey", query.Sort.PropertyName);
        parameters.Add("SortDirection", query.Sort.Direction);

        var serializedFilter = JsonConvert.SerializeObject(query.Filters);
        parameters.Add("FilterJson", serializedFilter);

        return parameters;
    }

    private static DynamicParameters BuildGridParameters(Type dtoType, GridQuery query)
    {
        var snapshotQuery = GridSnapshotQuery.For(dtoType);
        var parameters = new DynamicParameters();

        parameters.Add("PageNumber", query.PageNumber);
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("TableName", snapshotQuery.TableName);
        parameters.Add("AllowedColumnsJson", snapshotQuery.AllowedColumnsJson);
        parameters.Add("SelectList", snapshotQuery.SelectList);
        parameters.Add("SortKey", query.Sort.PropertyName);
        parameters.Add("SortDirection", query.Sort.Direction);

        var serializedFilter = JsonConvert.SerializeObject(query.Filters);
        parameters.Add("FilterJson", serializedFilter);

        return parameters;
    }

    private const string GridStoredProcedureName = "v2.GetBlazorGridData";
    private const string ExportBlazorGridDetailProcedureName = "v2.ExportBlazorGridDetail";

    private static readonly string DetailSourceName =
        typeof(TDetailItem).GetCustomAttribute<DetailSourceAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have DetailSourceAttribute");

    private static readonly string DetailSourceKeyColumn =
        typeof(TDetailItem).GetCustomAttribute<DetailSourceAttribute>()?.KeyColumn
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have DetailSourceAttribute");


    private static async Task<int> ReadCountAsync(SqlMapper.GridReader multi)
    {
        var result = await multi.ReadFirstOrDefaultAsync<dynamic>();
        return result?.TotalCount ?? 0;
    }
}
