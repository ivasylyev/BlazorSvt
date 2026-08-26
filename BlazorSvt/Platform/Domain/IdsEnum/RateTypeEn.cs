using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип тарифа (en). Значения = ItemId из legacy <c>dbo.vw_RateType (dbo.PrimitiveEntityData_2048)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
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
