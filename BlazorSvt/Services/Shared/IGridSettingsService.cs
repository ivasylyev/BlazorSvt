using BlazorSvt.Models.Grid;
using BlazorSvt.Models.Page;

namespace BlazorSvt.Services.Shared;

public interface IGridSettingsService<T>
{
    public PageSettings GetPageSettings();
    public Task<GridSettings<T>> GetGridSettingsAsync();
    public Task SaveGridSettingsAsync(GridSettings<T> settings);
    public Task ResetGridSettingsAsync();
    
}