using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridSettingsService<T>
{
    public Task<GridColumnSettingsCollection<T>> GetGridSettingsAsync(string lang);
    public Task SaveGridSettingsAsync(GridColumnSettingsCollection<T> columnSettingsCollection, string lang);
    public Task ResetGridSettingsAsync(string lang);
}