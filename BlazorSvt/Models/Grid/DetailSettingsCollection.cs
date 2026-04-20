using System.Collections.Frozen;

namespace BlazorSvt.Models.Grid;

public class DetailSettingsCollection<T>
{
    public FrozenDictionary<string, IReadOnlyCollection<DetailSetting<T>>> GroupSettings { get; }

    public DetailSettingsCollection(IEnumerable<DetailSetting<T>> detailSettings)
    {
        GroupSettings = detailSettings
            .GroupBy(x => x.GroupHeader)
            .ToFrozenDictionary(
                group => group.Key,
                group => (IReadOnlyCollection<DetailSetting<T>>)group.ToList().AsReadOnly());
    }
}