using BlazorBootstrap;
using Microsoft.Extensions.Options;

namespace BlazorSvt.Platform.Reporting.Services;

/// <summary>
/// Пакетная выгрузка grid/detail в Excel: цикл батчей + скачивание файла.
/// UI (confirm/preload/cancel) остаётся в <c>GenericGrid</c>.
/// </summary>
/// <remarks>
/// <c>Write*Async</c> пишут книгу в произвольный поток без JS — их использует API (<c>/export</c>).
/// </remarks>
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
        await WriteAsync(
            excelExporter.BeginShortReport(stream, columns),
            pageNumber => dataService.GetShortReportBatchAsync(
                baseRequest, pageNumber, batchSize, cancellationToken),
            batchSize,
            cancellationToken);

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
        await WriteAsync(
            excelExporter.BeginFullReport(stream, columns),
            pageNumber => dataService.GetFullReportBatchAsync(
                baseRequest, pageNumber, batchSize, cancellationToken),
            batchSize,
            cancellationToken);

        stream.Position = 0;
        await fileDownloadService.DownloadFromStreamAsync(stream, fileName, cancellationToken);
        logger.LogInformation("Full report: exported {Count} items", totalCount);
    }

    /// <summary>Краткий отчёт в <paramref name="output"/>; фильтры и сортировка — из <paramref name="query"/>.</summary>
    /// <returns>Число выгруженных строк.</returns>
    public Task<int> WriteShortAsync(
        Stream output,
        GridQuery query,
        IEnumerable<GridColumnSetting<TItem>> columns,
        CancellationToken cancellationToken)
    {
        var batchSize = reportOptions.Value.ReportBatchSize;
        return WriteAsync(
            excelExporter.BeginShortReport(output, columns),
            pageNumber => dataService.GetShortReportBatchAsync(
                query with { PageNumber = pageNumber, PageSize = batchSize }, cancellationToken),
            batchSize,
            cancellationToken);
    }

    /// <summary>Полный отчёт (поля карточки) в <paramref name="output"/>.</summary>
    /// <returns>Число выгруженных строк.</returns>
    public Task<int> WriteFullAsync(
        Stream output,
        GridQuery query,
        IEnumerable<DetailSetting<TDetailItem>> columns,
        CancellationToken cancellationToken)
    {
        var batchSize = reportOptions.Value.ReportBatchSize;
        return WriteAsync(
            excelExporter.BeginFullReport(output, columns),
            pageNumber => dataService.GetFullReportBatchAsync(
                query with { PageNumber = pageNumber, PageSize = batchSize }, cancellationToken),
            batchSize,
            cancellationToken);
    }

    private static async Task<int> WriteAsync<T>(
        IGridReportSession<T> session,
        Func<int, Task<IReadOnlyList<T>>> fetchBatch,
        int batchSize,
        CancellationToken cancellationToken)
    {
        using (session)
        {
            var written = 0;
            for (var pageNumber = 1; ; pageNumber++)
            {
                cancellationToken.ThrowIfCancellationRequested();

                var batch = await fetchBatch(pageNumber);
                if (batch.Count == 0)
                    break;

                session.WriteBatch(batch, cancellationToken);
                written += batch.Count;

                if (batch.Count < batchSize)
                    break;
            }

            session.Complete(written);
            return written;
        }
    }
}
