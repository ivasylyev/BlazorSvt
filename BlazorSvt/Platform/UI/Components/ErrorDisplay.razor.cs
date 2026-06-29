using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Platform.UI.Components;

public partial class ErrorDisplay
{
    [Parameter] 
    public Exception? Exception { get; set; }
}