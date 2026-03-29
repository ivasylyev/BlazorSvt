using Blazored.LocalStorage;
using BlazorSvt.Models.Grid;
using BlazorSvt.Models.Page;

namespace BlazorSvt.Services.Shared;

public abstract class BaseGridSettingsService<T>(ILocalStorageService localStorage) : IGridSettingsService<T>
{
    protected readonly ILocalStorageService LocalStorage = localStorage;

    protected abstract string StorageKey { get; }

    public abstract PageSettings GetPageSettings();


    public async Task<GridSettings<T>> GetGridSettingsAsync()
    {
        var defaultSettings = GetDefaultSettings();

        var loaded = await LocalStorage
                         .GetItemAsync<Dictionary<string, bool>>(StorageKey)
                     ?? new Dictionary<string, bool>();

        foreach (var column in defaultSettings)
            if (loaded.TryGetValue(column.Name, out var visible))
                column.Visible = visible;

        return new GridSettings<T>(defaultSettings);
    }

    public async Task SaveGridSettingsAsync(GridSettings<T> settings)
    {
        var visibility = settings.ColumnSettings
            .ToDictionary(x => x.Name, x => x.Visible);

        await LocalStorage.SetItemAsync(StorageKey, visibility);
    }

    public async Task ResetGridSettingsAsync()
    {
        await LocalStorage.RemoveItemAsync(StorageKey);
    }


    /// <summary>
    ///     Должен вернуть полный набор колонок (дефолт)
    /// </summary>
    protected abstract List<GridColumnSetting<T>> GetDefaultSettings();
}