using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Legs;

// ReSharper disable once InconsistentNaming
public class LegsDetailSettingsService(IStringLocalizer<Svt> L, ILogger<LegsDetailSettingsService> logger) : IDetailSettingsService<LegDetailDto>
{
    public DetailSettingsCollection<LegDetailDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var settings = new List<DetailSetting<LegDetailDto>>();
        settings.Add(new DetailSetting<LegDetailDto>()
        {
            Name = nameof(LegDetailDto.Code),
            Header = L["LegDetailDto.Code"],
            DisplaySelector = dto => dto.Code,
            VisibleSelector = dto => true
        });


        return new DetailSettingsCollection<LegDetailDto>(settings);
    }
}