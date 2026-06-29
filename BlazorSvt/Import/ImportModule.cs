using BlazorSvt.Import.Services;
using BlazorSvt.Import.Validators;
using FluentValidation;

namespace BlazorSvt.Import;

public static class ImportModule
{
    public static IServiceCollection AddImportModule(this IServiceCollection services)
    {
        services.AddScoped<IExcelParser, ExcelParser>();
        services.AddScoped<IExcelErrorWriter, ExcelErrorWriter>();
        services.AddScoped(typeof(IStagingRepository<>), typeof(StagingRepository<>));
        services.AddValidatorsFromAssemblyContaining<LegStgProxyDtoValidator>();

        return services;
    }
}
