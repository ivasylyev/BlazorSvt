using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип транспорта (en). Значения = ItemId из legacy <c>dbo.vw_TransportType (dbo.PrimitiveEntityData_2009)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TransportTypeEn
{
    [Display(Name = "Auto FTL")]
    Auto = 543796,

    [Display(Name = "Auto_Barrel")]
    Auto_Barrel = 18063623,

    [Display(Name = "Auto container bulk")]
    Auto_BLK = 543797,

    [Display(Name = "Tank truck")]
    Auto_C = 543799,

    [Display(Name = "Auto container")]
    Auto_cont = 543798,

    [Display(Name = "Ferryboat")]
    Auto_ferryboat = 17838198,

    [Display(Name = "Polivoz")]
    Auto_polivoz = 17838200,

    [Display(Name = "Auto tank container")]
    Auto_TK = 543812,

    [Display(Name = "Loading Rack")]
    Loading_Rack = 543807,

    [Display(Name = "Multimodal")]
    Mix = 543810,

    [Display(Name = "Multimodal: Auto container for barrels")]
    Mix_auto_barrel = 28834719,

    [Display(Name = "Multimodal: Auto container")]
    Mix_auto_cont = 9687414,

    [Display(Name = "Multimodal: auto container to port")]
    Mix_auto_port = 28834720,

    [Display(Name = "Multimodal: Rail container")]
    Mix_rail_cont = 9687413,

    [Display(Name = "Multimodal: Sea")]
    Mix_sea = 9687415,

    [Display(Name = "Multimodal: sea refrigerated")]
    Mix_sea_ref = 32892871,

    [Display(Name = "Pipe")]
    Pipe = 2612365,

    [Display(Name = "Tank rail wagon 35 cbm (styrene)")]
    Rail_35 = 543800,

    [Display(Name = "Tank rail wagon 95 cbm (raw NGL)")]
    Rail_95 = 543801,

    [Display(Name = "Rail container bulk")]
    Rail_BLK = 543803,

    [Display(Name = "Rail container")]
    Rail_cont = 543804,

    [Display(Name = "Rail boxcar")]
    Rail_KV = 543805,

    [Display(Name = "Rail tank container")]
    Rail_TK = 543806,

    [Display(Name = "Tank rail wagon (other goods)")]
    Rail_VC_other = 543802,

    [Display(Name = "Sea")]
    Sea = 543808,

    [Display(Name = "Tanker")]
    Tanker = 543809,

    [Display(Name = "Transshipment_land")]
    Transshipment_land = 543813,

    [Display(Name = "Transshipment_sea")]
    Transshipment_sea = 543814,

    [Display(Name = "Transshipment_TK")]
    Transshipment_TK = 543815,
}
