using System.Globalization;
using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.UnitTests;

/// <summary>Подписи колонок и текст ячеек таблицы дочернего примитива — как их выводит вложенный <c>Grid</c>.</summary>
internal static class DetailChildTableTestExtensions
{
    public static IReadOnlyList<string> Headers<TRow>(this DetailChildTable<TRow> table) =>
        table.Columns.Select(column => column.Header).ToList();

    public static IReadOnlyList<IReadOnlyList<string>> Cells<TRow>(this DetailChildTable<TRow> table) =>
        table.Rows
            .Select(row => (IReadOnlyList<string>)table.Columns
                .Select(column => Convert.ToString(column.DisplaySelector(row), CultureInfo.CurrentCulture) ?? string.Empty)
                .ToList())
            .ToList();
}
