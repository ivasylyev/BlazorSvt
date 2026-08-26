using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Вид транспорта (ru). Значения = ItemId из legacy <c>dbo.vw_TransportKind (dbo.PrimitiveEntityData_2008)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TransportKindRu
{
    [Display(Name = "Автомобильный")]
    Auto = 543760, //	Автомобильный транспорт / Truck transport
    [Display(Name = "Железнодорожный")]
    Rail = 543761, //	Железнодорожный транспорт / Railway transport
    //[Display(Name = "Водный транспорт")]
    //Sea = 543762, //	Водный транспорт / Sea transport
    [Display(Name = "Мультимодальный")]
    Mix = 543763, //	Комбинированная перевозка / Multimodal transport
    //[Display(Name = "Трубопровод")]
    //Pipe = 2612363 //	Трубопровод / Pipe
}