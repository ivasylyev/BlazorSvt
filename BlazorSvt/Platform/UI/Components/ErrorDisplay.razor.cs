using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.UI.Components;

public partial class ErrorDisplay
{
    private Exception? loggedException;

    [Inject]
    public ILogger<ErrorDisplay> Logger { get; set; } = default!;

    [Parameter]
    public Exception? Exception { get; set; }

    protected override void OnParametersSet()
    {
        if (Exception is null || ReferenceEquals(Exception, loggedException))
        {
            return;
        }

        loggedException = Exception;
        Logger.LogError(Exception, "Unhandled UI error shown in ErrorBoundary");
    }
}
