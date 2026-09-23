namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// One page size for every catalog grid. Stored in the browser, not per language and not per catalog.
/// </summary>
public static class GridPageSizes
{
    public const string StorageKey = "GridPageSize";

    public const int Default = 10;

    public static readonly int[] Allowed = { 10, 15, 20, 30, 50 };

    public static int Normalize(int? pageSize) =>
        pageSize is int value && Array.IndexOf(Allowed, value) >= 0 ? value : Default;
}
