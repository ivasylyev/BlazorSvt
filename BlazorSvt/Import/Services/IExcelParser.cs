using BlazorSvt.Import.Models;

namespace BlazorSvt.Import.Services;

public interface IExcelParser
{
    IReadOnlyList<ExcelRow<TItem>> Parse<TItem>(Stream workbook) where TItem : new();
}
