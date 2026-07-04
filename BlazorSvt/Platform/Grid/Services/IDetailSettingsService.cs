namespace BlazorSvt.Platform.Grid.Services;

public interface IDetailSettingsService<T>
{
    public DetailSettingsCollection<T> GetGridDetailSettings(string lang);
}