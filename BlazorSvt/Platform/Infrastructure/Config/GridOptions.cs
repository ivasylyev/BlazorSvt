namespace BlazorSvt.Platform.Infrastructure.Config;

public class GridOptions
{
    /// <summary>
    /// When true, detail panel hides fields whose display value is null/empty/whitespace.
    /// Does not affect full Excel report export.
    /// </summary>
    public bool HideEmptyDetailFields { get; set; } = true;

    /// <summary>
    /// Max rows shown in a child primitive table on the detail panel.
    /// </summary>
    public int ChildSectionRowLimit { get; set; } = 100;
}
