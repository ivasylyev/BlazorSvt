using BlazorSvt.Import.Models;
using BlazorSvt.Platform.Resources;
using FluentValidation;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Import.Validators;

public class LegStgWithoutProxyDtoValidator : AbstractValidator<LegStgWithoutProxyDto>
{
    public LegStgWithoutProxyDtoValidator(IStringLocalizer<PlatformResources> localizer)
    {
        RuleFor(x => x.Code)
            .NotEmpty()
            .WithMessage(localizer["Validation.Required"]);

        RuleFor(x => x.NodeFrom)
            .NotEmpty()
            .WithMessage(localizer["Validation.Required"]);

        RuleFor(x => x.NodeTo)
            .NotEmpty()
            .WithMessage(localizer["Validation.Required"]);

        RuleFor(x => x.Distance)
            .GreaterThan(0)
            .When(x => x.Distance.HasValue)
            .WithMessage(localizer["Validation.GreaterThanZero"]);

        // TODO: add the remaining business checks here.
    }
}
