using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип коэффициента (en). Значения = ItemId из legacy <c>dbo.vw_CoefficientType (dbo.PrimitiveEntityData_2077)</c>.
/// Короткий словарь: не входит в sync-cascade.
/// </summary>
public enum CoefficientTypeEn
{
    [Display(Name = "Deflator")]
    Deflator = 9687424, // 1 Дефлятор

    [Display(Name = "Slush")]
    Slush = 9687425 // 2 Распутица
}
