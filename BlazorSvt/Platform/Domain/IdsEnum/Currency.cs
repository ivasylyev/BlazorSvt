using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

public enum Currency
{
    [Display(Name = "CNY")]
    CNY = 32652,

    [Display(Name = "EUR")]
    EUR = 32628,

    [Display(Name = "RUB")]
    RUB = 32694,

    [Display(Name = "USD")]
    USD = 32623
}
