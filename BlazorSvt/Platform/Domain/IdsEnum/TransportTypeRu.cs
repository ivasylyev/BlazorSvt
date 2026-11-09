using System.ComponentModel.DataAnnotations;
// ReSharper disable InconsistentNaming

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип транспорта (ru). Значения = ItemId из legacy <c>dbo.vw_TransportType (dbo.PrimitiveEntityData_2009)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TransportTypeRu
{
    [Display(Name = "Авто полуприцеп фура")]
    Auto = 543796,

    [Display(Name = "Авто_Бочки")]
    Auto_Barrel = 18063623,

    [Display(Name = "Авто контейнер балк")]
    Auto_BLK = 543797,

    [Display(Name = "Авто цистерна")]
    Auto_C = 543799,

    [Display(Name = "Авто контейнер на контейнеровозе")]
    Auto_cont = 543798,

    [Display(Name = "Паром")]
    Auto_ferryboat = 17838198,

    [Display(Name = "Полимеровоз")]
    Auto_polivoz = 17838200,

    [Display(Name = "Авто танк-контейнер")]
    Auto_TK = 543812,

    [Display(Name = "Эстакада")]
    Loading_Rack = 543807,

    [Display(Name = "Мультимодальный")]
    Mix = 543810,

    [Display(Name = "Мультимодальный: авто контейнер для бочек")]
    Mix_auto_barrel = 28834719,

    [Display(Name = "Мультимодальный: авто контейнер")]
    Mix_auto_cont = 9687414,

    [Display(Name = "Мультимодальный: авто контейнер до порта")]
    Mix_auto_port = 28834720,

    [Display(Name = "Мультимодальный: жд контейнер универсальный")]
    Mix_rail_cont = 9687413,

    [Display(Name = "Мультимодальный: морской")]
    Mix_sea = 9687415,

    [Display(Name = "Мультимодальный: морской рефрижераторный")]
    Mix_sea_ref = 32892871,

    [Display(Name = "Трубопровод")]
    Pipe = 2612365,

    [Display(Name = "ВЦ стирол 35 куб")]
    Rail_35 = 543800,

    [Display(Name = "ВЦ ШФЛУ 95 куб")]
    Rail_95 = 543801,

    [Display(Name = "ЖД контейнер балк")]
    Rail_BLK = 543803,

    [Display(Name = "ЖД контейнер универсальный")]
    Rail_cont = 543804,

    [Display(Name = "ЖД крытый вагон")]
    Rail_KV = 543805,

    [Display(Name = "ЖД танк-контейнер")]
    Rail_TK = 543806,

    [Display(Name = "ВЦ прочие грузы")]
    Rail_VC_other = 543802,

    [Display(Name = "Контейнеровоз")]
    Sea = 543808,

    [Display(Name = "Танкер")]
    Tanker = 543809,

    [Display(Name = "Перевалка: сухопутная перевалка СУГ и ЖХ")]
    Transshipment_land = 543813,

    [Display(Name = "Перевалка: Морская перевалка")]
    Transshipment_sea = 543814,

    [Display(Name = "Перевалка: Перевалка танк-контейнеров в АРА")]
    Transshipment_TK = 543815,
}
