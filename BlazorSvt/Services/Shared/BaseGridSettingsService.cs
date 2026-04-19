using Blazored.LocalStorage;
using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public abstract class BaseGridSettingsService<T>(
    ILocalStorageService localStorage,
    ILogger logger)
    : IGridSettingsService<T>
{
    protected readonly ILocalStorageService LocalStorage = localStorage;

    protected abstract string StorageKey { get; }

    public async Task<GridColumnSettingsCollection<T>> GetGridSettingsAsync(string lang)
    {
        var defaultSettings = GetDefaultSettings(lang);

        Dictionary<string, bool> loaded;
        try
        {
            loaded = await LocalStorage
                         .GetItemAsync<Dictionary<string, bool>>($"{StorageKey}_{lang}")
                     ?? new Dictionary<string, bool>();
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not load from settings from LocalStorage");
            loaded = new Dictionary<string, bool>();
        }

        foreach (var column in defaultSettings)
        {
            if (loaded.TryGetValue(column.Name, out var visible))
            {
                column.Visible = visible;
            }
        }

        return new GridColumnSettingsCollection<T>(defaultSettings);
    }

    public async Task SaveGridSettingsAsync(GridColumnSettingsCollection<T> columnSettingsCollection, string lang)
    {
        var visibility = columnSettingsCollection.ColumnSettings
            .ToDictionary(x => x.Name, x => x.Visible);

        await LocalStorage.SetItemAsync($"{StorageKey}_{lang}", visibility);
    }

    public async Task ResetGridSettingsAsync(string lang)
    {
        await LocalStorage.RemoveItemAsync($"{StorageKey}_{lang}");
    }


    protected abstract List<GridColumnSetting<T>> GetDefaultSettings(string lang);
}