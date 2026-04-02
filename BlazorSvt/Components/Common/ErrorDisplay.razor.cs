using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Common;

public partial class ErrorDisplay
{
    [Parameter] 
    public Exception? Exception { get; set; }
}