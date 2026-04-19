using BlazorBootstrap;
using BlazorSvt.Models.Dto;
using BlazorSvt.Services.Shared;
using Microsoft.AspNetCore.Components;

namespace BlazorSvt.Components.Pages;

public partial class LegsGrid : BaseGridPage<LegDto, LegDetailDto>
{
    [Inject] 
    public IGridDataService<LegDto, LegDetailDto> LegsDataService { get; set; } = default!;

    protected override object DetailKeySelector(LegDto request)
    {
        return request.LegId;
    }

    protected override async Task<GridDataProviderResult<LegDto>> GetDataAsync(GridDataProviderRequest<LegDto> request, string lang)
    {
        return await LegsDataService.GetDataAsync(request, lang);
    }

    protected override async Task<LegDetailDto> GetDetailDataAsync(LegDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await LegsDataService.GetDetailDataAsync(key);
    }

    private string GetCustomMessage(LegDetailDto detail)
    {
        var message = "Кастомное сообщение от транспортных плеч: ";
        message += detail.ProxyNodeCode is null
            ? "Плечо без промежуточного узла"
            : "Плечо с промежуточным узлом";
        return message;
    }
}