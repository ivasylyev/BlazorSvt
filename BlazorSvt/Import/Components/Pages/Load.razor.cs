using BlazorSvt.Import.Models;
using BlazorSvt.Import.Services;
using FluentValidation;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Import.Components.Pages;

public partial class Load : SvtComponentBase
{
    [Inject]
    public IExcelParser Parser { get; set; } = default!;

    [Inject]
    public IValidator<LegStgWithoutProxyDto> Validator { get; set; } = default!;

    [Inject]
    public IExcelErrorWriter ErrorWriter { get; set; } = default!;

    [Inject]
    public IStagingRepository<LegStgWithoutProxyDto> Repository { get; set; } = default!;

    [Inject]
    public ILogger<Load> Logger { get; set; } = default!;

    private readonly ImportState _withoutProxy = new();

    private async Task ImportAsync(ExcelFile file)
    {
        _withoutProxy.Reset();

        Logger.LogInformation(
            "Import started: file {FileName}, size {SizeBytes} bytes, type {ItemType}",
            file.FileName,
            file.Content.Length,
            nameof(LegStgWithoutProxyDto));

        try
        {
            using var stream = new MemoryStream(file.Content);
            var rows = Parser.Parse<LegStgWithoutProxyDto>(stream);

            Logger.LogInformation(
                "Import parsed {RowCount} data row(s) from {FileName}",
                rows.Count,
                file.FileName);

            var results = rows.Select(Validate).ToList();

            var invalidRows = results
                .Where(result => !result.IsValid)
                .ToDictionary(result => result.RowNumber, result => result.Errors);

            if (invalidRows.Count > 0)
            {
                Logger.LogWarning(
                    "Import validation failed for {FileName}: {InvalidCount} invalid row(s) of {TotalCount}",
                    file.FileName,
                    invalidRows.Count,
                    results.Count);

                _withoutProxy.AnnotatedWorkbook = ErrorWriter.WriteErrors(file.Content, invalidRows);
                _withoutProxy.AnnotatedFileName = BuildErrorFileName(file.FileName);
                _withoutProxy.ErrorMessage = L["Load.ImportHasErrors", invalidRows.Count];
                return;
            }

            var validItems = results.Select(result => result.Item).ToList();
            var inserted = await Repository.BulkInsertAsync(validItems);

            Logger.LogInformation(
                "Import succeeded for {FileName}: inserted {InsertedCount} {ItemType} row(s)",
                file.FileName,
                inserted,
                nameof(LegStgWithoutProxyDto));

            _withoutProxy.SuccessMessage = L["Load.ImportSuccess", inserted];
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Import failed for file {FileName}", file.FileName);
            _withoutProxy.ErrorMessage = ex.Message;
        }
    }

    private RowValidationResult<LegStgWithoutProxyDto> Validate(ExcelRow<LegStgWithoutProxyDto> row)
    {
        var errors = new List<ValidationError>(row.ParseErrors);

        var validation = Validator.Validate(row.Item);
        errors.AddRange(validation.Errors.Select(failure =>
            new ValidationError(failure.PropertyName, failure.ErrorMessage)));

        return new RowValidationResult<LegStgWithoutProxyDto>(row.RowNumber, row.Item, errors);
    }

    private static string BuildErrorFileName(string original)
    {
        var name = Path.GetFileNameWithoutExtension(original);
        var extension = Path.GetExtension(original);
        return $"{name}_errors{extension}";
    }
}
