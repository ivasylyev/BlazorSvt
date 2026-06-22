namespace BlazorSvt.Models.Config;

[AttributeUsage(AttributeTargets.Class)]
public class TableFunctionAttribute(string name, string keyColumn) : Attribute
{
    public string Name { get; } = name;

    public string KeyColumn { get; } = keyColumn;
}
