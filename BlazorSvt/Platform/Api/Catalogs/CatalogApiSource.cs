using System.Globalization;

namespace BlazorSvt.Platform.Api.Catalogs;

/// <summary>
/// Чтение одного справочника без знания его DTO снаружи: <see cref="CatalogApiReader"/> выбирает источник по коду.
/// Регистрируется в <c>AddCatalogApi</c>.
/// </summary>
public interface ICatalogApiSource
{
    CatalogApiDescriptor Descriptor { get; }

    CatalogSchemaResponse GetSchema(IServiceProvider services);

    Task<CatalogSearchResponse<object>> SearchAsync(IServiceProvider services, GridQuery query, CancellationToken cancellationToken);

    Task<object?> FindAsync(IServiceProvider services, long id, CancellationToken cancellationToken);
}

/// <summary>Типизированный источник: те же сервисы грида и карточки, что у Blazor.</summary>
public sealed class CatalogApiSource<TItem, TDetail>(CatalogApiDescriptor descriptor) : ICatalogApiSource
{
    public CatalogApiDescriptor Descriptor => descriptor;

    public CatalogSchemaResponse GetSchema(IServiceProvider services)
    {
        var gridSettings = services.GetRequiredService<IGridSettingsService<TItem>>();
        var detailSettings = services.GetRequiredService<IDetailSettingsService<TDetail>>();

        var listTitles = new Dictionary<string, string>();
        foreach (var lang in LangsCurrentFirst())
        {
            foreach (var column in gridSettings.GetDefaultSettings(lang))
            {
                listTitles.TryAdd(column.Name, column.Header);
            }
        }

        var detailTitles = new Dictionary<string, (string, string?)>();
        foreach (var lang in LangsCurrentFirst())
        {
            foreach (var (groupHeader, settings) in detailSettings.GetGridDetailSettings(lang).GroupSettings)
            {
                foreach (var setting in settings)
                {
                    detailTitles.TryAdd(setting.Name, (setting.Header, groupHeader));
                }
            }
        }

        return CatalogSchemaBuilder.Build(
            descriptor,
            CatalogTitles.GetCurrent(descriptor.TitleResourceKey),
            listTitles,
            detailTitles);
    }

    public async Task<CatalogSearchResponse<object>> SearchAsync(
        IServiceProvider services,
        GridQuery query,
        CancellationToken cancellationToken)
    {
        var dataService = services.GetRequiredService<IGridDataService<TItem, TDetail>>();
        var freshness = services.GetRequiredService<ICatalogFreshnessReader>();

        var page = await dataService.GetPageAsync(query, cancellationToken);
        var dataAsOf = await freshness.GetDataAsOfAsync(descriptor.SyncEntity, cancellationToken);

        var items = page.Items as IReadOnlyList<object> ?? page.Items.Cast<object>().ToList();
        return new CatalogSearchResponse<object>(items, page.TotalCount, dataAsOf);
    }

    public async Task<object?> FindAsync(IServiceProvider services, long id, CancellationToken cancellationToken)
    {
        var dataService = services.GetRequiredService<IGridDataService<TItem, TDetail>>();
        return await dataService.FindDetailAsync(id, cancellationToken);
    }

    private static IEnumerable<string> LangsCurrentFirst()
    {
        var current = CultureInfo.CurrentUICulture.TwoLetterISOLanguageName == "en" ? "en" : "ru";
        yield return current;
        yield return current == "ru" ? "en" : "ru";
    }
}
