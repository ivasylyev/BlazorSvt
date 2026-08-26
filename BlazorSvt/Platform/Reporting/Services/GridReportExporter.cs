using BlazorBootstrap;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Reporting.Services;

/// <summary>
/// Пакетная выгрузка grid/detail в Excel: цикл батчей + скачивание файла.
/// UI (confirm/preload/cancel) остаётся в <c>GenericGrid</c>.
/// </summary>
public sealed class GridReportExporter<TItem, TDetailItem>(
    IGridDataService<TItem, TDetailItem> dataService,
    IGridExcelExporter excelExporter,
    IFileDownloadService fileDownloadService,
    IOptions<ReportOptions> reportOptions,
    ILogger<GridReportExporter<TItem, TDetailItem>> logger)
{
    public async Task ExportShortAsync(
        GridDataProviderRequest<TItem> baseRequest,
        IEnumerable<GridColumnSetting<TItem>> columns,
        int totalCount,
        string fileName,
        CancellationToken cancellationToken)
    {
        var batchSize = reportOptions.Value.ReportBatchSize;

        await using var stream = new MemoryStream();
        var session = excelExporter.BeginShortReport(stream, columns);

        try
        {
            await WriteBatchesAsync(
                pageNumber => dataService.GetShortReportBatchAsync(
                    baseRequest, pageNumber, batchSize, cancellationToken),
                session,
                batchSize,
                cancellationToken);

            session.Complete(totalCount);
        }
        finally
        {
            session.Dispose();
        }

        stream.Position = 0;
        await fileDownloadService.DownloadFromStreamAsync(stream, fileName, cancellationToken);
        logger.LogInformation("Short report: exported {Count} items", totalCount);
    }

    public async Task ExportFullAsync(
        GridDataProviderRequest<TItem> baseRequest,
        IEnumerable<DetailSetting<TDetailItem>> columns,
        int totalCount,
        string fileName,
        CancellationToken cancellationToken)
    {
        var batchSize = reportOptions.Value.ReportBatchSize;

        await using var stream = new MemoryStream();
        var session = excelExporter.BeginFullReport(stream, columns);

        try
        {
            await WriteBatchesAsync(
                pageNumber => dataService.GetFullReportBatchAsync(
                    baseRequest, pageNumber, batchSize, cancellationToken),
                session,
                batchSize,
                cancellationToken);

            session.Complete(totalCount);
        }
        finally
        {
            session.Dispose();
        }

        stream.Position = 0;
        await fileDownloadService.DownloadFromStreamAsync(stream, fileName, cancellationToken);
        logger.LogInformation("Full report: exported {Count} items", totalCount);
    }

    private static async Task WriteBatchesAsync<T>(
        Func<int, Task<IReadOnlyList<T>>> fetchBatch,
        IGridReportSession<T> session,
        int batchSize,
        CancellationToken cancellationToken)
    {
        for (var pageNumber = 1; ; pageNumber++)
        {
            cancellationToken.ThrowIfCancellationRequested();

            var batch = await fetchBatch(pageNumber);
            if (batch.Count == 0)
                break;

            session.WriteBatch(batch, cancellationToken);

            if (batch.Count < batchSize)
                break;
        }
    }
}
