using System.Data;
using Dapper;

namespace BlazorSvt.Platform.Infrastructure.Data;

public class DbConnectionLogDecorator(IDbConnection connection, ILogger logger, int commandTimeoutSeconds)
{
    private const int MaxPropertyLength = 500;

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
                    cancellationToken: cancellationToken)),
            BuildProperties(sql, commandType, parameters));
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
                commandTimeout: commandTimeoutSeconds),
            BuildProperties(sql, commandType, parameters));
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
                    cancellationToken: cancellationToken)),
            BuildProperties(sql, commandType, parameters));
    }

    public Task<T> QuerySingleAsync<T>(
        string sql,
        DynamicParameters parameters,
        CommandType commandType,
        CancellationToken cancellationToken = default)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.QuerySingleAsync<T>(
                new CommandDefinition(
                    sql,
                    parameters,
                    commandType: commandType,
                    commandTimeout: commandTimeoutSeconds,
                    cancellationToken: cancellationToken)),
            BuildProperties(sql, commandType, parameters));
    }

    public Task<int> ExecuteAsync(
        string sql,
        DynamicParameters parameters,
        CommandType commandType,
        CancellationToken cancellationToken = default)
    {
        return LoggedOperation.ExecuteAsync(
            logger,
            sql,
            () => connection.ExecuteAsync(
                new CommandDefinition(
                    sql,
                    parameters,
                    commandType: commandType,
                    commandTimeout: commandTimeoutSeconds,
                    cancellationToken: cancellationToken)),
            BuildProperties(sql, commandType, parameters));
    }

    private Dictionary<string, object?> BuildProperties(
        string sql,
        CommandType commandType,
        DynamicParameters parameters)
    {
        var properties = new Dictionary<string, object?>
        {
            ["DbCommand"] = Truncate(sql),
            ["CommandType"] = commandType.ToString(),
            ["CommandTimeoutSeconds"] = commandTimeoutSeconds
        };

        foreach (var (name, value) in parameters.ToDictionary())
        {
            // Крупные JSON-метаданные колонок засоряют scope; ключевые параметры фильтра/страницы оставляем.
            if (name is "AllowedColumnsJson" or "SelectList")
            {
                continue;
            }

            properties[$"DbParam_{name}"] = TruncateValue(value);
        }

        return properties;
    }

    private static object? TruncateValue(object? value) =>
        value switch
        {
            null => null,
            string s => Truncate(s),
            byte[] bytes => Convert.ToHexString(bytes),
            _ => Truncate(value.ToString())
        };

    private static string? Truncate(string? value)
    {
        if (value is null)
        {
            return null;
        }

        return value.Length <= MaxPropertyLength
            ? value
            : value[..MaxPropertyLength] + "…";
    }
}
