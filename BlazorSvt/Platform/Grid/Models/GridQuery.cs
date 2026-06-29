namespace BlazorSvt.Platform.Grid.Models;

public sealed record GridQuery(
    int PageNumber,
    int PageSize,
    string Lang,
    GridSort Sort,
    IReadOnlyList<GridFilter> Filters);
