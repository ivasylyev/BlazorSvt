using System.Globalization;
using System.Reflection;
using ClosedXML.Excel;
using Microsoft.AspNetCore.Components;
using Microsoft.AspNetCore.Components.Forms;

namespace BlazorSvt.Components.Common;

public partial class ExcelLoader<TItem> : SvtComponentBase, IDisposable where TItem : new()
{
    private const int HeaderRowNumber = 3;
    private const int DataStartRowNumber = 4;
    private const double CopyBufferProgressPercent = 10;
    private const double WorkbookCreateProgressPercent = 20;
    private const double RowsReadProgressPercent = 70;

    private static readonly Dictionary<string, PropertyInfo> Properties =
        typeof(TItem)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

    private const int ProgressUpdateRowBatch = 25;

    [Inject]
    public ILogger<ExcelLoader<TItem>> Logger { get; set; } = default!;

    private bool _isLoading;
    private bool _showProgress;
    private double _progressPercent;

    public List<TItem> Items { get; private set; } = [];

    public MemoryStream? WorkbookCopy { get; private set; }

    public async Task OnFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file is null)
        {
            return;
        }

        await StartProgressAsync();

        try
        {
            await using var uploadStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            using var buffer = new MemoryStream();
            await uploadStream.CopyToAsync(buffer);
            await SetProgressStageAsync(ExcelLoaderStage.BufferCopyCompleted);

            ReplaceWorkbookCopy(buffer.ToArray());
            Items = await ParseWorkbookAsync(WorkbookCopy!);
            await CompleteProgressAsync();
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Excel file {FileName}", file.Name);
            Items = [];
            throw;
        }
        finally
        {
            await FinishProgressAsync();
        }
    }

    private void ReplaceWorkbookCopy(byte[] content)
    {
        WorkbookCopy?.Dispose();
        WorkbookCopy = new MemoryStream(content);
    }

    private async Task<List<TItem>> ParseWorkbookAsync(Stream stream)
    {
        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        await SetProgressStageAsync(ExcelLoaderStage.WorkbookCreated);
        var worksheet = workbook.Worksheets.First();

        var columnMap = BuildColumnMap(worksheet);
        if (columnMap.Count == 0)
        {
            return [];
        }

        var items = new List<TItem>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? HeaderRowNumber;
        var totalRows = Math.Max(1, lastRow - DataStartRowNumber + 1);
        var processedRows = 0;

        for (var rowNumber = DataStartRowNumber; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (!IsEmptyRow(row, columnMap.Keys))
            {
                var item = new TItem();
                foreach (var (columnNumber, property) in columnMap)
                {
                    var cellValue = row.Cell(columnNumber).GetString();
                    if (string.IsNullOrWhiteSpace(cellValue))
                    {
                        continue;
                    }

                    var converted = ConvertCellValue(cellValue, property.PropertyType);
                    property.SetValue(item, converted);
                }

                items.Add(item);
            }

            processedRows++;
            UpdateRowsReadProgress(processedRows, totalRows);

            if (processedRows % ProgressUpdateRowBatch == 0 || processedRows == totalRows)
            {
                await InvokeAsync(StateHasChanged);
            }
        }

        return items;
    }

    private static Dictionary<int, PropertyInfo> BuildColumnMap(IXLWorksheet worksheet)
    {
        var headerRow = worksheet.Row(HeaderRowNumber);
        var lastColumn = worksheet.LastColumnUsed()?.ColumnNumber() ?? 0;
        var columnMap = new Dictionary<int, PropertyInfo>();

        for (var columnNumber = 1; columnNumber <= lastColumn; columnNumber++)
        {
            var header = headerRow.Cell(columnNumber).GetString().Trim();
            if (string.IsNullOrEmpty(header))
            {
                continue;
            }

            if (Properties.TryGetValue(header, out var property))
            {
                columnMap[columnNumber] = property;
            }
        }

        return columnMap;
    }

    private static bool IsEmptyRow(IXLRow row, IEnumerable<int> columnNumbers)
    {
        return columnNumbers.All(columnNumber => string.IsNullOrWhiteSpace(row.Cell(columnNumber).GetString()));
    }

    private static object? ConvertCellValue(string value, Type targetType)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
        {
            return value;
        }

        if (underlyingType.IsEnum)
        {
            return Enum.Parse(underlyingType, value, ignoreCase: true);
        }

        if (underlyingType == typeof(DateOnly))
        {
            if (DateOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateOnly))
            {
                return dateOnly;
            }

            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
            {
                return DateOnly.FromDateTime(dateTime);
            }

            return null;
        }

        if (underlyingType == typeof(DateTime))
        {
            return DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime)
                ? dateTime
                : null;
        }

        if (underlyingType == typeof(bool))
        {
            if (bool.TryParse(value, out var boolValue))
            {
                return boolValue;
            }

            return value is "1" or "да" or "yes" or "Yes" or "Да";
        }

        return Convert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture);
    }

    public void Dispose()
    {
        WorkbookCopy?.Dispose();
    }

    private enum ExcelLoaderStage
    {
        BufferCopyCompleted,
        WorkbookCreated
    }

    private async Task StartProgressAsync()
    {
        _isLoading = true;
        _showProgress = true;
        _progressPercent = 0;
        await InvokeAsync(StateHasChanged);
    }

    private async Task SetProgressStageAsync(ExcelLoaderStage stage)
    {
        _progressPercent = stage switch
        {
            ExcelLoaderStage.BufferCopyCompleted => CopyBufferProgressPercent,
            ExcelLoaderStage.WorkbookCreated => CopyBufferProgressPercent + WorkbookCreateProgressPercent,
            _ => _progressPercent
        };

        await InvokeAsync(StateHasChanged);
    }

    private void UpdateRowsReadProgress(int processedRows, int totalRows)
    {
        var rowsProgress = processedRows / (double)totalRows;
        _progressPercent = CopyBufferProgressPercent
                           + WorkbookCreateProgressPercent
                           + rowsProgress * RowsReadProgressPercent;
    }

    private async Task CompleteProgressAsync()
    {
        _progressPercent = 100;
        await InvokeAsync(StateHasChanged);
    }

    private async Task FinishProgressAsync()
    {
        _isLoading = false;
        await InvokeAsync(StateHasChanged);
        await Task.Delay(500);
        _showProgress = false;
        await InvokeAsync(StateHasChanged);
    }
}
