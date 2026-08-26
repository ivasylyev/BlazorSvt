using BlazorBootstrap;

namespace BlazorSvt.Platform.Grid.Services;

public interface IGridQueryFactory<TItem>
{
    GridQuery Create(
        GridDataProviderRequest<TItem> request,
        int? pageNumber = null,
        int? pageSize = null);
}
