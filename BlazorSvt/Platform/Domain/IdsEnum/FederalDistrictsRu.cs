using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Федеральный округ (ru). Значения = Id из dbo.vw_Federal_Districts.
/// Короткий словарь: в каскад синхронизации не входит. В данных бывает NULL.
/// </summary>
public enum FederalDistrictsRu
{
    [Display(Name = "Центральный федеральный округ")]
    Central = 16660670,

    [Display(Name = "Приволжский федеральный округ")]
    Volga = 16660671,

    [Display(Name = "Северо-Западный федеральный округ")]
    Northwestern = 16660672,

    [Display(Name = "Дальневосточный федеральный округ")]
    Far_Eastern = 16660673,

    [Display(Name = "Сибирский федеральный округ")]
    Siberian = 16660674,

    [Display(Name = "Южный федеральный округ")]
    Southern = 16660675,

    [Display(Name = "Северо-Кавказский федеральный округ")]
    North_Caucasian = 16660676,

    [Display(Name = "Уральский федеральный округ")]
    Ural = 16660677
}
