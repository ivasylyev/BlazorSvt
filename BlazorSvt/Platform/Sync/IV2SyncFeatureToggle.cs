namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Оперативный kill switch синхронизации legacy → snapshot.
/// Источник — <c>dbo.vw_FeatureToggle</c>, код <c>V2SyncEnabled</c>.
/// </summary>
public interface IV2SyncFeatureToggle
{
    /// <summary>
    /// <c>true</c>, только если строка тогла есть и <c>ToggleState = 1</c>.
    /// Нет строки или ошибка чтения → <c>false</c> (fail-closed).
    /// </summary>
    Task<bool> IsEnabledAsync(CancellationToken cancellationToken = default);
}
