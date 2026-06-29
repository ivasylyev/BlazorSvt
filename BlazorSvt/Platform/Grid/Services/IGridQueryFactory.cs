using BlazorBootstrap;
using BlazorSvt.Platform.Grid.Models;

namespace BlazorSvt.Platform.Grid.Services;

public interface IGridQueryFactory<TItem>
{
    GridQuery Create(
        GridDataProviderRequest<TItem> request,
        string lang,
        int? pageNumber = null,
        int? pageSize = null);
}
