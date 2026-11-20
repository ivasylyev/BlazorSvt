using BlazorSvt.Modules.Relevance.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Relevance.List;

public partial class RelevanceGrid : BaseGridPage<RelevanceDto, RelevanceDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Relevance> EL { get; set; } = default!;

    protected override object DetailKeySelector(RelevanceDto request)
        => request.RelevanceId;
}
