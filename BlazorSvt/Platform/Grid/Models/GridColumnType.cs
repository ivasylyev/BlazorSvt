namespace BlazorSvt.Platform.Grid.Models;

/// <summary>
/// Тип колонки в whitelist <c>@AllowedColumnsJson</c> для <c>v2.GetBlazorGridData</c>.
/// Должен совпадать с ожидаемыми значениями SP (ID / NVARCHAR / DATE / BIT).
/// </summary>
public enum GridColumnType
{
    Id,
    Nvarchar,
    Date,
    Bit
}
