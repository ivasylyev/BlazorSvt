using BlazorSvt.Models.Grid;
using ClosedXML.Excel;

namespace BlazorSvt.Services.Shared;

public class GridExcelExporter : IGridExcelExporter
{
    private sealed record ExportColumn<T>(string Header, Func<T, object?> GetValue);

    public byte[] ExportShortReport<T>(IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns) =>
        ExportCore(
            items,
            columns
                .Where(c => c.Visible)
                .Select(c => new ExportColumn<T>(c.Header, item => c.DisplaySelector(item)))
                .ToList());

    public byte[] ExportFullReport<T>(IReadOnlyList<T> items, IEnumerable<DetailSetting<T>> columns) =>
        ExportCore(
            items,
            columns
                .Select(c => new ExportColumn<T>(
                    c.Header,
                    item => c.VisibleSelector(item) ? c.DisplaySelector(item) : null))
                .ToList());

    private static byte[] ExportCore<T>(IReadOnlyList<T> items, IReadOnlyList<ExportColumn<T>> columns)
    {
        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        for (var col = 0; col < columns.Count; col++)
        {
            var headerCell = worksheet.Cell(1, col + 1);
            headerCell.Value = columns[col].Header;
            headerCell.Style.Font.Bold = true;
        }

        for (var row = 0; row < items.Count; row++)
        {
            var item = items[row];
            for (var col = 0; col < columns.Count; col++)
                SetCellValue(worksheet.Cell(row + 2, col + 1), columns[col].GetValue(item));
        }

        worksheet.Columns().AdjustToContents();

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    private static void SetCellValue(IXLCell cell, object? value)
    {
        switch (value)
        {
            case null:
                cell.Value = Blank.Value;
                break;
            case DateOnly dateOnly:
                cell.Value = dateOnly.ToDateTime(TimeOnly.MinValue);
                break;
            case DateTime dateTime:
                cell.Value = dateTime;
                break;
            case bool boolean:
                cell.Value = boolean;
                break;
            case int intValue:
                cell.Value = intValue;
                break;
            case long longValue:
                cell.Value = longValue;
                break;
            case decimal decimalValue:
                cell.Value = decimalValue;
                break;
            case double doubleValue:
                cell.Value = doubleValue;
                break;
            default:
                cell.Value = value.ToString() ?? string.Empty;
                break;
        }
    }
}
