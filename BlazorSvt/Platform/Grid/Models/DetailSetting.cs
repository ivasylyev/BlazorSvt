using System.Linq.Expressions;

namespace BlazorSvt.Platform.Grid.Models;

public class DetailSetting<T>
{
    public required string Name { get; set; }
    public required string Header { get; set; }
    public required string GroupHeader { get; set; }
    public required Func<T, bool> VisibleSelector { get; set; }
    public required Func<T, object> DisplaySelector { get; set; }

    /// <summary>Ключ сортировки — значение свойства <see cref="Name"/>, не подпись из <see cref="DisplaySelector"/>.</summary>
    public required Expression<Func<T, IComparable>> SortKeySelector { get; set; }

    public bool HasMargin { get; set; } = false;
    public override string ToString()
    {
        return $"{GroupHeader}:{Header} ({Name})";
    }
}