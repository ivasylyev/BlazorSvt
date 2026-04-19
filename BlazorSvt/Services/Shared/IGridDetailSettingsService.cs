using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridDetailSettingsService<T>
{
    public GridDetailSettingsCollection<T> GetGridDetailSettings(string lang);
}