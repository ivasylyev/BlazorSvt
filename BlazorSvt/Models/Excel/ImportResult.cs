namespace BlazorSvt.Models.Excel;

public abstract record ImportResult
{
    public sealed record Succeeded(int InsertedCount) : ImportResult;

    public sealed record HasErrors(byte[] AnnotatedWorkbook, int InvalidRowCount) : ImportResult;
}
