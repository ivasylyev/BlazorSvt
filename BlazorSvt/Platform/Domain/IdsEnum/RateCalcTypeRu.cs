using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Способ расчёта ставки (ru). Значения = ItemId из legacy <c>dbo.vw_RateCalcType (dbo.PrimitiveEntityData_2092)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum RateCalcTypeRu
{
    [Display(Name = "За тонну")]
    PerTonne = 10126907,

    [Display(Name = "За транспортное средство")]
    PerVehicle = 10126908,
}
