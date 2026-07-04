namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GridColumnAttribute : Attribute
{
    public GridColumnType? ColumnType { get; init; }
    public string? SqlColumn { get; init; }
    public GridSelectTransform SelectTransform { get; init; } = GridSelectTransform.Auto;
    public string? SelectExpression { get; init; }
    public bool Filterable { get; init; } = true;
    public bool IncludeInSelect { get; init; } = true;
    public bool IsEntityKey { get; init; }
}
