using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Models.Dto.IdsEnum;

public enum ShipmentTypeEn
{
    [Display(Name = "Tverdaya")]
    TVD = 27584768, //  Твердая	 | 	Tverdaya

    [Display(Name = "Povagonnaya")]
    PVG = 27584769, //  Повагонная	 | 	Povagonnaya

    [Display(Name = "Gruppovaya")]
    GPP = 27584770, //  Групповая	 | 	Gruppovaya

    [Display(Name = "Marshrutnaya")]
    MRT = 27584771, //  Маршрутная	 | 	Marshrutnaya

    [Display(Name = "Zhestkaya nitka")]
    ZHN = 27584772, //  Жесткая нитка	 | 	Zhestkaya nitka
}

