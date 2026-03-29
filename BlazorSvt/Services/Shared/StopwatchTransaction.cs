using System.Diagnostics;

namespace BlazorSvt.Services.Shared;

public class StopwatchTransaction(PageTimingService timingService) : IDisposable
{
    private readonly Stopwatch stopwatch = Stopwatch.StartNew();

    public void Dispose()
    {
        stopwatch.Stop();
        timingService.LoadDuration = stopwatch.Elapsed;
    }
}