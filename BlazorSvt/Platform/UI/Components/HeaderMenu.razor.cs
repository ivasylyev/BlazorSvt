using BlazorBootstrap;
using BlazorSvt.Platform.Access;
using BlazorSvt.Platform.UI.Navigation;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Routing;

namespace BlazorSvt.Platform.UI.Components;

public partial class HeaderMenu : SvtComponentBase, IDisposable
{
    [Inject]
    private ICurrentUser CurrentUser { get; set; } = default!;
    private List<MenuItem>? menuItems = default;
    private List<MenuItem> GetMenuItems() =>
    [
        new() { Url = "", Text = L["HeaderMenu.Home"] , Icon = IconName.HouseDoorFill },
        new() { Url = "transportrate", Text = L["HeaderMenu.TransportRate"], Icon = IconName.Calculator },
        new() { Url = "averageratelevel3", Text = L["HeaderMenu.AverageRateLevel3"], Icon = IconName.CalculatorFill },
        new() { Url = "parityrates", Text = L["HeaderMenu.ParityRates"], Icon = IconName.Percent },
        new() { Url = "transportleg", Text = L["HeaderMenu.TransportLeg"], Icon = IconName.SignpostSplit },
        new() { Url = "locationsnodes", Text = L["HeaderMenu.LocationsNodes"], Icon = IconName.GeoAltFill },
        //    new() { Url = "load", Text = L["HeaderMenu.Load"], Icon = IconName.Upload }
    ];

    protected override void OnInitialized()
    {
        menuItems = GetMenuItems();
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged(); // перерисовать меню, чтобы обновилась подсветка
    }

    private string GetButtonClass(string url)
    {
        return IsActive(url) ? "sibur-dark-inverted-btn" : "sibur-dark-btn";
    }

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

