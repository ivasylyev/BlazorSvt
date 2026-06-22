using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridExcelExporter
{
    byte[] ExportShortReport<T>(IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns);
    byte[] ExportFullReport<T>(IReadOnlyList<T> items, IEnumerable<DetailSetting<T>> columns);
}
