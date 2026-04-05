using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Models.Dto;

public enum RateTypeEn
{
    [Display(Name = "Tender")] 
    Tender = 2, //Тендерная

    [Display(Name = "Spot")] 
    Spot = 3, //Спотовая

    [Display(Name = "Indicative")] 
    Indicative = 4, //Индикативная

    [Display(Name = "Agreement")] 
    Agreement = 5, //Договорная

    [Display(Name = "Fact")] 
    Fact = 6 //Фактическая
}