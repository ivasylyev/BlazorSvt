namespace BlazorSvt.Models.Config;

[AttributeUsage(AttributeTargets.Class)]
public class StoredProcedureAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}