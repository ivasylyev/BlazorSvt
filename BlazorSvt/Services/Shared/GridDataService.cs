using BlazorBootstrap;
using BlazorSvt.Models.Config;
using BlazorSvt.Utils;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;
using System.Data;
using System.Data.SqlClient;
using System.Reflection;

namespace BlazorSvt.Services.Shared;

public class GridDataService<TItem, TDetailItem>(IOptions<DatabaseOptions> options, ILogger<GridDataService<TItem, TDetailItem>> logger) :IGridDataService<TItem, TDetailItem>
{
    private const string IsArchiveFieldName = "IsArchive";

    private readonly string connectionString = options.Value.MdmDb;
    private readonly int defaultQueryTimeoutSeconds = options.Value.DefaultQueryTimeoutSeconds;
    private readonly int reportQueryTimeoutSeconds = options.Value.ReportQueryTimeoutSeconds;

    public async Task<IReadOnlyList<TItem>> GetShortReportDataAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int totalCount)
    {
        var (sortString, sortDirection) = ExtractSorting(request);
        var filters = ExtractFilters(request);

        var (data, _) = await ExecuteStoredProcedureAsync(
            filters,
            pageNumber: 1,
            pageSize: totalCount,
            sortString,
            sortDirection,
            lang,
            request.CancellationToken,
            reportQueryTimeoutSeconds);

        return data.ToList();
    }

    public async Task<IReadOnlyList<TDetailItem>> GetFullReportDataAsync(
        GridDataProviderRequest<TItem> request,
        string lang,
        int totalCount)
    {
        var (sortString, sortDirection) = ExtractSorting(request);
        var filters = ExtractFilters(request);

#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(request.CancellationToken);

        var parameters = BuildExportParameters(filters, totalCount, sortString, sortDirection, lang);

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

        var sql = $"SELECT * FROM {DetailTableFunctionName}() WHERE {DetailTableFunctionKeyColumn} = @Key";

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, defaultQueryTimeoutSeconds);

        var result = await loggingConnection.QuerySingleOrDefaultAsync<TDetailItem?>(
            sql,
            parameters,
            CommandType.Text);

        return result ?? throw new Exception($"{DetailTableFunctionName} with {key} returns no data");
    }

    public async Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang)
    {
        var (sortString, sortDirection) = ExtractSorting(request);

        var filters = ExtractFilters(request);

        var (data, totalCount) = await ExecuteStoredProcedureAsync(
            filters,
            request.PageNumber,
            request.PageSize,
            sortString,
            sortDirection,
            lang,
            request.CancellationToken,
            defaultQueryTimeoutSeconds);

        return new GridDataProviderResult<TItem>
        {
            Data = data,
            TotalCount = totalCount
        };
    }

    private static (string? sortString, SortDirection sortDirection) ExtractSorting(GridDataProviderRequest<TItem> request)
    {
        // Бывает что request.Sorting пустой из-за того что компонент не успел инициализироваться
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request.Sorting is not null && request.Sorting.Any())
        {
            var sort = request.Sorting.First();
            return (sort.SortString, sort.SortDirection);
        }

        return (null, SortDirection.None);
    }

    private List<FilterItem> ExtractFilters(GridDataProviderRequest<TItem> request)
    {
        // Бывает что request.Filters пустой из-за того что компонент не успел инициализироваться
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request.Filters is not null)
            return request.Filters.Select(ExtractEnumFilter).ToList();

        return
        [
            new FilterItem(
                IsArchiveFieldName,
                "False",
                FilterOperator.Equals,
                StringComparison.InvariantCultureIgnoreCase)
        ];
    }

    private static FilterItem ExtractEnumFilter(FilterItem filter)
    {
        DtoProperties.TryGetValue(filter.PropertyName, out var propInfo);
        if (propInfo != null)
        {
            // Проверяем, является ли тип перечислением (учитываем Nullable<Enum>)
            var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (propType.IsEnum && filter.Value != null)
            {
                if (Enum.TryParse(propType, filter.Value, out var enumValue))
                {
                    var value = ((int)enumValue).ToString();
                    return new FilterItem(filter.PropertyName, value, filter.Operator, filter.StringComparison);
                }
            }
        }
        return filter;
    }



    private async Task<(IEnumerable<TItem>, int)> ExecuteStoredProcedureAsync(
        IEnumerable<FilterItem> filters,
        int pageNumber,
        int pageSize,
        string? sortKey,
        SortDirection sortDirection,
        string lang,
        CancellationToken cancellationToken,
        int commandTimeoutSeconds)
    {
#pragma warning disable CS0618 // Type or member is obsolete
        await using var connection = new SqlConnection(connectionString);
#pragma warning restore CS0618 // Type or member is obsolete
        await connection.OpenAsync(cancellationToken);

        var parameters = BuildParameters(filters, pageNumber, pageSize, sortKey, sortDirection, lang);

        var loggingConnection = new DbConnectionLogDecorator(connection, logger, commandTimeoutSeconds);

        await using var multi = await loggingConnection.QueryMultipleAsync(
            StoredProcedureName,
            parameters,
            CommandType.StoredProcedure);

        var data = (await multi.ReadAsync<TItem>()).ToList();
        var count = await ReadCountAsync(multi);

        return (data, count);
    }

    private static DynamicParameters BuildExportParameters(
        IEnumerable<FilterItem> filters,
        int pageSize,
        string? sortKey,
        SortDirection sortDirection,
        string lang)
    {
        var parameters = new DynamicParameters();

        parameters.Add("PageSize", pageSize);
        parameters.Add("Lang", lang);
        parameters.Add("SortKey", sortKey);
        parameters.Add("SortDirection", sortDirection == SortDirection.Descending ? "DESC" : "ASC");

        var serializedFilter = JsonConvert.SerializeObject(filters);
        parameters.Add("FilterJson", serializedFilter);

        return parameters;
    }

    private static DynamicParameters BuildParameters(
        IEnumerable<FilterItem> filters,
        int pageNumber,
        int pageSize,
        string? sortKey,
        SortDirection sortDirection, string lang)
    {
        var parameters = new DynamicParameters();

        parameters.Add("PageNumber", pageNumber);
        parameters.Add("PageSize", pageSize);
        parameters.Add("Lang", lang);

        parameters.Add("SortKey", sortKey);
        parameters.Add("SortDirection", sortDirection == SortDirection.Descending ? "DESC" : "ASC");

        var serializedFilter = JsonConvert.SerializeObject(filters);
        parameters.Add("FilterJson", serializedFilter);


        return parameters;
    }
    private static readonly Dictionary<string, PropertyInfo> DtoProperties =
        typeof(TItem)
            .GetProperties()
            .ToDictionary(p => p.Name);

    private static readonly string StoredProcedureName =
        typeof(TItem).GetCustomAttribute<StoredProcedureAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TItem).Name} does not have StoredProcedureAttribute");

    private static readonly string FullReportExportProcedureName =
        typeof(TDetailItem).GetCustomAttribute<FullReportExportAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have FullReportExportAttribute");

    private static readonly string DetailTableFunctionName =
        typeof(TDetailItem).GetCustomAttribute<TableFunctionAttribute>()?.Name
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have TableFunctionAttribute");

    private static readonly string DetailTableFunctionKeyColumn =
        typeof(TDetailItem).GetCustomAttribute<TableFunctionAttribute>()?.KeyColumn
        ?? throw new InvalidOperationException(
            $"DTO {typeof(TDetailItem).Name} does not have TableFunctionAttribute");


    private static async Task<int> ReadCountAsync(SqlMapper.GridReader multi)
    {
        var result = await multi.ReadFirstOrDefaultAsync<dynamic>();
        return result?.TotalCount ?? 0;
    }
}
