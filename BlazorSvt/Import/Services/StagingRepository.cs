namespace BlazorSvt.Import.Services;

// Placeholder persistence: the target staging table / stored procedure is not defined yet.
// Replace the body with the real bulk insert (SqlBulkCopy or Dapper) once the schema is known.
public class StagingRepository<TItem> : IStagingRepository<TItem>
{
    public Task<int> BulkInsertAsync(IReadOnlyList<TItem> items, CancellationToken cancellationToken = default)
    {
        throw new NotImplementedException(
            $"Staging for {typeof(TItem).Name} is not wired yet (attempted {items.Count} row(s)).");
    }
}
