using System.Diagnostics;

namespace BlazorSvt.Utils;

public static class LoggedOperation
{
    public static async Task ExecuteAsync(
        ILogger logger,
        string operationName,
        Func<Task> action)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await action();
            stopwatch.Stop();

            logger.LogDebug(
                "{OperationName} completed in {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error in {OperationName} after {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }

    public static async Task<TResult> ExecuteAsync<TResult>(
        ILogger logger,
        string operationName,
        Func<Task<TResult>> action)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await action();
            stopwatch.Stop();

            logger.LogDebug(
                "{OperationName} completed in {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error in {OperationName} after {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
