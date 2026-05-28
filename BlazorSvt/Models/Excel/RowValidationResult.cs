namespace BlazorSvt.Models.Excel;

public record RowValidationResult<TItem>(
    int RowNumber,
    TItem Item,
    IReadOnlyList<ValidationError> Errors)
{
    public bool IsValid => Errors.Count == 0;
}
