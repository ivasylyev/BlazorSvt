using System.Data;
using System.Diagnostics;
using Dapper;

namespace BlazorSvt.Utils;

public class DbConnectionLogDecorator(IDbConnection connection, ILogger logger)
{
    public Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        return ExecuteWithLogging(
            sql,
            parameters,
            commandType,
            () => connection.QueryMultipleAsync(
                sql,
                parameters,
                commandType: commandType));
    }

    public Task<TDetailItem?> QuerySingleOrDefaultAsync<TDetailItem>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        return ExecuteWithLogging(
            sql,
            parameters,
            commandType,
            () => connection.QuerySingleOrDefaultAsync<TDetailItem>(
                sql,
                parameters,
                commandType: commandType));
    }

    private async Task<TResult> ExecuteWithLogging<TResult>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType,
        Func<Task<TResult>> action)
    {
        logger.LogDebug(
            "Executing {CommandType} {Sql} with params {@Params}",
            commandType,
            sql,
            parameters.ToDictionary());

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await action();

            stopwatch.Stop();

            logger.LogDebug(
                "Executed {Sql} in {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds);

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error executing {Sql} after {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds);

            throw;
        }
    }
}