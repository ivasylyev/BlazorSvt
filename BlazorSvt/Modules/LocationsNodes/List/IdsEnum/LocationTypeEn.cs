using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.LocationsNodes.List.IdsEnum;

public enum LocationTypeEn
{
    [Display(Name = "Railway station")]
    C1 = 42854,

    [Display(Name = "Tie-in point")]
    C2 = 42855,

    [Display(Name = "Sea port")]
    C3 = 42856,

    [Display(Name = "Truck hub")]
    C4 = 42857,

    [Display(Name = "Region")]
    C5 = 42858,

    [Display(Name = "Airport")]
    C6 = 8017951,

    [Display(Name = "Internal port")]
    C7 = 8017952,

    [Display(Name = "City")]
    C201 = 9572740,
}
