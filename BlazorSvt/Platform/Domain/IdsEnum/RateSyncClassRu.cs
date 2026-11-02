using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Класс синхронизации ставок (ru). Значения = ItemId из legacy <c>dbo.vw_RateSyncClass (dbo.PrimitiveEntityData_2127)</c>.
/// Короткий словарь: не входит в sync-cascade.
/// </summary>
public enum RateSyncClassRu
{
    [Display(Name = "Сухие и опасные")]
    DryHazardous = 23266492,

    [Display(Name = "Сухие и неопасные")]
    DryNonHazardous = 23266493,

    [Display(Name = "Жидкие и опасные")]
    LiquidHazardous = 23266494,

    [Display(Name = "Жидкие и неопасные")]
    LiquidNonHazardous = 23266495,
}
