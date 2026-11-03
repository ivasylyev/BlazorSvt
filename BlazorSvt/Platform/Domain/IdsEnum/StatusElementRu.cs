using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Статус записи (ru). Значения = Id из dbo.vw_StatusElement.
/// Короткий словарь: в каскад синхронизации не входит.
/// </summary>
public enum StatusElementRu
{
    [Display(Name = "Активная")]
    Active = 42859,

    [Display(Name = "Архивная")]
    Archived = 42860
}
