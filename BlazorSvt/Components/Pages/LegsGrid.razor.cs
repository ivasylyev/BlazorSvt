using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Legs;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class LegsGrid : BaseGridPage<LegDto>
{
    [Inject]
    public ILegsDataService LegsDataService { get; set; } = default!;

    protected override async Task<GridDataProviderResult<LegDto>> GetDataAsync(GridDataProviderRequest<LegDto> request, string lang)
    {
         return await LegsDataService.GetLegsAsync(request, lang);
    }
}