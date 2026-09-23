namespace BlazorSvt.Platform.UI.Navigation;

/// <summary>
/// Пункт меню справочника. Регистрируется в <c>Add*Module</c>.
/// <see cref="VisibleToEditor"/> соответствует колонке «Меню ЕО»: до MVP 0.4 шапка по нему не фильтрует.
/// </summary>
public sealed record CatalogMenuContribution(
    CatalogDomain Domain,
    string Url,
    string TextResourceKey,
    bool VisibleToEditor);
