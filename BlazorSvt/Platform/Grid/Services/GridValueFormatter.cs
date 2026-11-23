namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Значение свойства DTO для отображения в гриде и карточке по умолчанию.
/// Правило для нового типа данных добавляется отдельной веткой.
/// </summary>
internal static class GridValueFormatter
{
    public static object Format<TProp>(TProp value) => value switch
    {
        null => string.Empty,
        DateTime dateTime => GridDateTime.ToDisplay(dateTime),
        _ => value
    };
}
