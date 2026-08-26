using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

/// <summary>
/// Тип места / TypePlace (en). Значения = ItemId из legacy <c>dbo.vw_TypePlace (dbo.PrimitiveEntityData_1007)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum LocationTypeEn
{
    [Display(Name = "Railway station")]
    Railway_station = 42854,

    [Display(Name = "Tie-in point")]
    Tie_in_point = 42855,

    [Display(Name = "Sea port")]
    Sea_port = 42856,

    [Display(Name = "Truck hub")]
    Truck_hub = 42857,

    [Display(Name = "Region")]
    Region = 42858,

    [Display(Name = "Airport")]
    Airport = 8017951,

    [Display(Name = "Internal port")]
    Internal_port = 8017952,

    [Display(Name = "City")]
    City = 9572740,
}
