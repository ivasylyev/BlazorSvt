namespace BlazorSvt.Platform.Grid.Services;

public interface IDetailChildDataService
{
    /// <summary>
    /// Строки view из <see cref="DetailChildSourceAttribute"/> на <typeparamref name="TRow"/> по ключу родителя
    /// и набору состояний: по <c>Id</c>, не больше <c>Grid:ChildSectionRowLimit</c>, с общим числом строк.
    /// </summary>
    Task<DetailChildPage<TRow>> GetRowsAsync<TRow>(
        object parentKey,
        IReadOnlyCollection<int> states,
        CancellationToken cancellationToken);
}
