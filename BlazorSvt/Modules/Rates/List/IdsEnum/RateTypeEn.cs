using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Modules.Rates.List.IdsEnum;

public enum RateTypeEn
{
    [Display(Name = "Tender")] 
    Tender = 543746, //2 Тендерная

    [Display(Name = "Spot")] 
    Spot = 543748, //3 Спотовая

    [Display(Name = "Indicative")] 
    Indicative = 543749, // 4 Индикативная

    [Display(Name = "Agreement")] 
    Agreement = 19434322, // 5 Договорная

    [Display(Name = "Fact")] 
    Fact = 19434323 // 6 Фактическая
}