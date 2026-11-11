using BlazorSvt.Modules.SmartRateLookup.Api;
using BlazorSvt.Modules.SmartRateLookup.Data;
using BlazorSvt.Modules.SmartRateLookup.Services;
using BlazorSvt.Platform.Api;
using BlazorSvt.Platform.Api.Mcp;
using Microsoft.Extensions.DependencyInjection.Extensions;

namespace BlazorSvt.Modules.SmartRateLookup;

/// <summary>Умный подбор ставки (API для Атласа). Без UI.</summary>
public static class SmartRateLookupModule
{
    public static IServiceCollection AddSmartRateLookupModule(this IServiceCollection services)
    {
        services.TryAddSingleton(TimeProvider.System);
        services.AddScoped<ISmartRateLookupRepository, SmartRateLookupRepository>();
        services.AddScoped<SmartRateLookupService>();
        services.AddSingleton<IApiEndpointContribution, SmartRateLookupEndpoint>();
        services.AddMcpTool<SmartRateLookupMcpTool>();

        return services;
    }
}
