namespace BlazorBootstrap;

public static class GridFilterUtility
{
    public static bool IsTextPropertyType(string propertyTypeName) =>
        propertyTypeName is StringConstants.PropertyTypeNameString
            or StringConstants.PropertyTypeNameChar;

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
}
