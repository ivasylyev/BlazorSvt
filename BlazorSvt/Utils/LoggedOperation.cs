using System.Diagnostics;

namespace BlazorSvt.Utils;

public static class LoggedOperation
{
    public static Task ExecuteAsync(
        ILogger logger,
        string operationName,
        Func<Task> action) =>
        ExecuteCoreAsync(logger, operationName, action);

    public static async Task<TResult> ExecuteAsync<TResult>(
        ILogger logger,
        string operationName,
        Func<Task<TResult>> action)
    {
        TResult result = default!;

        await ExecuteCoreAsync(logger, operationName, async () =>
        {
            result = await action();
        });

        return result;
    }

    private static async Task ExecuteCoreAsync(
        ILogger logger,
        string operationName,
        Func<Task> action)
    {
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await action();

            logger.LogDebug(
                "{OperationName} completed in {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);
        }
        catch (Exception ex)
        {
            logger.LogError(
                ex,
                "Error in {OperationName} after {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}
