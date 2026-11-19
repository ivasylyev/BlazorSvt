using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип отчёта (ru). Значения = ItemId из legacy <c>dbo.vw_ReportDictionaryTypes (dbo.PrimitiveEntityData_2129)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum ReportDictionaryTypesRu
{
    [Display(Name = "Ставки")]
    Rates = 23869944, // 1 Ставки

    [Display(Name = "Средневзвешенные ставки")]
    AverageRates = 23869945 // 2 Средневзвешенные ставки
}
