using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class LegsGrid : BaseGridPage<LegDto, LegDetailsDto>
{
    [Inject] 
    public IGridDataService<LegDto, LegDetailsDto> LegsDataService { get; set; } = default!;
    protected override object DetailKeySelector(LegDto request)
        => request.LegId;
    protected override async Task<GridDataProviderResult<LegDto>> GetDataAsync(GridDataProviderRequest<LegDto> request, string lang) 
        => await LegsDataService.GetDataAsync(request, lang);

    protected override async Task<LegDetailsDto> GetDetailDataAsync(LegDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await LegsDataService.GetDetailDataAsync(key, lang) ?? new LegDetailsDto();
    }
}