namespace BlazorSvt.Platform.Infrastructure.Config;

public class GridOptions
{
    /// <summary>
    /// When true, detail panel hides fields whose display value is null/empty/whitespace.
    /// Does not affect full Excel report export.
    /// </summary>
    public bool HideEmptyDetailFields { get; set; } = true;
}
