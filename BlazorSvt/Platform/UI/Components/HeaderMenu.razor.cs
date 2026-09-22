using System.Globalization;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.UI.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorSvt.Platform.UI.Components;

public partial class HeaderMenu : SvtComponentBase, IDisposable
{
    [Inject]
    private ICurrentUser CurrentUser { get; set; } = default!;

    [Inject]
    private IEnumerable<CatalogMenuContribution> Contributions { get; set; } = [];

    private IReadOnlyList<MenuDomainGroup> DomainGroups =>
        MenuComposer.Compose(Contributions, key => L[key].Value, CultureInfo.CurrentUICulture);

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged();
    }

    private string GetButtonClass(bool active) =>
        active ? "sibur-dark-inverted-btn" : "sibur-dark-btn";

    private bool IsHomeActive() => IsActive("");

    private bool IsDomainActive(MenuDomainGroup group) =>
        group.Items.Any(item => IsActive(item.Url));

    private bool IsActive(string url)
    {
        var relative = Nav.ToBaseRelativePath(Nav.Uri).Trim('/');
        var target = (url ?? string.Empty).Trim('/');
        return string.Equals(relative, target, StringComparison.OrdinalIgnoreCase);
    }

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
    }
}
