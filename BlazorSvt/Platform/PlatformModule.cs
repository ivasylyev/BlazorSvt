using BlazorSvt.Platform.Access;
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
        services.Configure<GridOptions>(configuration.GetSection("Grid"));
        services.Configure<AccessOptions>(configuration.GetSection(AccessOptions.SectionName));
        services.AddScoped<PageTimingService>();
        services.AddScoped(typeof(IGridQueryFactory<>), typeof(GridQueryFactory<>));
        services.AddScoped(typeof(IGridDataService<,>), typeof(GridDataService<,>));
        services.AddScoped(typeof(GridReportExporter<,>), typeof(GridReportExporter<,>));
        services.AddScoped<IGridExcelExporter, GridExcelExporter>();
        services.AddScoped<IFileDownloadService, FileDownloadService>();

        services.AddScoped<CurrentUser>();
        services.AddScoped<ICurrentUser>(sp => sp.GetRequiredService<CurrentUser>());
        services.AddScoped<IWindowsIdentityAccessor, HttpContextWindowsIdentityAccessor>();
        services.AddSingleton<IActiveDirectoryClient>(sp =>
        {
            if (!OperatingSystem.IsWindows())
            {
                return new UnsupportedDirectoryClient();
            }

            return new AccountManagementDirectoryClient();
        });
        services.AddScoped<IUserAccessRepository, UserAccessRepository>();
        services.AddScoped<UserAccessSynchronizer>();
        services.AddScoped<IAccessGuard, AccessGuard>();
        services.AddScoped<CircuitHandler, AccessCircuitHandler>();
        services.AddScoped<CircuitHandler, CircuitCorrelationHandler>();

        return services;
    }
}
