namespace BlazorSvt.Models.Grid;

public class GridDetailSettingsCollection<T>(List<GridDetailSetting<T>> detailSettings)
{
    public GridDetailSettingsCollection() : this(new List<GridDetailSetting<T>>())
    {
    }

    public List<GridDetailSetting<T>> DetailSettings { get; set; } = detailSettings;

    public IReadOnlyCollection<GridDetailSetting<T>> GetGridDetailSettingsCopy()
    {
        return DetailSettings
            .Select(cs => new GridDetailSetting<T>
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