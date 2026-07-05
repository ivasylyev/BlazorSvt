using System.Data.SqlClient;

namespace BlazorSvt.Platform.Infrastructure;

public static class OperationCancellation
{
    public static bool IsCancellation(Exception ex, CancellationToken cancellationToken = default) =>
        ex switch
        {
            OperationCanceledException => cancellationToken.CanBeCanceled
                ? cancellationToken.IsCancellationRequested
                : true,
#pragma warning disable CS0618 // Type or member is obsolete
            SqlException sql when IsSqlCancellation(sql) => !cancellationToken.CanBeCanceled
                || cancellationToken.IsCancellationRequested,
#pragma warning restore CS0618 // Type or member is obsolete
            _ => false
        };

#pragma warning disable CS0618 // Type or member is obsolete
    private static bool IsSqlCancellation(SqlException ex) =>
#pragma warning restore CS0618 // Type or member is obsolete
        ex.Number == -2
        || ex.Message.Contains("cancelled", StringComparison.OrdinalIgnoreCase)
        || ex.Message.Contains("canceled", StringComparison.OrdinalIgnoreCase);
}
