namespace BlazorSvt.Platform.Access;

public sealed record RoleRecord(int Id, string? Name, string? DomainGroup);

public sealed record UserRecord(int Id, string Login, string? Name, string? Email);

public sealed record DirectoryUser(string? DisplayName, string? Email, IReadOnlyList<string> Groups);

public sealed record UserRoleDiff(IReadOnlyList<int> Add, IReadOnlyList<int> Remove);
