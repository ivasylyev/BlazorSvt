using System.Data;
using System.Diagnostics;
using Dapper;

namespace BlazorSvt.Utils;

public class DbConnectionLogDecorator(IDbConnection connection, ILogger logger)
{
    public async Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        logger.LogDebug(
            "Executing {CommandType} {Sql} with params {@Params}",
            commandType,
            sql,
            parameters.ToDictionary()
        );

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await connection.QueryMultipleAsync(
                sql,
                parameters,
                commandType: commandType);

            stopwatch.Stop();

            logger.LogDebug(
                "Executed {Sql} in {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds
            );

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error executing {Sql} after {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }


    public async Task<TDetailItem?> QuerySingleOrDefault<TDetailItem>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        logger.LogDebug(
            "Executing {CommandType} {Sql} with params {@Params}",
            commandType,
            sql,
            parameters.ToDictionary()
        );

        var stopwatch = Stopwatch.StartNew();

        try
        {
            var result = await connection.QueryFirstOrDefaultAsync<TDetailItem>(
                sql,
                parameters,
                commandType: commandType);

            stopwatch.Stop();

            logger.LogDebug(
                "Executed {Sql} in {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds
            );

            return result;
        }
        catch (Exception ex)
        {
            stopwatch.Stop();

            logger.LogError(
                ex,
                "Error executing {Sql} after {ElapsedMs} ms",
                sql,
                stopwatch.ElapsedMilliseconds
            );

            throw;
        }
    }
}