using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.Platform.Reporting.Services;

public interface IGridExcelExporter
{
    void ExportShortReport<T>(Stream output, IReadOnlyList<T> items, IEnumerable<GridColumnSetting<T>> columns);

    void ExportFullReport<T>(Stream output, IReadOnlyList<T> items, IEnumerable<DetailSetting<T>> columns);
}
