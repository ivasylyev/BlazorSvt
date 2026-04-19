namespace BlazorSvt.Models.Grid;

public class DetailSetting<T>
{
    public required string Name { get; set; }
    public required string Header { get; set; }
    public required Func<T, bool> VisibleSelector { get; set; }
    public required Func<T, object> DisplaySelector { get; set; }

    public override string ToString()
    {
        return $"{Header} ({Name})";
    }
}