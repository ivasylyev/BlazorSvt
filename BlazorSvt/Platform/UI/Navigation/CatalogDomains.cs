namespace BlazorSvt.Platform.UI.Navigation;

public sealed record CatalogDomainDefinition(
    CatalogDomain Domain,
    string TextResourceKey,
    string HintResourceKey);

public static class CatalogDomains
{
    public static IReadOnlyList<CatalogDomainDefinition> All { get; } =
    [
        new(CatalogDomain.Rates, "HeaderMenu.Domain.Rates", "HeaderMenu.Domain.Rates.Hint"),
        new(CatalogDomain.Routes, "HeaderMenu.Domain.Routes", "HeaderMenu.Domain.Routes.Hint"),
        new(CatalogDomain.Products, "HeaderMenu.Domain.Products", "HeaderMenu.Domain.Products.Hint"),
        new(CatalogDomain.Warehouses, "HeaderMenu.Domain.Warehouses", "HeaderMenu.Domain.Warehouses.Hint"),
        new(CatalogDomain.MK, "HeaderMenu.Domain.MK", "HeaderMenu.Domain.MK.Hint"),
        new(CatalogDomain.Cbd, "HeaderMenu.Domain.Cbd", "HeaderMenu.Domain.Cbd.Hint"),
        new(CatalogDomain.Other, "HeaderMenu.Domain.Other", "HeaderMenu.Domain.Other.Hint")
    ];
}
