namespace BlazorSvt.Platform.Grid.Models;

public sealed record GridFilter(
    string PropertyName,
    string? Value,
    string Operator);
