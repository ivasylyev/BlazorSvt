using BlazorSvt.Models.Excel;

namespace BlazorSvt.Services.Shared;

public interface IExcelErrorWriter
{
    byte[] WriteErrors(byte[] sourceWorkbook, IReadOnlyDictionary<int, IReadOnlyList<ValidationError>> errorsByRow);
}
