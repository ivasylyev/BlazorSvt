using BlazorBootstrap;
using BlazorSvt.Modules.AverageRateLevel3.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.AverageRateLevel3.List;

public partial class AverageRateLevel3Grid : BaseGridPage<AverageRateLevel3Dto, AverageRateLevel3DetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.AverageRateLevel3> EL { get; set; } = default!;

    [Inject]
    public IGridDataService<AverageRateLevel3Dto, AverageRateLevel3DetailDto> AverageRateLevel3DataService { get; set; } = default!;

    protected override object DetailKeySelector(AverageRateLevel3Dto request)
        => request.AverageRateLevel3Id;

    protected override async Task<GridDataProviderResult<AverageRateLevel3Dto>> GetDataAsync(
        GridDataProviderRequest<AverageRateLevel3Dto> request,
        string lang) =>
        await AverageRateLevel3DataService.GetDataAsync(request, lang);

    protected override async Task<AverageRateLevel3DetailDto> GetDetailDataAsync(AverageRateLevel3Dto request, string lang)
    {
        var key = DetailKeySelector(request);
        return await AverageRateLevel3DataService.GetDetailDataAsync(key);
    }
}
