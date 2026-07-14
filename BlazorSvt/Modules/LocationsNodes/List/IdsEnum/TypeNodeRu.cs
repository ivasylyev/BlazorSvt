using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

/// <summary>
/// Тип узла (ru). Значения = ItemId из legacy <c>dbo.vw_TypeNode (dbo.PrimitiveEntityData_2132)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum TypeNodeRu
{
    [Display(Name = "Страна")]
    Country = 24730972,

    [Display(Name = "Округ")]
    Federal_district = 24730973,

    [Display(Name = "Регион")]
    Region = 24730974,

    [Display(Name = "Авто")]
    Auto = 24730975,

    [Display(Name = "Погранпереход")]
    Border = 24730976,

    [Display(Name = "ЖД")]
    Rail = 24730977,

    [Display(Name = "Порт")]
    Port = 24730978,

    [Display(Name = "Точка врезки трубы")]
    Pipe = 24730979,

    [Display(Name = "Склад")]
    Warehouse = 24730980,

    [Display(Name = "Аэропорт")]
    Airport = 25073271,
}
