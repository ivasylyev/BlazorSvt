namespace BlazorSvt.Platform.Infrastructure.Config;

public class DatabaseOptions
{
    public string MdmDb { get; set; } = string.Empty;

    public int DefaultQueryTimeoutSeconds { get; set; } = 30;

    public int ReportQueryTimeoutSeconds { get; set; } = 300;
}