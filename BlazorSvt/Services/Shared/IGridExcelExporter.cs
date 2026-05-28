using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridExcelExporter
{
    byte[] Export<T>(IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns);
}
