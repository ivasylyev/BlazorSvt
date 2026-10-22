namespace BlazorSvt.Modules.SmartRateLookup.Domain;

/// <summary>Что «поднято» до более общего значения на шаге поиска.</summary>
[Flags]
public enum LookupLifts
{
    None = 0,
    ProductToGroup = 1,
    DestinationToRegion = 2,
    TransportTypeToKind = 4
}

/// <summary>
/// Уровни поиска 1–8 из <c>CalcTransportRateCRM</c> (эталон — поведение на проде).
/// Порядок уровней — приоритет: сначала точное совпадение, затем всё более общие условия.
/// </summary>
public static class LookupLevels
{
    public static readonly IReadOnlyList<(int Level, LookupLifts Lifts)> All =
    [
        (1, LookupLifts.None),
        (2, LookupLifts.ProductToGroup),
        (3, LookupLifts.DestinationToRegion),
        (4, LookupLifts.DestinationToRegion | LookupLifts.ProductToGroup),
        (5, LookupLifts.TransportTypeToKind),
        (6, LookupLifts.TransportTypeToKind | LookupLifts.ProductToGroup),
        (7, LookupLifts.DestinationToRegion | LookupLifts.TransportTypeToKind),
        (8, LookupLifts.DestinationToRegion | LookupLifts.ProductToGroup | LookupLifts.TransportTypeToKind)
    ];
}
