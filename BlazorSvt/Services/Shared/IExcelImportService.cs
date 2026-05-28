using BlazorSvt.Models.Excel;

namespace BlazorSvt.Services.Shared;

public interface IExcelImportService<TItem> where TItem : new()
{
    Task<ImportResult> ImportAsync(byte[] workbook, CancellationToken cancellationToken = default);
}
