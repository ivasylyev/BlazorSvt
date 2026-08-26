namespace BlazorSvt.Modules.HomeRatePivot.Data;

/// <summary>
/// Строка long-факта из SQL (пара × месяц).
/// </summary>
public sealed class HomeRatePivotFactRow
{
    public int SortOrder { get; init; }
    public required string NodeFromCode { get; init; }
    public required string NodeToCode { get; init; }
    public string? NodeFromNameRu { get; init; }
    public string? NodeFromNameEn { get; init; }
    public string? NodeToNameRu { get; init; }
    public string? NodeToNameEn { get; init; }
    public int? Year { get; init; }
    public int? Month { get; init; }
    public decimal? RateLevel3 { get; init; }
}
