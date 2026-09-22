using Blazored.LocalStorage;

namespace BlazorSvt.Platform.Grid.Services;

public class GridPageSizeService(
    ILocalStorageService localStorage,
    ILogger<GridPageSizeService> logger)
    : IGridPageSizeService
{
    public async Task<int> GetAsync()
    {
        try
        {
            var stored = await localStorage.GetItemAsync<int?>(GridPageSizes.StorageKey);
            return GridPageSizes.Normalize(stored);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not load grid page size from LocalStorage ({StorageKey})", GridPageSizes.StorageKey);
            return GridPageSizes.Default;
        }
    }

    public async Task SaveAsync(int pageSize)
    {
        var normalized = GridPageSizes.Normalize(pageSize);
        try
        {
            await localStorage.SetItemAsync(GridPageSizes.StorageKey, normalized);
        }
        catch (Exception e)
        {
            logger.LogError(e, "Could not save grid page size to LocalStorage ({StorageKey})", GridPageSizes.StorageKey);
            throw;
        }
    }
}
