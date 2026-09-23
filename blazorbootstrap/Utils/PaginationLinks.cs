namespace BlazorBootstrap;

/// <summary>
/// Page-number window and ±N jumps for <see cref="Pagination"/>.
/// </summary>
public static class PaginationLinks
{
    public const int DefaultWindowSize = 5;

    public static readonly int[] JumpSteps = { 5, 20, 100, 1000 };

    /// <summary>
    /// Up to <paramref name="windowSize"/> page numbers centered on the active page.
    /// Near either end the window slides so it stays inside 1…<paramref name="totalPages"/>.
    /// When the active page is past the last page, the window anchors on the last page.
    /// </summary>
    public static IReadOnlyList<int> GetPageNumbers(int activePageNumber, int totalPages, int windowSize = DefaultWindowSize)
    {
        if (totalPages < 1 || windowSize < 1)
            return Array.Empty<int>();

        var anchor = activePageNumber;
        if (anchor < 1)
            anchor = 1;
        else if (anchor > totalPages)
            anchor = totalPages;

        var radius = windowSize / 2;
        var start = anchor - radius;
        var end = start + windowSize - 1;

        if (end > totalPages)
        {
            start -= end - totalPages;
            end = totalPages;
        }

        if (start < 1)
        {
            end += 1 - start;
            if (end > totalPages)
                end = totalPages;
            start = 1;
        }

        var pages = new List<int>(end - start + 1);
        for (var page = start; page <= end; page++)
            pages.Add(page);

        return pages;
    }

    /// <summary>
    /// Jumps before the page numbers, largest step first: −1000, −100, −20, −5.
    /// A step is omitted when the target page is outside 1…<paramref name="totalPages"/>.
    /// The active page is not clamped: a stale page past the end produces no jumps.
    /// </summary>
    public static IReadOnlyList<PageJump> GetBackwardJumps(int activePageNumber, int totalPages) =>
        GetJumps(activePageNumber, totalPages, backward: true);

    /// <summary>
    /// Jumps after the page numbers: +5, +20, +100, +1000.
    /// A step is omitted when the target page is outside 1…<paramref name="totalPages"/>.
    /// </summary>
    public static IReadOnlyList<PageJump> GetForwardJumps(int activePageNumber, int totalPages) =>
        GetJumps(activePageNumber, totalPages, backward: false);

    private static IReadOnlyList<PageJump> GetJumps(int activePageNumber, int totalPages, bool backward)
    {
        var jumps = new List<PageJump>(JumpSteps.Length);
        if (backward)
        {
            for (var index = JumpSteps.Length - 1; index >= 0; index--)
                AddJump(jumps, activePageNumber, totalPages, -JumpSteps[index]);
        }
        else
        {
            for (var index = 0; index < JumpSteps.Length; index++)
                AddJump(jumps, activePageNumber, totalPages, JumpSteps[index]);
        }

        return jumps;
    }

    private static void AddJump(List<PageJump> jumps, int activePageNumber, int totalPages, int delta)
    {
        var target = GetJumpTarget(activePageNumber, delta, totalPages);
        if (target is not int page)
            return;

        var sign = delta < 0 ? "-" : "+";
        var step = delta < 0 ? -delta : delta;
        jumps.Add(new PageJump(sign + step.ToString(System.Globalization.CultureInfo.InvariantCulture), page));
    }

    public static int? GetJumpTarget(int activePageNumber, int delta, int totalPages)
    {
        if (totalPages < 1 || delta == 0)
            return null;

        var target = (long)activePageNumber + delta;
        if (target < 1 || target > totalPages)
            return null;

        return (int)target;
    }
}

public readonly record struct PageJump(string Label, int Page);
