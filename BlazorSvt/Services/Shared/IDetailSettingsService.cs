using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IDetailSettingsService<T>
{
    public DetailSettingsCollection<T> GetGridDetailSettings(string lang);
}