using BlazorBootstrap;
using BlazorSvt.Models.Grid;

namespace BlazorSvt.Services.Shared;

public interface IGridQueryFactory<TItem>
{
    GridQuery Create(
        GridDataProviderRequest<TItem> request,
        string lang,
        int? pageNumber = null,
        int? pageSize = null);
}
