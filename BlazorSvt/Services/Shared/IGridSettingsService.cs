using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridSettingsService<T>
{
    public Task<GridSettings<T>> GetGridSettingsAsync(string lang);
    public Task SaveGridSettingsAsync(GridSettings<T> settings, string lang);
    public Task ResetGridSettingsAsync(string lang);
}