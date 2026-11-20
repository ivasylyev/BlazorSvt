using BlazorSvt.Modules.ReportConstructor.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ReportConstructor.List;

public partial class ReportConstructorGrid : BaseGridPage<ReportConstructorDto, ReportConstructorDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.ReportConstructor> EL { get; set; } = default!;

    protected override object DetailKeySelector(ReportConstructorDto request)
        => request.ReportConstructorId;
}
