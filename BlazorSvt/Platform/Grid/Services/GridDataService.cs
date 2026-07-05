using BlazorBootstrap;
using BlazorSvt.Platform.Infrastructure;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace BlazorSvt.Platform.Grid.Services;

public class GridDataService<TItem, TDetailItem>(
    IOptions<DatabaseOptions> options,
    ILogger<GridDataService<TItem, TDetailItem>> logger,
    IGridQueryFactory<TItem> gridQueryFactory)
    : IGridDataService<TItem, TDetailItem>
{
    private readonly string connectionString = options.Value.MdmDb;
    private readonly int defaultQueryTimeoutSeconds = options.Value.DefaultQueryTimeoutSeconds;
    private readonly int reportQueryTimeoutSeconds = options.Value.ReportQueryTimeoutSeconds;

    public async Task<IReadOnlyList<TItem>> GetShortReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = gridQueryFactory.Create(request, lang, pageNumber, pageSize);

        var (data, _) = await ExecuteStoredProcedureAsync(
            query,
            cancellationToken,
            reportQueryTimeoutSeconds);

        return data.ToList();
    }

    public async Task<IReadOnlyList<TDetailItem>> GetFullReportBatchAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int pageNumber,
        int pageSize,
        CancellationToken cancellationToken)
    {
        var query = gridQueryFactory.Create(request, lang, pageNumber, pageSize);

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
#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync();

        var parameters = new DynamicParameters();
        parameters.Add("Key", key);

        var sql = $"SELECT * FROM {DetailSourceName} WHERE {DetailSourceKeyColumn} = @Key";

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, defaultQueryTimeoutSeconds);

        var result = await loggingConnection.QuerySingleOrDefaultAsync<TDetailItem?>(
            sql,
            parameters,
            CommandType.Text);

        return result ?? throw new Exception($"{DetailSourceName} with {key} returns no data");
    }

    public async Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang)
    {
        var query = gridQueryFactory.Create(request, lang);

        var (data, totalCount) = await ExecuteStoredProcedureAsync(
            query,
            request.CancellationToken,
            defaultQueryTimeoutSeconds);

        return new GridDataProviderResult<TItem>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

    private async Task<(IEnumerable<TItem>, int)> ExecuteStoredProcedureAsync(
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
