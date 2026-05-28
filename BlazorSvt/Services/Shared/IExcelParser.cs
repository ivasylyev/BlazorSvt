using BlazorSvt.Models.Excel;

namespace BlazorSvt.Services.Shared;

public interface IExcelParser
{
    IReadOnlyList<ExcelRow<TItem>> Parse<TItem>(Stream workbook) where TItem : new();
}
