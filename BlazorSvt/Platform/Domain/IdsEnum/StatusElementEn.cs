using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Record status (en). Values = Id from dbo.vw_StatusElement.
/// Short dictionary: not part of the sync cascade.
/// </summary>
public enum StatusElementEn
{
    [Display(Name = "Active")]
    Active = 42859,

    [Display(Name = "Archived")]
    Archived = 42860
}
