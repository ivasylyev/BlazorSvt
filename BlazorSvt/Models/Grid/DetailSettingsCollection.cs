namespace BlazorSvt.Models.Grid;

public class DetailSettingsCollection<T>(List<DetailSetting<T>> detailSettings)
{
    public DetailSettingsCollection() : this(new List<DetailSetting<T>>())
    {
    }

    public List<DetailSetting<T>> DetailSettings { get; set; } = detailSettings;

    public IReadOnlyCollection<DetailSetting<T>> GetGridDetailSettingsCopy()
    {
        return DetailSettings
            .Select(cs => new DetailSetting<T>
            {
                Name = cs.Name,
                Header = cs.Header,
                VisibleSelector = cs.VisibleSelector,
                DisplaySelector = cs.DisplaySelector
            })
            .ToList()
            .AsReadOnly();
    }
}