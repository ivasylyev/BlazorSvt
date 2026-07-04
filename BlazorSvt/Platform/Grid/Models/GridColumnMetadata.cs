namespace BlazorSvt.Platform.Grid.Models;

public sealed class GridColumnMetadata
{
    public required string PropertyName { get; init; }
    public required string SqlColumnName { get; init; }
    public required GridColumnType ColumnType { get; init; }
    public required string SelectExpression { get; init; }
    public bool Filterable { get; init; }
    public bool IncludeInSelect { get; init; }
    public bool IsEntityKey { get; init; }
    public int Order { get; init; }
}

public sealed class GridSnapshotMetadata
{
    public required string TableName { get; init; }
    public required IReadOnlyList<GridColumnMetadata> Columns { get; init; }
    public string? EntityKeyPropertyName { get; init; }
}
