namespace BlazorSvt.Platform.Infrastructure.Config;

public class ReportOptions
{
    public int ShortReportConfirmationThreshold { get; set; } = 10_000;

    public int FullReportConfirmationThreshold { get; set; } = 5_000;
}
