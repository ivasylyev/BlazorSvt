namespace BlazorSvt.Platform.Grid.Models;

/// <summary>Строки дочернего примитива, прочитанные для одного родителя.</summary>
public sealed record DetailChildPage<TRow>(IReadOnlyList<TRow> Rows, int TotalCount);

/// <summary>
/// Таблица дочернего примитива на карточке без привязки к типу строки: так её держит <c>GenericDetailView</c>.
/// Типизированные строки и колонки — в <see cref="DetailChildTable{TRow}"/>.
/// </summary>
public abstract record DetailChildTable(int TotalCount, int LoadedCount)
{
    public abstract Type RowType { get; }

    public bool HasRows => TotalCount > 0;

    public bool IsTruncated => TotalCount > LoadedCount;
}

/// <summary>Типизированные строки дочернего примитива и колонки в порядке вывода.</summary>
public sealed record DetailChildTable<TRow>(
    IReadOnlyList<DetailSetting<TRow>> Columns,
    IReadOnlyList<TRow> Rows,
    int TotalCount)
    : DetailChildTable(TotalCount, Rows.Count)
{
    public override Type RowType => typeof(TRow);
}
