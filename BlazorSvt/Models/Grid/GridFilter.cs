namespace BlazorSvt.Models.Grid;

public sealed record GridFilter(
    string PropertyName,
    string? Value,
    string Operator);
