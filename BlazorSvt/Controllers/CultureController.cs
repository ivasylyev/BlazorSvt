using Microsoft.AspNetCore.Localization;
using Microsoft.AspNetCore.Mvc;

namespace BlazorSvt.Controllers;

[Route("[controller]/[action]")]
public class CultureController : Controller
{
    public IActionResult Set(string culture, string redirectUri)
    {
        if (culture is not null)
            HttpContext.Response.Cookies.Append(
                CookieRequestCultureProvider.DefaultCookieName,
                CookieRequestCultureProvider.MakeCookieValue(new RequestCulture(culture)),
                new CookieOptions { Expires = DateTimeOffset.UtcNow.AddYears(1) }
            );

        redirectUri = NormalizeAppRelativePath(redirectUri);

        return LocalRedirect(Url.Content($"~{redirectUri}")!);
    }

    private string NormalizeAppRelativePath(string path)
    {
        var pathBase = Request.PathBase.Value ?? string.Empty;

        if (!string.IsNullOrEmpty(pathBase) &&
            path.StartsWith(pathBase, StringComparison.OrdinalIgnoreCase))
        {
            path = path[pathBase.Length..];
        }

        if (string.IsNullOrEmpty(path) || !path.StartsWith('/'))
            path = "/";

        return path;
    }
}
