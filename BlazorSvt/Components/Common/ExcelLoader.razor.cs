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

    private static readonly Dictionary<string, PropertyInfo> Properties =
        typeof(TItem)
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase);

    [Inject]
    public ILogger<ExcelLoader<TItem>> Logger { get; set; } = default!;

    public List<TItem> Items { get; private set; } = [];

    public MemoryStream? WorkbookCopy { get; private set; }

    public async Task OnFileSelectedAsync(InputFileChangeEventArgs args)
    {
        var file = args.File;
        if (file is null)
        {
            return;
        }

        try
        {
            await using var uploadStream = file.OpenReadStream(maxAllowedSize: 10 * 1024 * 1024);
            using var buffer = new MemoryStream();
            await uploadStream.CopyToAsync(buffer);

            ReplaceWorkbookCopy(buffer.ToArray());
            Items = ParseWorkbook(WorkbookCopy!);
        }
        catch (Exception ex)
        {
            Logger.LogError(ex, "Failed to load Excel file {FileName}", file.Name);
            Items = [];
            throw;
        }
    }

    private void ReplaceWorkbookCopy(byte[] content)
    {
        WorkbookCopy?.Dispose();
        WorkbookCopy = new MemoryStream(content);
    }

    private static List<TItem> ParseWorkbook(Stream stream)
    {
        stream.Position = 0;
        using var workbook = new XLWorkbook(stream);
        var worksheet = workbook.Worksheets.First();

        var columnMap = BuildColumnMap(worksheet);
        if (columnMap.Count == 0)
        {
            return [];
        }

        var items = new List<TItem>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? HeaderRowNumber;

        for (var rowNumber = DataStartRowNumber; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (IsEmptyRow(row, columnMap.Keys))
            {
                continue;
            }

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
}
