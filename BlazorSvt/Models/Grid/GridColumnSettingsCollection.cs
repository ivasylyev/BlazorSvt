namespace BlazorSvt.Models.Grid;

public class GridColumnSettingsCollection<T>(List<GridColumnSetting<T>> columnSettings)
{
    public GridColumnSettingsCollection() : this(new List<GridColumnSetting<T>>())
    {
    }

    public List<GridColumnSetting<T>> ColumnSettings { get; set; } = columnSettings;

    public IReadOnlyCollection<IGridColumnSetting> GetGridColumnSettingsCopy()
    {
        return ColumnSettings
            .Select(cs => new GridColumnSetting<T>
            {
                Name = cs.Name,
                Header = cs.Header,
                Visible = cs.Visible,
                Filterable = cs.Filterable,

                // намеренно не копируем:
                DisplaySelector = default!,
                SortSelector = default!
            })
            .Cast<IGridColumnSetting>()
            .ToList()
            .AsReadOnly();
    }

    public void ApplyGridColumnSettings(IReadOnlyCollection<IGridColumnSetting> settings)
    {
        var map = settings.ToDictionary(s => s.Name);

        foreach (var column in ColumnSettings)
        {
            if (!map.TryGetValue(column.Name, out var incoming))
                continue;

            column.Header = incoming.Header;
            column.Visible = incoming.Visible;
            column.Filterable = incoming.Filterable;
        }
    }
}