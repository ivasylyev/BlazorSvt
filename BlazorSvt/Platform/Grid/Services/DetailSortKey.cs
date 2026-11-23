namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Ключ сортировки колонки таблицы дочернего примитива. Пустое значение (<c>null</c>) идёт раньше любого
/// непустого при сортировке по возрастанию; ключи одной колонки всегда одного типа.
/// </summary>
public readonly struct DetailSortKey : IComparable<DetailSortKey>, IComparable
{
    private readonly IComparable? value;

    public DetailSortKey(IComparable? value) => this.value = value;

    public int CompareTo(DetailSortKey other) => (value, other.value) switch
    {
        (null, null) => 0,
        (null, _) => -1,
        (_, null) => 1,
        var (left, right) => left!.CompareTo(right)
    };

    public int CompareTo(object? obj) => obj switch
    {
        null => 1,
        DetailSortKey other => CompareTo(other),
        _ => throw new ArgumentException($"Object must be of type {nameof(DetailSortKey)}.", nameof(obj))
    };
}
