using BlazorSvt.Resources;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;
using System.Globalization;

namespace BlazorSvt.Components.Common;

public abstract class SvtComponentBase : ComponentBase
{

    [Inject]
    protected IStringLocalizer<Svt> L { get; set; } = default!;

    protected string Lang => CultureInfo.CurrentCulture.TwoLetterISOLanguageName;

}