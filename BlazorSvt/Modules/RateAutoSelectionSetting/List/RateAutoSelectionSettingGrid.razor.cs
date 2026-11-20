using BlazorSvt.Modules.RateAutoSelectionSetting.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.RateAutoSelectionSetting.List;

public partial class RateAutoSelectionSettingGrid : BaseGridPage<RateAutoSelectionSettingDto, RateAutoSelectionSettingDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.RateAutoSelectionSetting> EL { get; set; } = default!;

    protected override object DetailKeySelector(RateAutoSelectionSettingDto request)
        => request.RateAutoSelectionSettingId;
}
