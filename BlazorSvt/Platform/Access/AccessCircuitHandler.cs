using Microsoft.AspNetCore.Components.Server.Circuits;

namespace BlazorSvt.Platform.Access;

/// <summary>Сверка доступа при открытии Blazor-circuit (один раз на сессию).</summary>
public sealed class AccessCircuitHandler(
    UserAccessSynchronizer synchronizer,
    ILogger<AccessCircuitHandler> logger) : CircuitHandler
{
    public override async Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        try
        {
            await synchronizer.SynchronizeAsync(cancellationToken);
        }
        catch (OperationCanceledException)
        {
            throw;
        }
        catch (Exception ex)
        {
            logger.LogError(ex, "User access synchronization failed while opening circuit {CircuitId}", circuit.Id);
            throw;
        }
    }
}
