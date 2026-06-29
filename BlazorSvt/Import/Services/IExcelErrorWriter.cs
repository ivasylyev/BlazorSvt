using BlazorSvt.Import.Models;

namespace BlazorSvt.Import.Services;

public interface IExcelErrorWriter
{
    byte[] WriteErrors(byte[] sourceWorkbook, IReadOnlyDictionary<int, IReadOnlyList<ValidationError>> errorsByRow);
}
