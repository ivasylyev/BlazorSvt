using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Фильтрация, сортировка и страница по уже загруженным строкам дочернего примитива — на клиенте,
/// тем же <c>GridDataProviderRequest.ApplyTo</c>, что и <c>Grid</c> с <c>Data</c>.
/// Значения фильтров <c>DateTime</c> приводятся так же, как для SQL-фильтра грида справочника
/// (<see cref="GridDateTime.NormalizeFilterValue"/>).
/// </summary>
public static class DetailChildGridData
{
    public static GridDataProviderResult<TRow> Apply<TRow>(
        IEnumerable<TRow> rows,
        GridDataProviderRequest<TRow> request) =>
        NormalizeDateFilters(request).ApplyTo(rows);

    private static GridDataProviderRequest<TRow> NormalizeDateFilters<TRow>(GridDataProviderRequest<TRow> request)
    {
        // Фильтры не заданы или не поменяются: исходный запрос.
        if (request.Filters is null || !request.Filters.Any(IsDateTimeFilter<TRow>))
            return request;

        return new GridDataProviderRequest<TRow>
        {
            PageNumber = request.PageNumber,
            PageSize = request.PageSize,
            Sorting = request.Sorting,
            CancellationToken = request.CancellationToken,
            Filters = request.Filters
                .Select(filter => IsDateTimeFilter<TRow>(filter)
                    ? new FilterItem(
                        filter.PropertyName,
                        GridDateTime.NormalizeFilterValue(filter.Value),
                        filter.Operator,
                        filter.StringComparison)
                    : filter)
                .ToList()
        };
    }

    private static bool IsDateTimeFilter<TRow>(FilterItem filter)
    {
        if (filter.Value is null)
            return false;

        var property = typeof(TRow).GetProperty(filter.PropertyName);
        return property is not null
               && (Nullable.GetUnderlyingType(property.PropertyType) ?? property.PropertyType) == typeof(DateTime);
    }
}
