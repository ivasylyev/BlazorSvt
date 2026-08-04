namespace BlazorSvt.Platform.Grid.Models;

public sealed record GridQuery(
    int PageNumber,
    int PageSize,
    GridSort Sort,
    IReadOnlyList<GridFilter> Filters);
