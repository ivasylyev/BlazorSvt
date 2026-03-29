using BlazorBootstrap;
using Microsoft.AspNetCore.Components.Routing;
using BlazorSvt.Models.Navigation;

namespace BlazorSvt.Components.Common;

public partial class HeaderMenu
{
    private List<MenuItem> MenuItems =
    [
        new() { Url = "/", Text = "Домашняя", Icon = IconName.HouseDoorFill },
        new() { Url = "/rates", Text = "Ставки", Icon = IconName.Table }
    ];

    protected override void OnInitialized()
    {
        Nav.LocationChanged += OnLocationChanged;
    }

    private void OnLocationChanged(object? sender, LocationChangedEventArgs e)
    {
        StateHasChanged(); // перерисовать меню, чтобы обновилась подсветка
    }

    private bool IsActive(string url)
    {
         var relative = "/" + Nav.ToBaseRelativePath(Nav.Uri).Trim('/');
         return relative == url;
    }

    public void Dispose()
    {
        Nav.LocationChanged -= OnLocationChanged;
    }
}