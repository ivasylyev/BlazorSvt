using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Класс опасности (ru). Значения = Id из dbo.vw_DangerClass.
/// Короткий словарь: в каскад синхронизации не входит.
/// </summary>
public enum DangerClassRu
{
    [Display(Name = "Взрывчатые вещества")]
    Explosives = 42873,

    [Display(Name = "Газы")]
    Gases = 42874,

    [Display(Name = "Легковоспламеняющиеся жидкости")]
    FlammableLiquids = 42875,

    [Display(Name = "Легковоспламеняющиеся вещества и материалы")]
    FlammableSolids = 42876,

    [Display(Name = "Окисляющие вещества и органические пероксиды")]
    Oxidizers = 42877,

    [Display(Name = "Ядовитые и инфекционные вещества")]
    ToxicAndInfectious = 42878,

    [Display(Name = "Радиоактивные вещества")]
    Radioactive = 42879,

    [Display(Name = "Едкие и коррозионные вещества")]
    Corrosives = 42880,

    [Display(Name = "Прочие опасные вещества и изделия")]
    MiscellaneousDangerous = 42881
}
