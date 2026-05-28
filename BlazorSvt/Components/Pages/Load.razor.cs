using BlazorSvt.Components.Common;
using BlazorSvt.Models.Excel;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;
using Microsoft.JSInterop;

namespace BlazorSvt.Components.Pages;

public partial class Load : SvtComponentBase
{
    [Inject]
    public IExcelImportService<Models.Dto.LegStgWithoutProxyDto> WithoutProxyImporter { get; set; } = default!;

    [Inject]
    public IJSRuntime JS { get; set; } = default!;

    private readonly ImportState _withoutProxy = new();

    private async Task ImportAsync(ExcelFile file)
    {
        _withoutProxy.Reset();

        var result = await WithoutProxyImporter.ImportAsync(file.Content);
        switch (result)
        {
            case ImportResult.Succeeded succeeded:
                _withoutProxy.SuccessMessage = L["Load.ImportSuccess", succeeded.InsertedCount];
                break;

            case ImportResult.HasErrors hasErrors:
                _withoutProxy.ErrorMessage = L["Load.ImportHasErrors", hasErrors.InvalidRowCount];
                _withoutProxy.AnnotatedWorkbook = hasErrors.AnnotatedWorkbook;
                _withoutProxy.AnnotatedFileName = BuildErrorFileName(file.FileName);
                break;
        }
    }

    private async Task DownloadAsync()
    {
        if (_withoutProxy.AnnotatedWorkbook is null)
        {
            return;
        }

        using var stream = new MemoryStream(_withoutProxy.AnnotatedWorkbook);
        using var streamRef = new DotNetStreamReference(stream);
        await JS.InvokeVoidAsync("downloadFileFromStream", _withoutProxy.AnnotatedFileName, streamRef);
    }

    private static string BuildErrorFileName(string original)
    {
        var name = Path.GetFileNameWithoutExtension(original);
        var extension = Path.GetExtension(original);
        return $"{name}_errors{extension}";
    }

    private sealed class ImportState
    {
        public string? SuccessMessage { get; set; }
        public string? ErrorMessage { get; set; }
        public byte[]? AnnotatedWorkbook { get; set; }
        public string AnnotatedFileName { get; set; } = "errors.xlsx";

        public void Reset()
        {
            SuccessMessage = null;
            ErrorMessage = null;
            AnnotatedWorkbook = null;
        }
    }
}
