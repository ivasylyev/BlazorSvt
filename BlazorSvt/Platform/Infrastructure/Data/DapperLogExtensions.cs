using Dapper;

namespace BlazorSvt.Platform.Infrastructure.Data;

public static class DapperLogExtensions
{
    public static Dictionary<string, object?> ToDictionary(this DynamicParameters parameters)
    {
        var dict = new Dictionary<string, object?>();

        foreach (var name in parameters.ParameterNames)
        {
            dict[name] = parameters.Get<object?>(name);
        }

        return dict;
    }
}