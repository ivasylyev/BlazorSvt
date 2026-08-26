using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

/// <summary>
/// Тип места / TypePlace (ru). Значения = ItemId из legacy <c>dbo.vw_TypePlace (dbo.PrimitiveEntityData_1007)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
public enum LocationTypeRu
{
    [Display(Name = "ЖД станция")]
    Railway_station = 42854,

    [Display(Name = "Точка врезки трубы")]
    Tie_in_point = 42855,

    [Display(Name = "Морской порт")]
    Sea_port = 42856,

    [Display(Name = "Авто узел")]
    Truck_hub = 42857,

    [Display(Name = "Регион")]
    Region = 42858,

    [Display(Name = "Аэропорт")]
    Airport = 8017951,

    [Display(Name = "Внутренний порт")]
    Internal_port = 8017952,

    [Display(Name = "Город")]
    City = 9572740,
}
