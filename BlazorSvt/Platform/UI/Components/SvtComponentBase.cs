using System.Globalization;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.UI.Components;

public abstract class SvtComponentBase : ComponentBase
{
    [Inject] protected IStringLocalizer<PlatformResources> L { get; set; } = default!;
    protected string Lang => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
}