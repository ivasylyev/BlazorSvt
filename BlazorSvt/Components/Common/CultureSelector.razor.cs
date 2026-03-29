using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;

public partial class CultureSelector
{
    [Inject] private NavigationManager Navigation { get; set; } = default!;

    private void ChangeLanguage(string culture)
    {
        var uri = new Uri(Navigation.Uri).GetComponents(UriComponents.PathAndQuery, UriFormat.Unescaped);
        var query = $"/Culture/Set?culture={culture}&redirectUri={Uri.EscapeDataString(uri)}";

        Navigation.NavigateTo(query, true);
    }
}