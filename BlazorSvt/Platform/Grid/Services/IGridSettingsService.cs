namespace BlazorSvt.Platform.Grid.Services;

public interface IGridSettingsService<T>
{
    public Task<GridColumnSettingsCollection<T>> GetGridSettingsAsync(string lang);
    public Task SaveGridSettingsAsync(GridColumnSettingsCollection<T> columnSettingsCollection, string lang);
    public Task ResetGridSettingsAsync(string lang);

    /// <summary>Колонки по умолчанию без пользовательских настроек (без LocalStorage/JS).</summary>
    public IReadOnlyList<GridColumnSetting<T>> GetDefaultGridSettings(string lang);
}
