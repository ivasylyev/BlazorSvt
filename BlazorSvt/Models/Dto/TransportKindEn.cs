using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Models.Dto;

public enum TransportKindEn
{
    [Display(Name = "Truck transport")]
    Auto = 543760, //	Автомобильный транспорт / Truck transport
    [Display(Name = "Railway transport")]
    Rail = 543761, //	Железнодорожный транспорт / Railway transport
    [Display(Name = "Sea transport")]
    Sea = 543762, //	Водный транспорт / Sea transport
    [Display(Name = "Multimodal transport")]
    Mix = 543763, //	Комбинированная перевозка / Multimodal transport
    [Display(Name = "Pipe")]
    Pipe = 2612363 //	Трубопровод / Pipe
}