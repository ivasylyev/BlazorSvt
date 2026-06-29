namespace BlazorSvt.Import.Services;

public interface IStagingRepository<TItem>
{
    Task<int> BulkInsertAsync(IReadOnlyList<TItem> items, CancellationToken cancellationToken = default);
}
