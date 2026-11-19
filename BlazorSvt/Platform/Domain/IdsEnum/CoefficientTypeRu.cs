using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип коэффициента (ru). Значения = ItemId из legacy <c>dbo.vw_CoefficientType (dbo.PrimitiveEntityData_2077)</c>.
/// Короткий словарь: не входит в sync-cascade.
/// </summary>
public enum CoefficientTypeRu
{
    [Display(Name = "Дефлятор")]
    Deflator = 9687424, // 1 Дефлятор

    [Display(Name = "Распутица")]
    Slush = 9687425 // 2 Распутица
}
