using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Валюта. Значения = ItemId из legacy <c>dbo.vw_Currency (dbo.PrimitiveEntityData_2016)</c>.
/// Стабильный справочник: не входит в sync-cascade (см. svt-development-patterns).
/// </summary>
public enum Currency
{
    [Display(Name = "CNY")]
    CNY = 32652,

    [Display(Name = "EUR")]
    EUR = 32628,

    [Display(Name = "RUB")]
    RUB = 32694,

    [Display(Name = "USD")]
    USD = 32623
}
