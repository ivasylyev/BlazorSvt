using System.Data;
using Dapper;

using BlazorSvt.Platform.Infrastructure.Logging;

namespace BlazorSvt.Platform.Infrastructure.Data;

public class DbConnectionLogDecorator(IDbConnection connection, ILogger logger, int commandTimeoutSeconds)
{
    public Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QueryMultipleAsync(
                sql,
                parameters,
                commandType: commandType,
                commandTimeout: commandTimeoutSeconds));
    }

    public Task<TDetailItem?> QuerySingleOrDefaultAsync<TDetailItem>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QuerySingleOrDefaultAsync<TDetailItem>(
                sql,
                parameters,
                commandType: commandType,
                commandTimeout: commandTimeoutSeconds));
    }

    public Task<IEnumerable<T>> QueryAsync<T>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QueryAsync<T>(
                sql,
                parameters,
                commandType: commandType,
                commandTimeout: commandTimeoutSeconds));
    }
}
