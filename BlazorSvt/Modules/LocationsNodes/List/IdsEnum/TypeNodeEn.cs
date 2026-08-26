using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

/// <summary>
/// Тип узла (en). Значения = ItemId из legacy <c>dbo.vw_TypeNode (dbo.PrimitiveEntityData_2132)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TypeNodeEn
{
    [Display(Name = "Country")]
    Country = 24730972,

    [Display(Name = "Federal_district")]
    Federal_district = 24730973,

    [Display(Name = "Region")]
    Region = 24730974,

    [Display(Name = "Auto")]
    Auto = 24730975,

    [Display(Name = "Border")]
    Border = 24730976,

    [Display(Name = "Rail")]
    Rail = 24730977,

    [Display(Name = "Port")]
    Port = 24730978,

    [Display(Name = "Pipe")]
    Pipe = 24730979,

    [Display(Name = "Warehouse")]
    Warehouse = 24730980,

    [Display(Name = "Airport")]
    Airport = 25073271,
}
