namespace BlazorSvt.Modules.HomeRatePivot.Data;

public sealed class HomeRatePivotTable
{
    /// <summary>Первые числа месяцев окна (−3…+2 от текущего), длина 6.</summary>
    public required IReadOnlyList<DateOnly> Months { get; init; }

    public required IReadOnlyList<HomeRatePivotRow> Rows { get; init; }
}

public sealed class HomeRatePivotRow
{
    public required string DirectionLabel { get; init; }

    /// <summary>Ставки в порядке <see cref="HomeRatePivotTable.Months"/>; null = нет ставки.</summary>
    public required decimal?[] RatesByMonth { get; init; }
}
