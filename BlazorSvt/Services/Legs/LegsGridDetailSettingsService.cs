using BlazorSvt.Models.Dto;
using BlazorSvt.Models.Grid;
using BlazorSvt.Resources;
using BlazorSvt.Services.Shared;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Services.Legs;

// ReSharper disable once InconsistentNaming
public class LegsGridDetailSettingsService(IStringLocalizer<Svt> L, ILogger<LegsGridDetailSettingsService> logger) : IGridDetailSettingsService<LegDetailsDto>
{
    public GridDetailSettingsCollection<LegDetailsDto> GetGridDetailSettings(string lang)
    {
        var isRu = lang == "ru";

        var settings = new List<GridDetailSetting<LegDetailsDto>>();
        settings.Add(new GridDetailSetting<LegDetailsDto>()
        {
            Name = nameof(LegDetailsDto.Code),
            Header = L["LegDetailsDto.Code"],
            DisplaySelector = dto => dto.Code,
            VisibleSelector = dto => true
        });


        return new GridDetailSettingsCollection<LegDetailsDto>(settings);
    }
}