using System.Diagnostics;
using BlazorSvt.Platform.Infrastructure;

namespace BlazorSvt.Platform.Infrastructure.Logging;

public static class LoggedOperation
{
    /// <summary>Предупреждение, если операция дольше этого порога (мс).</summary>
    public const int SlowOperationThresholdMs = 5_000;

    public static Task ExecuteAsync(
        ILogger logger,
        string operationName,
        Func<Task> action,
        IReadOnlyDictionary<string, object?>? properties = null) =>
        ExecuteCoreAsync(logger, operationName, action, properties);

    public static async Task<TResult> ExecuteAsync<TResult>(
        ILogger logger,
        string operationName,
        Func<Task<TResult>> action,
        IReadOnlyDictionary<string, object?>? properties = null)
    {
        TResult result = default!;

        await ExecuteCoreAsync(logger, operationName, async () =>
        {
            result = await action();
        }, properties);

        return result;
    }

    private static async Task ExecuteCoreAsync(
        ILogger logger,
        string operationName,
        Func<Task> action,
        IReadOnlyDictionary<string, object?>? properties)
    {
        using var scope = properties is null ? null : logger.BeginScope(properties);
        var stopwatch = Stopwatch.StartNew();

        try
        {
            await action();

            var elapsedMs = stopwatch.ElapsedMilliseconds;

            if (elapsedMs >= SlowOperationThresholdMs)
            {
                logger.LogWarning(
                    "{OperationName} completed slowly in {ElapsedMs} ms",
                    operationName,
                    elapsedMs);
            }
            else
            {
                logger.LogDebug(
                    "{OperationName} completed in {ElapsedMs} ms",
                    operationName,
                    elapsedMs);
            }
        }
        catch (Exception ex) when (OperationCancellation.IsCancellation(ex))
        {
            logger.LogDebug(
                "{OperationName} cancelled after {ElapsedMs} ms",
                operationName,
                stopwatch.ElapsedMilliseconds);
            throw;
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
