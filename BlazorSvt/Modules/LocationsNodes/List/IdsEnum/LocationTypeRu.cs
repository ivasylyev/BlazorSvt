using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

public enum LocationTypeRu
{
    [Display(Name = "Ж/Д станция")]
    C1 = 42854,

    [Display(Name = "Точка врезки трубы")]
    C2 = 42855,

    [Display(Name = "Морской порт")]
    C3 = 42856,

    [Display(Name = "Авто узел")]
    C4 = 42857,

    [Display(Name = "Регион")]
    C5 = 42858,

    [Display(Name = "Аэропорт")]
    C6 = 8017951,

    [Display(Name = "Внутренний порт")]
    C7 = 8017952,

    [Display(Name = "Город")]
    C201 = 9572740,
}
