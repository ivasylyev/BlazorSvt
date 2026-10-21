namespace BlazorSvt.Platform.Grid.Models;

/// <summary>Страница read-модели: строки и общее число строк под фильтром.</summary>
public sealed record GridPage<TItem>(IReadOnlyList<TItem> Items, int TotalCount);
