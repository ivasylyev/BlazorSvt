namespace BlazorSvt.Platform.Infrastructure.Config;

/// <summary>
/// Помечает DTO строки дочернего примитива источником таблицы на карточке.
/// </summary>
/// <param name="name">Имя view, напр. <c>v2.vw_CoefficientConditionValue_Detail</c>.</param>
/// <param name="parentKeyColumn">Колонка ключа родителя во view.</param>
/// <remarks>
/// View обязан отдавать <c>Id</c> (порядок строк) и <c>PrimitiveEntityDataStateId</c> (отбор по состоянию).
/// </remarks>
[AttributeUsage(AttributeTargets.Class)]
public class DetailChildSourceAttribute(string name, string parentKeyColumn) : Attribute
{
    public string Name { get; } = name;

    public string ParentKeyColumn { get; } = parentKeyColumn;
}
