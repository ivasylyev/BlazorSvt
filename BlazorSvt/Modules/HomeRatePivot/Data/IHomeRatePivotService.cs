namespace BlazorSvt.Modules.HomeRatePivot.Data;

public interface IHomeRatePivotService
{
    /// <param name="useRussianNames">true → NameRu, false → NameEn (с fallback).</param>
    Task<HomeRatePivotTable> GetTableAsync(bool useRussianNames, CancellationToken cancellationToken = default);
}
