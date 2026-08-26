using BlazorSvt.Modules.TransportLeg.Detail;
using BlazorSvt.Modules.TransportLeg.List;
using BlazorSvt.Modules.TransportLeg.Sync;
using BlazorSvt.Platform.Sync;

namespace BlazorSvt.Modules.TransportLeg;

public static class TransportLegModule
{
    public static IServiceCollection AddTransportLegModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<TransportLegDto>, TransportLegGridSettingsService>();
        services.AddScoped<IDetailSettingsService<TransportLegDetailDto>, TransportLegDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, TransportLegSyncJob>();

        return services;
    }
}
