using BlazorSvt.Models.Excel;
using FluentValidation;

namespace BlazorSvt.Services.Shared;

public class ExcelImportService<TItem>(
    IExcelParser parser,
    IValidator<TItem> validator,
    IExcelErrorWriter errorWriter,
    IStagingRepository<TItem> repository) : IExcelImportService<TItem>
    where TItem : new()
{
    public async Task<ImportResult> ImportAsync(byte[] workbook, CancellationToken cancellationToken = default)
    {
        using var stream = new MemoryStream(workbook);
        var rows = parser.Parse<TItem>(stream);

        var results = rows
            .Select(Validate)
            .ToList();

        var invalidRows = results
            .Where(result => !result.IsValid)
            .ToDictionary(
                result => result.RowNumber,
                result => result.Errors);

        if (invalidRows.Count > 0)
        {
            var annotated = errorWriter.WriteErrors(workbook, invalidRows);
            return new ImportResult.HasErrors(annotated, invalidRows.Count);
        }

        var validItems = results.Select(result => result.Item).ToList();
        var inserted = await repository.BulkInsertAsync(validItems, cancellationToken);
        return new ImportResult.Succeeded(inserted);
    }

    private RowValidationResult<TItem> Validate(ExcelRow<TItem> row)
    {
        var errors = new List<ValidationError>(row.ParseErrors);

        var validation = validator.Validate(row.Item);

        errors.AddRange(validation.Errors.Select(failure =>
            new ValidationError(failure.PropertyName, failure.ErrorMessage)));

        return new RowValidationResult<TItem>(row.RowNumber, row.Item, errors);
    }
}
