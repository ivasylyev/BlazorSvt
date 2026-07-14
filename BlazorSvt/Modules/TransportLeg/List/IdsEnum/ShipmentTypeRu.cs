using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.TransportLeg.List.IdsEnum;

/// <summary>
/// Тип отправки (ru). Значения = ItemId из legacy <c>dbo.vw_ShipmentType (dbo.PrimitiveEntityData_2142)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
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

