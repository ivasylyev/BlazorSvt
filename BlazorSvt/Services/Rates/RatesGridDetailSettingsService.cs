using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

public class RatesGridDetailSettingsService(IStringLocalizer<Svt> L, ILogger<RatesGridDetailSettingsService> logger) : IGridDetailSettingsService<RateDetailsDto>
{
    public GridDetailSettingsCollection<RateDetailsDto> GetGridDetailSettings(string lang)
    {
        throw new NotImplementedException();
    }
}