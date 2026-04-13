using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Models.Dto.IdsEnum;

public enum ShipmentTypeRu
{
    [Display(Name = "Твердая")]
    TVD = 27584768, //  Твердая	 | 	Tverdaya

    [Display(Name = "Повагонная")]
    PVG = 27584769, //  Повагонная	 | 	Povagonnaya

    [Display(Name = "Групповая")]
    GPP = 27584770, //  Групповая	 | 	Gruppovaya

    [Display(Name = "Маршрутная")]
    MRT = 27584771, //  Маршрутная	 | 	Marshrutnaya

    [Display(Name = "Жесткая нитка")]
    ZHN = 27584772, //  Жесткая нитка	 | 	Zhestkaya nitka
}
