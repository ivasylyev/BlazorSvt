using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridSettingsService<T>
{
    public Task<GridSettings<T>> GetGridSettingsAsync();
    public Task SaveGridSettingsAsync(GridSettings<T> settings);
    public Task ResetGridSettingsAsync();
}