using BlazorSvt.Modules.Contractor.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Contractor.List;

public partial class ContractorGrid : BaseGridPage<ContractorDto, ContractorDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Contractor> EL { get; set; } = default!;

    protected override object DetailKeySelector(ContractorDto request)
        => request.ContractorId;
}
