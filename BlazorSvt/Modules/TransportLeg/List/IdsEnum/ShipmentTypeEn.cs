using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.TransportLeg.List.IdsEnum;

/// <summary>
/// Тип отправки (en). Значения = ItemId из legacy <c>dbo.vw_ShipmentType (dbo.PrimitiveEntityData_2142)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
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


