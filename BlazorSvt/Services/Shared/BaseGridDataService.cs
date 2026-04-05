using System.Data;
using System.Data.SqlClient;
using BlazorBootstrap;
using BlazorSvt.Models.Config;
using BlazorSvt.Utils;
using Dapper;
using Microsoft.Extensions.Options;
using Newtonsoft.Json;

namespace BlazorSvt.Services.Shared;

public abstract class BaseGridDataService<TItem>(
    IOptions<DatabaseOptions> options,
    ILogger logger)
{
    private readonly string connectionString = options.Value.MdmDb;

    protected abstract string StoredProcedureName { get; }

    protected virtual string IsArchiveFieldName => "IsArchive";

    protected async Task<GridDataProviderResult<TItem>> GetDataAsync(GridDataProviderRequest<TItem> request, string lang)
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
            request.CancellationToken);

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
        var filters = new List<FilterItem>();

        // Бывает что request.Filters пустой из-за того что компонент не успел инициализироваться
        // ReSharper disable once ConditionIsAlwaysTrueOrFalseAccordingToNullableAPIContract
        if (request.Filters is not null)
            filters.AddRange(request.Filters.Select(ExtractEnumFilter<TItem>));
        else
            filters.Add(new FilterItem(IsArchiveFieldName, "False", FilterOperator.Equals, StringComparison.InvariantCultureIgnoreCase));
        return filters;
    }

    private FilterItem ExtractEnumFilter<TItem>(FilterItem filter)
    {
        var propInfo = typeof(TItem).GetProperty(filter.PropertyName);
        if (propInfo != null)
        {
            // Проверяем, является ли тип перечислением (учитываем Nullable<Enum>)
            var propType = Nullable.GetUnderlyingType(propInfo.PropertyType) ?? propInfo.PropertyType;

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
        CancellationToken cancellationToken)
    {
        await using var connection = new SqlConnection(connectionString);
        await connection.OpenAsync(cancellationToken);

        var parameters = BuildParameters(filters, pageNumber, pageSize, sortKey, sortDirection, lang);

        var loggingConnection = new DbConnectionLogDecorator(connection, logger);

        await using var multi = await loggingConnection.QueryMultipleAsync(
            StoredProcedureName,
            parameters,
            CommandType.StoredProcedure);

        var data = (await multi.ReadAsync<TItem>()).ToList();
        var count = await ReadCountAsync(multi);

        return (data, count);
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

    protected virtual async Task<int> ReadCountAsync(SqlMapper.GridReader multi)
    {
        var result = await multi.ReadFirstOrDefaultAsync<dynamic>();
        return result?.TotalCount ?? 0;
    }
}