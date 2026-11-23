namespace BlazorSvt.Platform.Grid.Models;

/// <summary>Строки дочернего примитива, прочитанные для одного родителя.</summary>
public sealed record DetailChildPage<TRow>(IReadOnlyList<TRow> Rows, int TotalCount);

/// <summary>Готовая к выводу таблица дочернего примитива на карточке.</summary>
public sealed record DetailChildTable(
    IReadOnlyList<string> Headers,
    IReadOnlyList<IReadOnlyList<string>> Rows,
    int TotalCount)
{
    public bool HasRows => TotalCount > 0;

    public bool IsTruncated => TotalCount > Rows.Count;
}
