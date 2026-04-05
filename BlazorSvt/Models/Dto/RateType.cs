using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Models.Dto;

public enum RateType
{
    [Display(Name = "Тендерная")] 
    Tender = 2, //Тендерная

    [Display(Name = "Спотовая")] 
    Spot = 3, //Спотовая

    [Display(Name = "Индикативная")] 
    Indicative = 4, //Индикативная

    [Display(Name = "Договорная")] 
    Agreement = 5, //Договорная

    [Display(Name = "Фактическая")] 
    Fact = 6 //Фактическая
}