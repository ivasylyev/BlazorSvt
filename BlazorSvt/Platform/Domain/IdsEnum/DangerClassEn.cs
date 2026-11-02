using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Hazard class (en). Values = Id from dbo.vw_DangerClass.
/// Short dictionary: not part of the sync cascade.
/// </summary>
public enum DangerClassEn
{
    [Display(Name = "Explosives")]
    Explosives = 42873,

    [Display(Name = "Gases")]
    Gases = 42874,

    [Display(Name = "Flammable liquids")]
    FlammableLiquids = 42875,

    [Display(Name = "Flammable solids")]
    FlammableSolids = 42876,

    [Display(Name = "Oxidizing substances")]
    Oxidizers = 42877,

    [Display(Name = "Toxic and infectious substances")]
    ToxicAndInfectious = 42878,

    [Display(Name = "Radioactive substances")]
    Radioactive = 42879,

    [Display(Name = "Corrosives")]
    Corrosives = 42880,

    [Display(Name = "Miscellaneous dangerous goods")]
    MiscellaneousDangerous = 42881
}
