using BlazorBootstrap;
using BlazorSvt.Modules.LocationsNodes.Detail;
using BlazorSvt.Platform.Grid.Services;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.LocationsNodes.List;

public partial class LocationsNodesGrid : BaseGridPage<LocationsNodesDto, LocationsNodesDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.LocationsNodes> EL { get; set; } = default!;

    [Inject]
    public IGridDataService<LocationsNodesDto, LocationsNodesDetailDto> LocationsNodesDataService { get; set; } = default!;

    protected override object DetailKeySelector(LocationsNodesDto request) => request.LocationsNodesId;

    protected override async Task<GridDataProviderResult<LocationsNodesDto>> GetDataAsync(
        GridDataProviderRequest<LocationsNodesDto> request,
        string lang) =>
        await LocationsNodesDataService.GetDataAsync(request, lang);

    protected override async Task<LocationsNodesDetailDto> GetDetailDataAsync(LocationsNodesDto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await LocationsNodesDataService.GetDetailDataAsync(key);
    }
}
