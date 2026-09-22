using System.Globalization;

namespace BlazorSvt.Platform.UI.Navigation;

public sealed record MenuCatalogItem(
    string Url,
    string TextResourceKey,
    string Text,
    bool VisibleToEditor);

public sealed record MenuDomainGroup(
    CatalogDomain Domain,
    string TextResourceKey,
    string HintResourceKey,
    IReadOnlyList<MenuCatalogItem> Items);

public static class MenuComposer
{
    public static IReadOnlyList<MenuDomainGroup> Compose(
        IEnumerable<CatalogMenuContribution> contributions,
        Func<string, string> localize,
        CultureInfo culture)
    {
        var byDomain = contributions
            .GroupBy(contribution => contribution.Domain)
            .ToDictionary(group => group.Key);

        var comparer = StringComparer.Create(culture, ignoreCase: true);
        var groups = new List<MenuDomainGroup>();

        foreach (var domain in CatalogDomains.All)
        {
            if (!byDomain.TryGetValue(domain.Domain, out var domainContributions))
                continue;

            var items = domainContributions
                .Select(contribution => new MenuCatalogItem(
                    contribution.Url,
                    contribution.TextResourceKey,
                    localize(contribution.TextResourceKey),
                    contribution.VisibleToEditor))
                .OrderBy(item => item.Text, comparer)
                .ThenBy(item => item.Url, StringComparer.Ordinal)
                .ToList();

            if (items.Count == 0)
                continue;

            groups.Add(new MenuDomainGroup(
                domain.Domain,
                domain.TextResourceKey,
                domain.HintResourceKey,
                items));
        }

        return groups;
    }
}
