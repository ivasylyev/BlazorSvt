namespace BlazorSvt.Models.Excel;

public sealed class ImportState
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
