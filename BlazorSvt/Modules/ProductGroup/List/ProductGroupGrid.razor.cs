using BlazorSvt.Modules.ProductGroup.Detail;
using Microsoft.AspNetCore.Components;
using Microsoft.Extensions.Localization;

namespace BlazorSvt.Modules.ProductGroup.List;

public partial class ProductGroupGrid : BaseGridPage<ProductGroupDto, ProductGroupDetailDto>
{
    [Inject]
    protected IStringLocalizer<Resources.ProductGroup> EL { get; set; } = default!;

    protected override object DetailKeySelector(ProductGroupDto request)
        => request.ProductGroupId;
}
