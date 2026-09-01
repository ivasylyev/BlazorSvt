using System.Data;
using System.Data.SqlClient;
using BlazorSvt.Platform.Infrastructure.Data;
using Dapper;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Access;

/// <summary>Чтение/запись identity в легаси-таблицах; get-or-create идемпотентен (при дублях Login берётся min Id).</summary>
public sealed class UserAccessRepository(
    IOptions<DatabaseOptions> options,
    ILogger<UserAccessRepository> logger) : IUserAccessRepository
{
    private readonly string connectionString = options.Value.MdmDb;
    private readonly int commandTimeoutSeconds = options.Value.DefaultQueryTimeoutSeconds;

    public async Task<UserRecord?> FindByLoginAsync(string login, CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);
        return await FindByLoginCoreAsync(db, login, cancellationToken);
    }

    public async Task<UserRecord> GetOrCreateUserAsync(
        string login,
        string? name,
        string? email,
        CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        var existing = await FindByLoginCoreAsync(db, login, cancellationToken);
        if (existing is null)
        {
            await InsertUserAsync(db, login, name, email, cancellationToken);
        }

        return (await FindByLoginCoreAsync(db, login, cancellationToken))
            ?? throw new InvalidOperationException($"Failed to get or create dbo.[User] for {login}.");
    }

    public async Task UpdateProfileAsync(
        int userId,
        string? name,
        string? email,
        CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        var parameters = new DynamicParameters();
        parameters.Add("Id", userId);
        parameters.Add("Name", name);
        parameters.Add("Email", email);

        await db.ExecuteAsync(
            """
            UPDATE dbo.[User]
            SET [Name] = @Name,
                [Email] = @Email
            WHERE [Id] = @Id
            """,
            parameters,
            CommandType.Text,
            cancellationToken);
    }

    public async Task<IReadOnlyList<RoleRecord>> GetRolesAsync(CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        var rows = await db.QueryAsync<RoleRecord>(
            "SELECT [Id], [Name], [DomainGroup] FROM dbo.[Role]",
            new DynamicParameters(),
            CommandType.Text,
            cancellationToken);

        return rows.ToList();
    }

    public async Task<IReadOnlyList<int>> GetUserRoleIdsAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        var rows = await db.QueryAsync<int>(
            "SELECT [RoleId] FROM dbo.[UserRole] WHERE [UserId] = @UserId",
            parameters,
            CommandType.Text,
            cancellationToken);

        return rows.ToList();
    }

    public async Task ReconcileUserRolesAsync(
        int userId,
        IReadOnlyList<int> desiredRoleIds,
        CancellationToken cancellationToken = default)
    {
        var current = await GetUserRoleIdsAsync(userId, cancellationToken);
        var diff = UserRoleReconciler.Diff(current, desiredRoleIds);
        if (diff.Add.Count == 0 && diff.Remove.Count == 0)
        {
            return;
        }

        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        foreach (var roleId in diff.Add)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("RoleId", roleId);
            await db.ExecuteAsync(
                """
                IF NOT EXISTS (
                    SELECT 1 FROM dbo.[UserRole] WHERE [UserId] = @UserId AND [RoleId] = @RoleId)
                INSERT INTO dbo.[UserRole] ([UserId], [RoleId]) VALUES (@UserId, @RoleId)
                """,
                parameters,
                CommandType.Text,
                cancellationToken);
        }

        foreach (var roleId in diff.Remove)
        {
            var parameters = new DynamicParameters();
            parameters.Add("UserId", userId);
            parameters.Add("RoleId", roleId);
            await db.ExecuteAsync(
                "DELETE FROM dbo.[UserRole] WHERE [UserId] = @UserId AND [RoleId] = @RoleId",
                parameters,
                CommandType.Text,
                cancellationToken);
        }
    }

    public async Task<IReadOnlyList<string>> GetUserRoleNamesAsync(
        int userId,
        CancellationToken cancellationToken = default)
    {
        await using var connection = OpenConnection();
        await connection.OpenAsync(cancellationToken);
        var db = CreateLog(connection);

        var parameters = new DynamicParameters();
        parameters.Add("UserId", userId);

        var rows = await db.QueryAsync<string>(
            """
            SELECT r.[Name]
            FROM dbo.[UserRole] ur
            INNER JOIN dbo.[Role] r ON r.[Id] = ur.[RoleId]
            WHERE ur.[UserId] = @UserId
              AND r.[Name] IS NOT NULL
            """,
            parameters,
            CommandType.Text,
            cancellationToken);

        return rows.ToList();
    }

    private static async Task<UserRecord?> FindByLoginCoreAsync(
        DbConnectionLogDecorator db,
        string login,
        CancellationToken cancellationToken)
    {
        cancellationToken.ThrowIfCancellationRequested();

        var parameters = new DynamicParameters();
        parameters.Add("Login", login);

        return await db.QuerySingleOrDefaultAsync<UserRecord?>(
            """
            SELECT TOP (1) [Id], [Login], [Name], [Email]
            FROM dbo.[User]
            WHERE LOWER([Login]) = LOWER(@Login)
            ORDER BY [Id]
            """,
            parameters,
            CommandType.Text);
    }

    private static Task InsertUserAsync(
        DbConnectionLogDecorator db,
        string login,
        string? name,
        string? email,
        CancellationToken cancellationToken)
    {
        var parameters = new DynamicParameters();
        parameters.Add("Login", login);
        parameters.Add("Name", name);
        parameters.Add("Email", email);

        return db.ExecuteAsync(
            """
            INSERT INTO dbo.[User] ([Login], [Name], [Email], [Locale])
            VALUES (@Login, @Name, @Email, NULL)
            """,
            parameters,
            CommandType.Text,
            cancellationToken);
    }

    private DbConnectionLogDecorator CreateLog(SqlConnection connection) =>
        new(connection, logger, commandTimeoutSeconds);

    private SqlConnection OpenConnection()
    {
#pragma warning disable CS0618
        return new SqlConnection(connectionString);
#pragma warning restore CS0618
    }
}
