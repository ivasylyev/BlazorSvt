using BlazorSvt.Import.Models;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Import.Services;

public class ExcelErrorWriter(IStringLocalizer<PlatformResources> localizer) : IExcelErrorWriter
{
    private const int HeaderRowNumber = 3;

    public byte[] WriteErrors(
        byte[] sourceWorkbook,
        IReadOnlyDictionary<int, IReadOnlyList<ValidationError>> errorsByRow)
    {
        using var input = new MemoryStream(sourceWorkbook);
        using var workbook = new XLWorkbook(input);
        var worksheet = workbook.Worksheets.First();

        var errorColumn = (worksheet.LastColumnUsed()?.ColumnNumber() ?? 0) + 1;

        var headerCell = worksheet.Cell(HeaderRowNumber, errorColumn);
        headerCell.Value = localizer["Excel.ErrorsColumnHeader"].Value;
        headerCell.Style.Font.Bold = true;

        foreach (var (rowNumber, errors) in errorsByRow)
        {
            if (errors.Count == 0)
            {
                continue;
            }

            var text = string.Join(
                Environment.NewLine,
                errors.Select(FormatError));

            var cell = worksheet.Cell(rowNumber, errorColumn);
            cell.Value = text;
            cell.Style.Alignment.WrapText = true;
            cell.Style.Fill.BackgroundColor = XLColor.LightPink;
        }

        worksheet.Column(errorColumn).AdjustToContents();

        using var output = new MemoryStream();
        workbook.SaveAs(output);
        return output.ToArray();
    }

    private static string FormatError(ValidationError error) =>
        string.IsNullOrEmpty(error.PropertyName)
            ? error.Message
            : $"{error.PropertyName}: {error.Message}";
}
