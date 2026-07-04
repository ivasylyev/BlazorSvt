namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Class)]
public class GridSnapshotAttribute(string tableName) : Attribute
{
    public string TableName { get; } = tableName;
}
