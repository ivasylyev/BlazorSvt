using BlazorSvt.Platform.Infrastructure.Logging;
using BlazorSvt.Platform.Reporting.Services;
using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorSvt.Platform;

public static class PlatformModule
{
    public static IServiceCollection AddPlatform(this IServiceCollection services, IConfiguration configuration)
    {
        services.Configure<DatabaseOptions>(configuration.GetSection("Database"));
        services.Configure<ReportOptions>(configuration.GetSection("Reports"));
        services.AddScoped<PageTimingService>();
        services.AddScoped(typeof(IGridQueryFactory<>), typeof(GridQueryFactory<>));
        services.AddScoped(typeof(IGridDataService<,>), typeof(GridDataService<,>));
        services.AddScoped<IGridExcelExporter, GridExcelExporter>();
        services.AddScoped<IFileDownloadService, FileDownloadService>();
        services.AddScoped<CircuitHandler, CircuitCorrelationHandler>();

        return services;
    }
}
