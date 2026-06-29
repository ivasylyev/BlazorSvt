namespace BlazorSvt.Import.Models;

public record ExcelRow<TItem>(
    int RowNumber,
    TItem Item,
    IReadOnlyList<ValidationError> ParseErrors);
