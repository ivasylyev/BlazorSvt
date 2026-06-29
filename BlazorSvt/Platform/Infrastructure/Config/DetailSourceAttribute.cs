namespace BlazorSvt.Platform.Infrastructure.Config;

[AttributeUsage(AttributeTargets.Class)]
public class DetailSourceAttribute(string name, string keyColumn) : Attribute
{
    public string Name { get; } = name;

    public string KeyColumn { get; } = keyColumn;
}
