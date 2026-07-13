using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

public enum RateTypeRu
{
    [Display(Name = "Тендерная")] 
    Tender = 543746, // 2 Тендерная

    [Display(Name = "Спотовая")] 
    Spot = 543748, //3 Спотовая

    [Display(Name = "Индикативная")] 
    Indicative = 543749, // 4 Индикативная

    [Display(Name = "Договорная")] 
    Agreement = 19434322, //5 Договорная

    [Display(Name = "Фактическая")] 
    Fact = 19434323 // 6 Фактическая
}

