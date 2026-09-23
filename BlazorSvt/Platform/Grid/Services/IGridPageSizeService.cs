namespace BlazorSvt.Platform.Grid.Services;

public interface IGridPageSizeService
{
    Task<int> GetAsync();

    Task SaveAsync(int pageSize);
}
