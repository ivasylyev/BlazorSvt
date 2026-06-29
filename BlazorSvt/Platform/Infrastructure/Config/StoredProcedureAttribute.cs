namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Class)]
public class StoredProcedureAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}