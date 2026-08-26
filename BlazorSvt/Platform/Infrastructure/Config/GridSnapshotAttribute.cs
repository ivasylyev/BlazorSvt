namespace BlazorSvt.Platform.Infrastructure.Config;

/// <summary>
/// Помечает list-DTO как проекцию snapshot-таблицы read-модели.
/// </summary>
/// <param name="tableName">Полное имя таблицы, напр. <c>v2.TransportRate_Snapshot</c>.</param>
/// <remarks>
/// Имя передаётся в <c>v2.GetBlazorGridData</c> как <c>@TableName</c>.
/// Колонки описываются через <see cref="GridColumnAttribute"/>.
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class GridSnapshotAttribute(string tableName) : Attribute
{
    public string TableName { get; } = tableName;
}
