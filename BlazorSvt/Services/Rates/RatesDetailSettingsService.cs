using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Rates;

public class RatesDetailSettingsService(IStringLocalizer<Svt> L, ILogger<RatesDetailSettingsService> logger) : IDetailSettingsService<RateDetailDto>
{
    public DetailSettingsCollection<RateDetailDto> GetGridDetailSettings(string lang)
    {
        throw new NotImplementedException();
    }
}