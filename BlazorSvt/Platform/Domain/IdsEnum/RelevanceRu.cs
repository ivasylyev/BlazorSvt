using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Актуальность (ru). Значения = ItemId из legacy <c>dbo.vw_Relevance (dbo.PrimitiveEntityData_2108)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum RelevanceRu
{
    [Display(Name = "Месяц")]
    Month = 11610185,

    [Display(Name = "Квартал")]
    Quarter = 11610186,

    [Display(Name = "Год")]
    Year = 11610187,

    [Display(Name = "Запрос")]
    Request = 11610188
}
