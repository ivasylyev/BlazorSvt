namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Какие состояния строк дочернего примитива видны на карточке родителя.
/// </summary>
public static class DetailChildRowStates
{
    public const int Actual = 1;
    public const int Archival = 2;

    private static readonly IReadOnlyList<int> ActualOnly = [Actual];
    private static readonly IReadOnlyList<int> ActualAndArchival = [Actual, Archival];

    public static IReadOnlyList<int> For(bool parentIsArchive) =>
        parentIsArchive ? ActualAndArchival : ActualOnly;
}
