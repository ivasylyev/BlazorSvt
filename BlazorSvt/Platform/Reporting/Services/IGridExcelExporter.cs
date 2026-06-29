using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.Platform.Reporting.Services;

public interface IGridExcelExporter
{
    byte[] ExportShortReport<T>(IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns);
    byte[] ExportFullReport<T>(IReadOnlyList<T> items, IEnumerable<DetailSetting<T>> columns);
}
