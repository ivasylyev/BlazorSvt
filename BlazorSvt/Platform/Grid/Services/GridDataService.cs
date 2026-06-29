using BlazorBootstrap;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Grid.Models;
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

    public async Task<IReadOnlyList<TItem>> GetShortReportDataAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int totalCount)
    {
        var query = gridQueryFactory.Create(request, lang, pageNumber: 1, pageSize: totalCount);

        var (data, _) = await ExecuteStoredProcedureAsync(
            query,
            request.CancellationToken,
            reportQueryTimeoutSeconds);

        return data.ToList();
    }

    public async Task<IReadOnlyList<TDetailItem>> GetFullReportDataAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int totalCount)
    {
        var query = gridQueryFactory.Create(request, lang, pageNumber: 1, pageSize: totalCount);

#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(request.CancellationToken);

        var parameters = BuildExportParameters(query);

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, reportQueryTimeoutSeconds);

        var data = (await loggingConnection.QueryAsync<TDetailItem>(
            FullReportExportProcedureName,
            parameters,
            CommandType.StoredProcedure)).ToList();

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
#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(cancellationToken);

        var parameters = BuildParameters(query);

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, commandTimeoutSeconds);

        await using var multi = await loggingConnection.QueryMultipleAsync(
            StoredProcedureName,
            parameters,
            CommandType.StoredProcedure);

        var data = (await multi.ReadAsync<TItem>()).ToList();
        var count = await ReadCountAsync(multi);

        return (data, count);
    }

    private static DynamicParameters BuildExportParameters(GridQuery query)
    {
        var parameters = new DynamicParameters();

        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Lang", query.Lang);
        parameters.Add("SortKey", query.Sort.PropertyName);
        parameters.Add("SortDirection", query.Sort.Direction);

        var serializedFilter = JsonConvert.SerializeObject(query.Filters);
        parameters.Add("FilterJson", serializedFilter);

        return parameters;
    }

    private static DynamicParameters BuildParameters(GridQuery query)
    {
        var parameters = new DynamicParameters();

        parameters.Add("PageNumber", query.PageNumber);
        parameters.Add("PageSize", query.PageSize);
        parameters.Add("Lang", query.Lang);

        parameters.Add("SortKey", query.Sort.PropertyName);
        parameters.Add("SortDirection", query.Sort.Direction);

        var serializedFilter = JsonConvert.SerializeObject(query.Filters);
        parameters.Add("FilterJson", serializedFilter);


        return parameters;
    }
    private static readonly string StoredProcedureName =
        typeof(TItem).GetCustomAttribute<StoredProcedureAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TItem).Name} does not have StoredProcedureAttribute");

    private static readonly string FullReportExportProcedureName =
        typeof(TDetailItem).GetCustomAttribute<FullReportExportAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have FullReportExportAttribute");

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
