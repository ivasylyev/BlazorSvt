using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.UI.Components;

public partial class CultureSelector
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private void ChangeLanguage(string culture)
    {
        var redirectUri = "/" + Navigation.ToBaseRelativePath(Navigation.Uri).Trim('/');
        var query = $"Culture/Set?culture={culture}&redirectUri={Uri.EscapeDataString(redirectUri)}";

        Navigation.NavigateTo(query, forceLoad: true);
    }
}