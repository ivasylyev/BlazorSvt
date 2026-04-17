namespace BlazorSvt.Models.Config;

[AttributeUsage(AttributeTargets.Class)]
public class GridStoredProcedureAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}