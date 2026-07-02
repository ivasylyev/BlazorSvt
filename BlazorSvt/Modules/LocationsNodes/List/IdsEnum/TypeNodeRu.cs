using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

public enum TypeNodeRu
{
    [Display(Name = "Страна")]
    N10 = 24730972,

    [Display(Name = "Округ")]
    N20 = 24730973,

    [Display(Name = "Регион")]
    N30 = 24730974,

    [Display(Name = "Авто")]
    N40 = 24730975,

    [Display(Name = "Транзит")]
    N43 = 24730976,

    [Display(Name = "ЖД")]
    N44 = 24730977,

    [Display(Name = "Море")]
    N45 = 24730978,

    [Display(Name = "Точка врезки трубы")]
    N46 = 24730979,

    [Display(Name = "Склад")]
    N47 = 24730980,

    [Display(Name = "Аэропорт")]
    N48 = 25073271,
}
