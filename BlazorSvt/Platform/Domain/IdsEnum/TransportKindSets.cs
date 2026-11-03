namespace BlazorSvt.Platform.Domain.IdsEnum;

public static class TransportKindSets
{
    public static IReadOnlyList<TransportKindRu> MainRu { get; } =
    [
        TransportKindRu.Auto,
        TransportKindRu.Rail,
        TransportKindRu.Mix
    ];

    public static IReadOnlyList<TransportKindEn> MainEn { get; } =
    [
        TransportKindEn.Auto,
        TransportKindEn.Rail,
        TransportKindEn.Mix
    ];
}
