using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace BlazorSvt.Models.Dto;

public enum TransportTypeLevel3Ru
{
    // [Display(Name = "Авто фура 1,5 т")]
    // Auto_1 = 543820,

    [Display(Name = "Авто фура 10 т")]
    Auto_10 = 543817,

    [Display(Name = "Авто фура 20 т")]
    Auto_20 = 543816,

    // [Display(Name = "Авто фура 4 т")]
    // Auto_4 = 543819,

    // [Display(Name = "Авто фура 6 т")]
    // Auto_6 = 543818,

    [Display(Name = "Авто_Бочки_20")]
    Auto_Barrel_20 = 18063624,

    [Display(Name = "Авто контейнер балк")]
    Auto_BLK = 543821,

    [Display(Name = "Авто цистерна")]
    Auto_C = 543824,

    [Display(Name = "Авто контейнер 20 фт")]
    Auto_cont_20 = 543822,

    [Display(Name = "Авто контейнер 40 фт")]
    Auto_cont_40 = 543823,

    [Display(Name = "Паром 20т")]
    Auto_ferryboat_20t = 17838199,

    [Display(Name = "Полимеровоз")]
    Auto_polivoz = 17838201,

    [Display(Name = "Авто танк-контейнер")]
    Auto_TK = 543842,

    // [Display(Name = "Эстакада")]
    // Loading_Rack = 543834,

    [Display(Name = "Мультимодальный 20 фт")]
    Mix_20 = 543840,

    [Display(Name = "Мультимодальный 40 фт")]
    Mix_40 = 543841,

    [Display(Name = "Мультимодальный: авто контейнер для бочек")]
    Mix_auto_barrel = 28834725,

    [Display(Name = "Мультимодальный: авто контейнер 20 фт")]
    Mix_auto_cont_20 = 9687418,

    [Display(Name = "Мультимодальный: авто контейнер 40 фт")]
    Mix_auto_cont_40 = 9687419,

    [Display(Name = "Мультимодальный: авто контейнер до порта")]
    Mix_auto_port = 28834726,

    [Display(Name = "Мультимодальный авто танк-контейнер")]
    Mix_auto_TK = 21959144,

    [Display(Name = "Мультимодальный: жд контейнер 20 фт")]
    Mix_rail_cont_20 = 9687416,

    [Display(Name = "Мультимодальный: жд контейнер 40 фт")]
    Mix_rail_cont_40 = 9687417,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт вторая ступень")]
    // Mix_rail_cont_40_1 = 34800231,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт третья ступень")]
    // Mix_rail_cont_40_2 = 34800232,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт четвертая ступень")]
    // Mix_rail_cont_40_3 = 34800233,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт пятая ступень")]
    // Mix_rail_cont_40_4 = 34800234,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт шестая ступень")]
    // Mix_rail_cont_40_5 = 34800235,

    // [Display(Name = "Мультимодальный: жд контейнер 40 фт седьмая ступень")]
    // Mix_rail_cont_40_6 = 34800236,

    [Display(Name = "Мультимодальный жд танк-контейнер")]
    Mix_rail_TK = 21959145,

    [Display(Name = "Мультимодальный: морской")]
    Mix_sea = 9687420,

    // [Display(Name = "Мультимодальный: морской вторая ступень")]
    // Mix_sea_1 = 34800225,

    // [Display(Name = "Мультимодальный: морской третья ступень")]
    // Mix_sea_2 = 34800226,

    // [Display(Name = "Мультимодальный: морской четвертая ступень")]
    // Mix_sea_3 = 34800227,

    // [Display(Name = "Мультимодальный: морской пятая ступень")]
    // Mix_sea_4 = 34800228,

    // [Display(Name = "Мультимодальный: морской шестая ступень")]
    // Mix_sea_5 = 34800229,

    // [Display(Name = "Мультимодальный: морской седьмая ступень")]
    // Mix_sea_6 = 34800230,

    // [Display(Name = "Мультимодальный: морской рефрижераторный")]
    // Mix_sea_ref = 32892872,

    [Display(Name = "Мультимодальный танк-контейнер")]
    Mix_TK = 2448668,

    // [Display(Name = "Трубопровод")]
    // Pipe = 2612366,

    [Display(Name = "ВЦ стирол 35 куб")]
    Rail_35 = 543825,

    [Display(Name = "ВЦ ШФЛУ 95 куб")]
    Rail_95 = 543826,

    [Display(Name = "ЖД контейнер балк")]
    Rail_BLK = 543828,

    [Display(Name = "ЖД контейнер 20 фт")]
    Rail_cont_20 = 543829,

    [Display(Name = "ЖД контейнер 40 фт")]
    Rail_cont_40 = 543830,

    [Display(Name = "ЖД крытый вагон")]
    Rail_KV = 543831,

    [Display(Name = "ЖД танк-контейнер 20 фт")]
    Rail_TK_20 = 543832,

    [Display(Name = "ЖД танк-контейнер 30 фт")]
    Rail_TK_30 = 543833,

    [Display(Name = "ВЦ прочие грузы")]
    Rail_VC_other = 543827,

    // [Display(Name = "Контейнеровоз")]
    // Sea = 543835,

    // [Display(Name = "Танкер")]
    // Tanker = 543839,

    // [Display(Name = "Танкер ECO")]
    // Tanker_ECO = 543836,

    // [Display(Name = "Танкер Handysize")]
    // Tanker_HS = 543837,

    // [Display(Name = "Танкер MGC")]
    // Tanker_MGC = 543838,

    // [Display(Name = "Перевалка: сухопутная перевалка СУГ и ЖХ")]
    // Transshipment_land = 543843,

    // [Display(Name = "Перевалка: Морская перевалка")]
    // Transshipment_sea = 543844,

    // [Display(Name = "Перевалка: Перевалка танк-контейнеров в АРА")]
    // Transshipment_TK = 543845,
}