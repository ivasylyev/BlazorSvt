using System.Globalization;
using BlazorSvt.Host.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Platform.UI.Components;

public abstract class SvtComponentBase : ComponentBase
{
    [Inject] protected IStringLocalizer<Svt> L { get; set; } = default!;
    protected string Lang => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;
}