namespace BlazorSvt.Models.Grid;

public sealed record GridQuery(
    int PageNumber,
    int PageSize,
    string Lang,
    GridSort Sort,
    IReadOnlyList<GridFilter> Filters);
