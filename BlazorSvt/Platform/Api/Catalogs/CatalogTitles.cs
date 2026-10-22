using System.Globalization;
using System.Resources;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>Заголовок справочника на заданном языке (оба языка нужны в <c>/catalogs</c> независимо от запроса).</summary>
public static class CatalogTitles
{
    private static readonly ResourceManager Resources = new(
        typeof(PlatformResources).FullName!,
        typeof(PlatformResources).Assembly);

    private static readonly CultureInfo Ru = CultureInfo.GetCultureInfo("ru-RU");
    private static readonly CultureInfo En = CultureInfo.GetCultureInfo("en-US");

    public static string GetRu(string key) => Resources.GetString(key, Ru) ?? key;

    public static string GetEn(string key) => Resources.GetString(key, En) ?? key;

    public static string GetCurrent(string key) => Resources.GetString(key, CultureInfo.CurrentUICulture) ?? key;
}
