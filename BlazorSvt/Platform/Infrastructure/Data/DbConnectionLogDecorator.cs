using System.Data;
using Dapper;

using BlazorSvt.Platform.Infrastructure.Logging;

namespace BlazorSvt.Platform.Infrastructure.Data;

public class DbConnectionLogDecorator(IDbConnection connection, ILogger logger, int commandTimeoutSeconds)
{
    public Task<SqlMapper.GridReader> QueryMultipleAsync(
        string sql,
        DynamicParameters parameters,
        CommandType commandType,
        CancellationToken cancellationToken = default)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QueryMultipleAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    commandType: commandType,
                    commandTimeout: commandTimeoutSeconds,
                    cancellationToken: cancellationToken)));
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
        CommandType commandType,
        CancellationToken cancellationToken = default)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QueryAsync<T>(
                new CommandDefinition(
                    sql,
                    parameters,
                    commandType: commandType,
                    commandTimeout: commandTimeoutSeconds,
                    cancellationToken: cancellationToken)));
    }
}
