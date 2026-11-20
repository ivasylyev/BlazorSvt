using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Способ расчёта ставки (en). Значения = ItemId из legacy <c>dbo.vw_RateCalcType (dbo.PrimitiveEntityData_2092)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum RateCalcTypeEn
{
    [Display(Name = "Per tonne")]
    PerTonne = 10126907,

    [Display(Name = "Per vehicle")]
    PerVehicle = 10126908,
}
