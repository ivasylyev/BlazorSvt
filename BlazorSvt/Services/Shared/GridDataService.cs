using BlazorBootstrap;
using BlazorSvt.Models.Config;
using BlazorSvt.Models.Grid;
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
        var query = CreateGridQuery(request, lang, pageNumber: 1, pageSize: totalCount);

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
        var query = CreateGridQuery(request, lang, pageNumber: 1, pageSize: totalCount);

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
        var query = CreateGridQuery(request, lang);

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

    private static GridQuery CreateGridQuery(
        GridDataProviderRequest<TItem> request,
        string lang,
        int? pageNumber = null,
        int? pageSize = null) =>
        new(
            pageNumber ?? request.PageNumber,
            pageSize ?? request.PageSize,
            lang,
            ExtractSorting(request),
            ExtractFilters(request));

    private static GridSort ExtractSorting(GridDataProviderRequest<TItem> request)
    {
        // Бывает что request.Sorting пустой из-за того что компонент не успел инициализироваться
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request.Sorting is not null && request.Sorting.Any())
        {
            var sort = request.Sorting.First();
            return new GridSort(sort.SortString, ToSqlSortDirection(sort.SortDirection));
        }

        return new GridSort(null, "ASC");
    }

    private static List<GridFilter> ExtractFilters(GridDataProviderRequest<TItem> request)
    {
        // Бывает что request.Filters пустой из-за того что компонент не успел инициализироваться
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request.Filters is not null)
            return request.Filters.Select(ExtractEnumFilter).ToList();

        return
        [
            new GridFilter(
                IsArchiveFieldName,
                "False",
                GridFilterOperators.EqualsOperator)
        ];
    }

    private static GridFilter ExtractEnumFilter(FilterItem filter)
    {
        var value = filter.Value;
        DtoProperties.TryGetValue(filter.PropertyName, out var propInfo);
        if (propInfo != null)
        {
            // Проверяем, является ли тип перечислением (учитываем Nullable<Enum>)
            var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;
            // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
            if (propType.IsEnum && value != null)
            {
                if (Enum.TryParse(propType, value, out var enumValue))
                {
                    value = ((int)enumValue).ToString();
                }
            }
        }

        return new GridFilter(
            filter.PropertyName,
            value,
            ToGridFilterOperator(filter.Operator));
    }

    private static string ToGridFilterOperator(FilterOperator filterOperator) =>
        filterOperator switch
        {
            FilterOperator.Contains => GridFilterOperators.ContainsOperator,
            FilterOperator.DoesNotContain => GridFilterOperators.DoesNotContainOperator,
            FilterOperator.StartsWith => GridFilterOperators.StartsWithOperator,
            FilterOperator.EndsWith => GridFilterOperators.EndsWithOperator,
            FilterOperator.Equals => GridFilterOperators.EqualsOperator,
            FilterOperator.NotEquals => GridFilterOperators.NotEqualsOperator,
            FilterOperator.LessThan => GridFilterOperators.LessThanOperator,
            FilterOperator.LessThanOrEquals => GridFilterOperators.LessThanOrEqualsOperator,
            FilterOperator.GreaterThan => GridFilterOperators.GreaterThanOperator,
            FilterOperator.GreaterThanOrEquals => GridFilterOperators.GreaterThanOrEqualsOperator,
            FilterOperator.Clear => GridFilterOperators.ClearOperator,
            _ => GridFilterOperators.EqualsOperator
        };

    private static string ToSqlSortDirection(SortDirection sortDirection) =>
        sortDirection == SortDirection.Descending ? "DESC" : "ASC";

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
