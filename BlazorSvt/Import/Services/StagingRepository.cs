namespace BlazorSvt.Import.Services;

// Placeholder persistence: the target staging table / stored procedure is not defined yet.
// Replace the body with the real bulk insert (SqlBulkCopy or Dapper) once the schema is known.
public class StagingRepository<TItem>(ILogger<StagingRepository<TItem>> logger) : IStagingRepository<TItem>
{
    public Task<int> BulkInsertAsync(IReadOnlyList<TItem> items, CancellationToken cancellationToken = default)
    {
        logger.LogWarning(
            "BulkInsertAsync is not implemented yet. Skipped persisting {Count} {Type} item(s).",
            items.Count,
            typeof(TItem).Name);

        return Task.FromResult(items.Count);
    }
}
