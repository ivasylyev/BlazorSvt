using System.Collections.Concurrent;
using System.Globalization;
using System.Reflection;
using BlazorSvt.Import.Models;
using BlazorSvt.Host.Resources;
using ClosedXML.Excel;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Import.Services;

public class ExcelParser(IStringLocalizer<Svt> localizer) : IExcelParser
{
    private const int HeaderRowNumber = 3;
    private const int DataStartRowNumber = 4;

    private static readonly ConcurrentDictionary<Type, Dictionary<string, PropertyInfo>> PropertiesCache = new();

    public IReadOnlyList<ExcelRow<TItem>> Parse<TItem>(Stream workbook) where TItem : new()
    {
        var properties = GetWritableProperties(typeof(TItem));

        workbook.Position = 0;
        using var xlWorkbook = new XLWorkbook(workbook);
        var worksheet = xlWorkbook.Worksheets.First();

        var columnMap = BuildColumnMap(worksheet, properties);
        if (columnMap.Count == 0)
        {
            return [];
        }

        var rows = new List<ExcelRow<TItem>>();
        var lastRow = worksheet.LastRowUsed()?.RowNumber() ?? HeaderRowNumber;

        for (var rowNumber = DataStartRowNumber; rowNumber <= lastRow; rowNumber++)
        {
            var row = worksheet.Row(rowNumber);
            if (IsEmptyRow(row, columnMap.Keys))
            {
                continue;
            }

            var item = new TItem();
            var parseErrors = new List<ValidationError>();

            foreach (var (columnNumber, property) in columnMap)
            {
                var cellValue = row.Cell(columnNumber).GetString();
                if (string.IsNullOrWhiteSpace(cellValue))
                {
                    continue;
                }

                if (TryConvertCellValue(cellValue, property.PropertyType, out var converted))
                {
                    property.SetValue(item, converted);
                }
                else
                {
                    parseErrors.Add(new ValidationError(
                        property.Name,
                        localizer["Validation.ParseError", cellValue, property.Name]));
                }
            }

            rows.Add(new ExcelRow<TItem>(rowNumber, item, parseErrors));
        }

        return rows;
    }

    private static Dictionary<string, PropertyInfo> GetWritableProperties(Type type) =>
        PropertiesCache.GetOrAdd(type, t => t
            .GetProperties(BindingFlags.Public | BindingFlags.Instance)
            .Where(p => p.CanWrite)
            .ToDictionary(p => p.Name, StringComparer.OrdinalIgnoreCase));

    private static Dictionary<int, PropertyInfo> BuildColumnMap(
        IXLWorksheet worksheet,
        IReadOnlyDictionary<string, PropertyInfo> properties)
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

            if (properties.TryGetValue(header, out var property))
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

    private static bool TryConvertCellValue(string value, Type targetType, out object? result)
    {
        var underlyingType = Nullable.GetUnderlyingType(targetType) ?? targetType;

        if (underlyingType == typeof(string))
        {
            result = value;
            return true;
        }

        if (underlyingType.IsEnum)
        {
            if (Enum.TryParse(underlyingType, value, ignoreCase: true, out var enumValue))
            {
                result = enumValue;
                return true;
            }

            result = null;
            return false;
        }

        if (underlyingType == typeof(DateOnly))
        {
            if (DateOnly.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateOnly))
            {
                result = dateOnly;
                return true;
            }

            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
            {
                result = DateOnly.FromDateTime(dateTime);
                return true;
            }

            result = null;
            return false;
        }

        if (underlyingType == typeof(DateTime))
        {
            if (DateTime.TryParse(value, CultureInfo.CurrentCulture, DateTimeStyles.None, out var dateTime))
            {
                result = dateTime;
                return true;
            }

            result = null;
            return false;
        }

        if (underlyingType == typeof(bool))
        {
            if (bool.TryParse(value, out var boolValue))
            {
                result = boolValue;
                return true;
            }

            result = value is "1" or "да" or "yes" or "Yes" or "Да";
            return true;
        }

        try
        {
            result = Convert.ChangeType(value, underlyingType, CultureInfo.CurrentCulture);
            return true;
        }
        catch (Exception ex) when (ex is FormatException or InvalidCastException or OverflowException)
        {
            result = null;
            return false;
        }
    }
}
