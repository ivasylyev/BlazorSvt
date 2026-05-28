using BlazorSvt.Models.Grid;
using ClosedXML.Excel;

namespace BlazorSvt.Services.Shared;

public class GridExcelExporter : IGridExcelExporter
{
    public byte[] Export<T>(IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns)
    {
        var visibleColumns = columns.Where(c => c.Visible).ToList();

        using var workbook = new XLWorkbook();
        var worksheet = workbook.Worksheets.Add("Report");

        for (var col = 0; col < visibleColumns.Count; col++)
        {
            var headerCell = worksheet.Cell(1, col + 1);
            headerCell.Value = visibleColumns[col].Header;
            headerCell.Style.Font.Bold = true;
        }

        for (var row = 0; row < items.Count; row++)
        {
            var item = items[row];
            for (var col = 0; col < visibleColumns.Count; col++)
            {
                SetCellValue(worksheet.Cell(row + 2, col + 1), visibleColumns[col].DisplaySelector(item));
            }
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
