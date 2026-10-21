using BlazorSvt.Platform.UI.Navigation;

namespace BlazorSvt.Platform.Api.Catalogs;

public static class CatalogApiServiceCollectionExtensions
{
    /// <summary>
    /// Публикует справочник в API (<c>/api/v1/catalogs/{code}/…</c>). Вызывается из <c>Add{Name}Module</c>
    /// рядом с <c>AddCatalogMenu</c>: всё, что есть в СВТ 2.0, доступно и через API.
    /// </summary>
    /// <param name="code">Код в URL: kebab-case, мн. число, напр. <c>transport-rates</c>.</param>
    /// <param name="domain">Домен (как в меню).</param>
    /// <param name="titleResourceKey">Ключ заголовка в Platform.resx (как у меню).</param>
    /// <param name="period">Поля периода для <c>asOf</c>; <c>null</c> — без периода.</param>
    /// <param name="syncEntity">Значение <c>v2.SyncState.Entity</c>; по умолчанию — имя list-DTO без суффикса <c>Dto</c>.</param>
    public static IServiceCollection AddCatalogApi<TItem, TDetail>(
        this IServiceCollection services,
        string code,
        CatalogDomain domain,
        string titleResourceKey,
        CatalogPeriod? period = null,
        string? syncEntity = null)
    {
        var descriptor = new CatalogApiDescriptor(
            code,
            domain,
            titleResourceKey,
            typeof(TItem),
            typeof(TDetail),
            syncEntity ?? DefaultSyncEntity(typeof(TItem)),
            period);

        services.AddSingleton(descriptor);
        services.AddSingleton<IApiEndpointContribution>(new CatalogApiEndpoints<TItem, TDetail>(descriptor));
        return services;
    }

    private static string DefaultSyncEntity(Type itemType) =>
        itemType.Name.EndsWith("Dto", StringComparison.Ordinal) ? itemType.Name[..^3] : itemType.Name;
}
