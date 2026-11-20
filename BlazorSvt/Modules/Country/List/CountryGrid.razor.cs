using BlazorSvt.Modules.Country.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.Country.List;

public partial class CountryGrid : BaseGridPage<CountryDto, CountryDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.Country> EL { get; set; } = default!;

    protected override object DetailKeySelector(CountryDto request)
        => request.CountryId;
}
