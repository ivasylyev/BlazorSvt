using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Вид транспорта (en). Значения = ItemId из legacy <c>dbo.vw_TransportKind (dbo.PrimitiveEntityData_2008)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TransportKindEn
{
    [Display(Name = "Truck")]
    Auto = 543760, //	Автомобильный транспорт / Truck transport
    [Display(Name = "Railway")]
    Rail = 543761, //	Железнодорожный транспорт / Railway transport
    [Display(Name = "Sea")]
    Sea = 543762, //	Водный транспорт / Sea transport
    [Display(Name = "Multimodal")]
    Mix = 543763, //	Комбинированная перевозка / Multimodal transport
    [Display(Name = "Transshipment: LPG and liquid chemicals land transshipment")]
    Transshipment_land = 543765,
    [Display(Name = "Transshipment: Sea transshipment")]
    Transshipment_sea = 543766,
    [Display(Name = "Transshipment: Tank container transshipment in ARA region")]
    Transshipment_TK = 543767,
    [Display(Name = "Pipe")]
    Pipe = 2612363 //	Трубопровод / Pipe
}
