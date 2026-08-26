using LargeXlsx;

namespace BlazorSvt.Platform.Reporting.Services;

public class GridExcelExporter : IGridExcelExporter
{
    private const double DefaultColumnWidth = 18;
    private const string WorksheetName = "Report";

    private static readonly XlsxStyle HeaderStyle = XlsxStyle.Default.With(XlsxFont.Default.WithBold());
    private static readonly XlsxStyle DateStyle = XlsxStyle.Default.With(XlsxNumberFormat.ShortDate);

    private sealed record ExportColumn<T>(string Header, Func<T, object?> GetValue);

    public IGridReportSession<T> BeginShortReport<T>(Stream output, IEnumerable<GridColumnSetting<T>> columns) =>
        BeginReport(
            output,
            columns
                .Where(c => c.Visible)
                .Select(c => new ExportColumn<T>(c.Header, item => c.DisplaySelector(item)))
                .ToList());

    public IGridReportSession<T> BeginFullReport<T>(Stream output, IEnumerable<DetailSetting<T>> columns) =>
        BeginReport(
            output,
            columns
                .Select(c => new ExportColumn<T>(
                    c.Header,
                    item => c.VisibleSelector(item) ? c.DisplaySelector(item) : null))
                .ToList());

    private static IGridReportSession<T> BeginReport<T>(Stream output, IReadOnlyList<ExportColumn<T>> columns)
    {
        if (columns.Count == 0)
            throw new InvalidOperationException("Report must have at least one column.");

        return new GridReportSession<T>(output, columns);
    }

    private sealed class GridReportSession<T> : IGridReportSession<T>
    {
        private readonly XlsxWriter writer;
        private readonly IReadOnlyList<ExportColumn<T>> columns;
        private bool completed;

        public GridReportSession(Stream output, IReadOnlyList<ExportColumn<T>> columns)
        {
            this.columns = columns;
            writer = new XlsxWriter(output, requireCellReferences: false);

            writer.BeginWorksheet(
                WorksheetName,
                splitRow: 1,
                columns: [XlsxColumn.Formatted(DefaultColumnWidth, count: columns.Count)]);

            writer.BeginRow();
            foreach (var column in columns)
                writer.WriteSharedString(column.Header, HeaderStyle);
        }

        public void WriteBatch(IReadOnlyList<T> items, CancellationToken cancellationToken = default)
        {
            ObjectDisposedException.ThrowIf(completed, this);

            foreach (var item in items)
            {
                cancellationToken.ThrowIfCancellationRequested();
                writer.BeginRow();
                foreach (var column in columns)
                    WriteCellValue(writer, column.GetValue(item));
            }
        }

        public void Complete(int totalRowCount)
        {
            ObjectDisposedException.ThrowIf(completed, this);

            writer.SetAutoFilter(1, 1, totalRowCount + 1, columns.Count);
            completed = true;
        }

        public void Dispose()
        {
            writer.Dispose();
        }
    }

    private static void WriteCellValue(XlsxWriter writer, object? value)
    {
        switch (value)
        {
            case null:
                writer.Write();
                break;
            case DateOnly dateOnly:
                writer.Write(dateOnly.ToDateTime(TimeOnly.MinValue), DateStyle);
                break;
            case DateTime dateTime:
                writer.Write(dateTime, DateStyle);
                break;
            case bool boolean:
                writer.Write(boolean);
                break;
            case int intValue:
                writer.Write(intValue);
                break;
            case long longValue:
                writer.Write((double)longValue);
                break;
            case decimal decimalValue:
                writer.Write(decimalValue);
                break;
            case double doubleValue:
                writer.Write(doubleValue);
                break;
            case string stringValue:
                writer.WriteSharedString(stringValue);
                break;
            default:
                writer.WriteSharedString(value.ToString() ?? string.Empty);
                break;
        }
    }
}
