namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Helpers for detail-panel display values (empty-field hiding).
/// </summary>
public static class DetailDisplayValue
{
    public static bool HasMeaningfulValue(object? value) => value switch
    {
        null => false,
        string s => !string.IsNullOrWhiteSpace(s),
        _ => true
    };

    public static bool IsVisible<T>(
        DetailSetting<T> setting,
        T detail,
        bool hideEmptyFields) =>
        setting.VisibleSelector(detail)
        && (!hideEmptyFields || HasMeaningfulValue(setting.DisplaySelector(detail)));
}
