using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace BlazorSvt.Platform.Domain.IdsEnum;

public enum TransportTypeLevel3En
{
    // [Display(Name = "Auto FTL 1 ton")]
    // Auto_1 = 543820,

    [Display(Name = "Auto FTL 10 tons")]
    Auto_10 = 543817,

    [Display(Name = "Auto FTL 20 tons")]
    Auto_20 = 543816,

    // [Display(Name = "Auto FTL 4 tons")]
    // Auto_4 = 543819,

    // [Display(Name = "Auto FTL 6 tons")]
    // Auto_6 = 543818,

    [Display(Name = "Auto_Barrel_20")]
    Auto_Barrel_20 = 18063624,

    [Display(Name = "Auto container bulk")]
    Auto_BLK = 543821,

    [Display(Name = "Tank truck")]
    Auto_C = 543824,

    [Display(Name = "Auto container 20ft")]
    Auto_cont_20 = 543822,

    [Display(Name = "Auto container 40ft")]
    Auto_cont_40 = 543823,

    [Display(Name = "Ferryboat 20t")]
    Auto_ferryboat_20t = 17838199,

    [Display(Name = "Polivoz")]
    Auto_polivoz = 17838201,

    [Display(Name = "Auto tank container")]
    Auto_TK = 543842,

    // [Display(Name = "Loading_Rack")]
    // Loading_Rack = 543834,

    [Display(Name = "Mix container 20ft")]
    Mix_20 = 543840,

    [Display(Name = "Mix container 40ft")]
    Mix_40 = 543841,

    [Display(Name = "Mix: Auto container for barrels")]
    Mix_auto_barrel = 28834725,

    [Display(Name = "Mix: Auto container 20ft")]
    Mix_auto_cont_20 = 9687418,

    [Display(Name = "Mix: Auto container 40ft")]
    Mix_auto_cont_40 = 9687419,

    [Display(Name = "Mix: auto container to port")]
    Mix_auto_port = 28834726,

    [Display(Name = "Mix Auto tank container")]
    Mix_auto_TK = 21959144,

    [Display(Name = "Mix: Rail container 20ft")]
    Mix_rail_cont_20 = 9687416,

    [Display(Name = "Mix: Rail container 40ft")]
    Mix_rail_cont_40 = 9687417,

    // [Display(Name = "Mix: Rail container 40ft second level")]
    // Mix_rail_cont_40_1 = 34800231,

    // [Display(Name = "Mix: Rail container 40ft third level")]
    // Mix_rail_cont_40_2 = 34800232,

    // [Display(Name = "Mix: Rail container 40ft forth level")]
    // Mix_rail_cont_40_3 = 34800233,

    // [Display(Name = "Mix: Rail container 40ft fifth level")]
    // Mix_rail_cont_40_4 = 34800234,

    // [Display(Name = "Mix: Rail container 40ft sixth level")]
    // Mix_rail_cont_40_5 = 34800235,

    // [Display(Name = "Mix: Rail container 40ft seventh level")]
    // Mix_rail_cont_40_6 = 34800236,

    [Display(Name = "Mix Rail tank container")]
    Mix_rail_TK = 21959145,

    [Display(Name = "Mix: Sea")]
    Mix_sea = 9687420,

    // [Display(Name = "Mix: Sea second level")]
    // Mix_sea_1 = 34800225,

    // [Display(Name = "Mix: Sea third level")]
    // Mix_sea_2 = 34800226,

    // [Display(Name = "Mix: Sea fourth level")]
    // Mix_sea_3 = 34800227,

    // [Display(Name = "Mix: Sea fifth level")]
    // Mix_sea_4 = 34800228,

    // [Display(Name = "Mix: Sea sixth level")]
    // Mix_sea_5 = 34800229,

    // [Display(Name = "Mix: Sea seventh level")]
    // Mix_sea_6 = 34800230,

    // [Display(Name = "Mix: sea refrigerated")]
    // Mix_sea_ref = 32892872,

    [Display(Name = "Mix tank container")]
    Mix_TK = 2448668,

    // [Display(Name = "Pipe")]
    // Pipe = 2612366,

    [Display(Name = "Tank rail wagon 35 cbm (styrene)")]
    Rail_35 = 543825,

    [Display(Name = "Tank rail wagon 95 cbm (raw NGL)")]
    Rail_95 = 543826,

    [Display(Name = "Rail container bulk")]
    Rail_BLK = 543828,

    [Display(Name = "Rail container 20ft")]
    Rail_cont_20 = 543829,

    [Display(Name = "Rail container 40ft")]
    Rail_cont_40 = 543830,

    [Display(Name = "Rail boxcar")]
    Rail_KV = 543831,

    [Display(Name = "Rail tank container 20ft")]
    Rail_TK_20 = 543832,

    [Display(Name = "Rail tank container 30ft")]
    Rail_TK_30 = 543833,

    [Display(Name = "Tank rail wagon (other goods)")]
    Rail_VC_other = 543827,

    // [Display(Name = "Sea")]
    // Sea = 543835,

    // [Display(Name = "Tanker")]
    // Tanker = 543839,

    // [Display(Name = "Tanker_ECO")]
    // Tanker_ECO = 543836,

    // [Display(Name = "Tanker Handysize")]
    // Tanker_HS = 543837,

    // [Display(Name = "Tanker_MGC")]
    // Tanker_MGC = 543838,

    // [Display(Name = "Transshipment_land")]
    // Transshipment_land = 543843,

    // [Display(Name = "Transshipment_sea")]
    // Transshipment_sea = 543844,

    // [Display(Name = "Transshipment_TK")]
    // Transshipment_TK = 543845,
}
