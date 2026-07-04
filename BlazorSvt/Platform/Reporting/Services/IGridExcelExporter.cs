using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.Platform.Reporting.Services;

public interface IGridReportSession<T> : IDisposable
{
    void WriteBatch(IReadOnlyList<T> items, CancellationToken cancellationToken = default);

    void Complete(int totalRowCount);
}

public interface IGridExcelExporter
{
    IGridReportSession<T> BeginShortReport<T>(Stream output, IEnumerable<GridColumnSetting<T>> columns);

    IGridReportSession<T> BeginFullReport<T>(Stream output, IEnumerable<DetailSetting<T>> columns);
}
