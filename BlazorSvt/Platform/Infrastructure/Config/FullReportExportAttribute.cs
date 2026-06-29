namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Class)]
public class FullReportExportAttribute(string name) : Attribute
{
    public string Name { get; } = name;
}
