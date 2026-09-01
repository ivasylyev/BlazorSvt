using BlazorSvt.Platform.Access;

namespace BlazorSvt.Import.Services;

public class StagingRepository<TItem>(IAccessGuard accessGuard) : IStagingRepository<TItem>
{
    public Task<int> BulkInsertAsync(IReadOnlyList<TItem> items, CancellationToken cancellationToken = default)
    {
        accessGuard.Ensure(AccessAction.Import);

        throw new NotImplementedException(
            $"Staging for {typeof(TItem).Name} is not wired yet (attempted {items.Count} row(s)).");
    }
}
