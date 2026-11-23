using System.Globalization;

namespace BlazorSvt.Platform.Grid.Services;

/// <summary>
/// Группа-таблица дочернего примитива на карточке. Регистрируется в модуле справочника;
/// <c>GenericDetailView</c> выводит группы после групп родителя в порядке регистрации.
/// </summary>
public interface IDetailChildSection<in TDetailItem>
{
    string Title { get; }

    Task<DetailChildTable> LoadAsync(TDetailItem parent, string lang, CancellationToken cancellationToken);
}

public abstract class DetailChildSection<TDetailItem, TRow>(IDetailChildDataService dataService)
    : IDetailChildSection<TDetailItem>
{
    public abstract string Title { get; }

    protected abstract object ParentKey(TDetailItem parent);

    protected abstract bool ParentIsArchive(TDetailItem parent);

    /// <summary>Колонки в порядке вывода; группа билдера значения не имеет.</summary>
    protected abstract DetailSettingsCollection<TRow> BuildColumns(string lang, bool parentIsArchive);

    public async Task<DetailChildTable> LoadAsync(TDetailItem parent, string lang, CancellationToken cancellationToken)
    {
        var parentIsArchive = ParentIsArchive(parent);
        var page = await dataService.GetRowsAsync<TRow>(
            ParentKey(parent),
            DetailChildRowStates.For(parentIsArchive),
            cancellationToken);

        var columns = BuildColumns(lang, parentIsArchive).GroupSettings.Values.Single().ToList();

        var rows = page.Rows
            .Select(row => (IReadOnlyList<string>)columns
                .Select(column => Convert.ToString(column.DisplaySelector(row), CultureInfo.CurrentCulture) ?? string.Empty)
                .ToList())
            .ToList();

        return new DetailChildTable(columns.Select(c => c.Header).ToList(), rows, page.TotalCount);
    }
}
