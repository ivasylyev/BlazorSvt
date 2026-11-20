using BlazorSvt.Modules.ReportConstructor.Detail;
using BlazorSvt.Modules.ReportConstructor.List;
using BlazorSvt.Modules.ReportConstructor.Sync;
using BlazorSvt.Platform.Api.Catalogs;
using BlazorSvt.Platform.Sync;
using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Modules.ReportConstructor;

public static class ReportConstructorModule
{
    public static IServiceCollection AddReportConstructorModule(this IServiceCollection services)
    {
        services.AddScoped<IGridSettingsService<ReportConstructorDto>, ReportConstructorGridSettingsService>();
        services.AddScoped<IDetailSettingsService<ReportConstructorDetailDto>, ReportConstructorDetailSettingsService>();

        services.AddSingleton<ISnapshotSyncJob, ReportConstructorSyncJob>();
        services.AddCatalogMenu(new CatalogMenuContribution(
            CatalogDomain.Rates,
            "reportconstructor",
            "HeaderMenu.ReportConstructor",
            VisibleToEditor: true));
        services.AddCatalogApi<ReportConstructorDto, ReportConstructorDetailDto>(
            "report-constructors",
            CatalogDomain.Rates,
            "HeaderMenu.ReportConstructor",
            new CatalogPeriod(nameof(ReportConstructorDto.DateFrom), nameof(ReportConstructorDto.DateTo)));

        return services;
    }
}
