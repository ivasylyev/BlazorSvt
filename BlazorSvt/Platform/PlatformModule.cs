using BlazorSvt.Platform.Grid.Services;
using BlazorSvt.Platform.Infrastructure.Config;
using BlazorSvt.Platform.Reporting.Services;

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

        return services;
    }
}
