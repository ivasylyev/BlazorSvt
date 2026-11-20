using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Federal district (en). Values = Id from dbo.vw_Federal_Districts.
/// Short dictionary: not part of the sync cascade. NULL occurs in data.
/// </summary>
public enum FederalDistrictsEn
{
    [Display(Name = "Central")]
    Central = 16660670,

    [Display(Name = "Volga")]
    Volga = 16660671,

    [Display(Name = "Northwestern")]
    Northwestern = 16660672,

    [Display(Name = "Far Eastern")]
    Far_Eastern = 16660673,

    [Display(Name = "Siberian")]
    Siberian = 16660674,

    [Display(Name = "Southern")]
    Southern = 16660675,

    [Display(Name = "North Caucasian")]
    North_Caucasian = 16660676,

    [Display(Name = "Ural")]
    Ural = 16660677
}
