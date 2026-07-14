using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Тип тарифа (ru). Значения = ItemId из legacy <c>dbo.vw_RateType (dbo.PrimitiveEntityData_2048)</c>.
/// Стабильный справочник: не входит в sync-cascade.
/// </summary>
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

