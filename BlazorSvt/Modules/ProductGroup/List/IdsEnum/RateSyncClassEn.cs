using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.ProductGroup.List.IdsEnum;

/// <summary>
/// Класс синхронизации ставок (en). Значения = ItemId из legacy <c>dbo.vw_RateSyncClass (dbo.PrimitiveEntityData_2127)</c>.
/// Короткий словарь: не входит в sync-cascade.
/// </summary>
public enum RateSyncClassEn
{
    [Display(Name = "Dry, hazardous")]
    DryHazardous = 23266492,

    [Display(Name = "Dry, non-hazardous")]
    DryNonHazardous = 23266493,

    [Display(Name = "Liquid, hazardous")]
    LiquidHazardous = 23266494,

    [Display(Name = "Liquid, non-hazardous")]
    LiquidNonHazardous = 23266495,
}
