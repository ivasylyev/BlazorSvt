using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class LegsGrid : BaseGridPage<LegDto>
{
    [Inject] public IGridDataService<LegDto> LegsDataService { get; set; } = default!;

    protected override async Task<GridDataProviderResult<LegDto>> GetDataAsync(GridDataProviderRequest<LegDto> request, string lang)
    {
        return await LegsDataService.GetDataAsync(request, lang);
    }
}