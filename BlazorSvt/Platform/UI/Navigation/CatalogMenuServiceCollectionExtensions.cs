using Microsoft.Extensions.DependencyInjection;

namespace BlazorSvt.Platform.UI.Navigation;

public static class CatalogMenuServiceCollectionExtensions
{
    public static IServiceCollection AddCatalogMenu(
        this IServiceCollection services,
        CatalogMenuContribution contribution)
    {
        services.AddSingleton(contribution);
        return services;
    }
}
