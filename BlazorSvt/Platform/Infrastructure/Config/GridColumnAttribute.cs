using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Property)]
public sealed class GridColumnAttribute(GridColumnType columnType) : Attribute
{
    public GridColumnType ColumnType { get; } = columnType;
    public string? SqlColumn { get; init; }
    public GridSelectTransform SelectTransform { get; init; } = GridSelectTransform.Auto;
    public string? SelectExpression { get; init; }
    public bool Filterable { get; init; } = true;
    public bool IncludeInSelect { get; init; } = true;
    public bool IsEntityKey { get; init; }
    public int Order { get; init; }
}
