namespace BlazorSvt.Platform.Grid.Models;

public interface IGridColumnSetting
{
    public string Name { get; set; }
    public string Header { get; set; }
    public bool Visible { get; set; }
    public bool Filterable { get; set; }
}