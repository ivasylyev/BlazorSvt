using System.Globalization;

namespace BlazorBootstrap;

public static class GridFilterUtility
{
    /// <summary>
    /// Inclusive lower bound of SQL Server <c>DATETIME</c> (snapshot date columns).
    /// </summary>
    public static readonly DateTime SqlDateTimeMin = new(1753, 1, 1);

    /// <summary>
    /// Inclusive upper bound of SQL Server <c>DATETIME</c>.
    /// </summary>
    public static readonly DateTime SqlDateTimeMax = new(9999, 12, 31, 23, 59, 59, 997);

    public static bool IsTextPropertyType(string propertyTypeName) =>
        propertyTypeName is StringConstants.PropertyTypeNameString
            or StringConstants.PropertyTypeNameChar;

    public static bool IsDatePropertyType(string propertyTypeName) =>
        propertyTypeName is StringConstants.PropertyTypeNameDateOnly
            or StringConstants.PropertyTypeNameDateTime;

    /// <summary>
    /// Returns true when a text filter value is non-empty but shorter than the minimum length required for querying.
    /// </summary>
    public static bool IsPendingShortTextFilter(string propertyTypeName, string? filterValue, int minTextFilterLength)
    {
        if (minTextFilterLength <= 0 || !IsTextPropertyType(propertyTypeName))
            return false;

        if (string.IsNullOrWhiteSpace(filterValue))
            return false;

        var length = filterValue.Trim().Length;
        return length > 0 && length < minTextFilterLength;
    }

    /// <summary>
    /// Returns true when a date/datetime filter value is non-empty but not yet queryable:
    /// unparseable, or outside the SQL Server <c>DATETIME</c> range (1753-01-01 .. 9999-12-31).
    /// Covers intermediate browser values while typing a year (e.g. <c>0002-09-30</c>).
    /// </summary>
    public static bool IsPendingDateFilter(string propertyTypeName, string? filterValue)
    {
        if (!IsDatePropertyType(propertyTypeName))
            return false;

        if (string.IsNullOrWhiteSpace(filterValue))
            return false;

        if (!TryParseFilterDateTime(filterValue, out var parsed))
            return true;

        return parsed < SqlDateTimeMin || parsed > SqlDateTimeMax;
    }

    /// <summary>
    /// Returns true when the filters row should be rendered with auto-hide enabled:
    /// a filter value is set on a visible column, the item count is not known yet, or items do not fit on one page.
    /// </summary>
    public static bool ShouldShowFiltersRow(bool hasVisibleColumnFilters, int? totalCount, int pageSize) =>
        hasVisibleColumnFilters || !totalCount.HasValue || totalCount.Value > pageSize;

    /// <summary>
    /// Форматы «дата + час» — свободный TryParse их не читает, а UI-фильтр такие шлёт (точность «час»).
    /// </summary>
    private static readonly string[] HourFormats = { "yyyy-MM-dd'T'HH", "yyyy-MM-dd HH" };

    private static bool TryParseFilterDateTime(string filterValue, out DateTime parsed)
    {
        if (DateTime.TryParseExact(
                filterValue,
                HourFormats,
                CultureInfo.InvariantCulture,
                DateTimeStyles.None,
                out parsed))
            return true;

        if (DateTime.TryParse(
                filterValue,
                CultureInfo.InvariantCulture,
                DateTimeStyles.AllowWhiteSpaces | DateTimeStyles.RoundtripKind,
                out parsed))
            return true;

        return DateTime.TryParse(
            filterValue,
            CultureInfo.CurrentCulture,
            DateTimeStyles.AllowWhiteSpaces,
            out parsed);
    }
}
