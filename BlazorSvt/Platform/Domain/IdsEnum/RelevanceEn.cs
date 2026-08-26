using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Актуальность (en). Значения = ItemId из legacy <c>dbo.vw_Relevance (dbo.PrimitiveEntityData_2108)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum RelevanceEn
{
    [Display(Name = "Month")]
    Month = 11610185,

    [Display(Name = "Quarter")]
    Quarter = 11610186,

    [Display(Name = "Year")]
    Year = 11610187,

    [Display(Name = "Request")]
    Request = 11610188
}
