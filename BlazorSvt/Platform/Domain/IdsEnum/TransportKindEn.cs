using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

public enum TransportKindEn
{
    [Display(Name = "Truck")]
    Auto = 543760, //	Автомобильный транспорт / Truck transport
    [Display(Name = "Railway")]
    Rail = 543761, //	Железнодорожный транспорт / Railway transport
  //  [Display(Name = "Sea")]
 //   Sea = 543762, //	Водный транспорт / Sea transport
    [Display(Name = "Multimodal")]
    Mix = 543763, //	Комбинированная перевозка / Multimodal transport
  //  [Display(Name = "Pipe")]
  //  Pipe = 2612363 //	Трубопровод / Pipe
}
