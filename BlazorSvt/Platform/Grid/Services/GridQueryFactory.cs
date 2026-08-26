using System.Reflection;
using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Преобразует запрос BlazorBootstrap Grid в <see cref="GridQuery"/> для SP.
/// </summary>
/// <remarks>
/// Если фильтры ещё не инициализированы компонентом — подставляется
/// <c>IsArchive = False</c> (дефолт UI: только активные записи).
/// Enum-фильтры сериализуются в числовой ItemId для SQL.
/// </remarks>
public class GridQueryFactory<TItem> : IGridQueryFactory<TItem>
{
    private const string IsArchiveFieldName = "IsArchive";

    public GridQuery Create(
        GridDataProviderRequest<TItem> request,
        int? pageNumber = null,
        int? pageSize = null) =>
        new(
            pageNumber ?? request.PageNumber,
            pageSize ?? request.PageSize,
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
                    value = ((int)enumValue).ToString();
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

    private static readonly Dictionary<string, PropertyInfo> DtoProperties =
        typeof(TItem)
            .GetProperties()
            .ToDictionary(p => p.Name);
}
