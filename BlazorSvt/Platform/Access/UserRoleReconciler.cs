namespace BlazorSvt.Platform.Access;

/// <summary>Сопоставление AD-групп с <c>Role.DomainGroup</c> (ignore case, с доменом) и diff add/remove.</summary>
public static class UserRoleReconciler
{
    public static IReadOnlyList<int> Match(
        IReadOnlyList<RoleRecord> roles,
        IReadOnlyList<string> directoryGroups)
    {
        var groupSet = new HashSet<string>(StringComparer.OrdinalIgnoreCase);
        foreach (var group in directoryGroups)
        {
            if (!string.IsNullOrWhiteSpace(group))
            {
                groupSet.Add(group.Trim());
            }
        }

        var matched = new List<int>();
        var seen = new HashSet<int>();
        foreach (var role in roles)
        {
            if (string.IsNullOrWhiteSpace(role.DomainGroup))
            {
                continue;
            }

            if (groupSet.Contains(role.DomainGroup.Trim()) && seen.Add(role.Id))
            {
                matched.Add(role.Id);
            }
        }

        return matched;
    }

    public static UserRoleDiff Diff(
        IReadOnlyCollection<int> currentRoleIds,
        IReadOnlyCollection<int> desiredRoleIds)
    {
        var current = currentRoleIds as HashSet<int> ?? [.. currentRoleIds];
        var desired = desiredRoleIds as HashSet<int> ?? [.. desiredRoleIds];

        var add = desired.Except(current).ToList();
        var remove = current.Except(desired).ToList();
        return new UserRoleDiff(add, remove);
    }
}
