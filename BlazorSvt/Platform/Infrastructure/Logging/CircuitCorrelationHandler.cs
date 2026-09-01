using System.Collections.Concurrent;
using BlazorSvt.Platform.Access;
using Microsoft.AspNetCore.Components.Server.Circuits;
using Serilog.Context;

namespace BlazorSvt.Platform.Infrastructure.Logging;

/// <summary>
/// Корреляция логов для Blazor Server circuit: ShortCorrelationId + CircuitId
/// на каждое inbound-действие, плюс события open/close/connection-down.
/// </summary>
public sealed class CircuitCorrelationHandler(
    ILogger<CircuitCorrelationHandler> logger,
    ICurrentUser currentUser) : CircuitHandler
{
    private readonly ConcurrentDictionary<string, string> correlationIds = new();

    public override Task OnCircuitOpenedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        var corrId = CreateCorrelationId();
        correlationIds[circuit.Id] = corrId;

        logger.LogInformation(
            "Blazor circuit opened {CircuitId} {ShortCorrelationId}",
            circuit.Id,
            corrId);

        return Task.CompletedTask;
    }

    public override Task OnCircuitClosedAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        correlationIds.TryRemove(circuit.Id, out var corrId);

        logger.LogInformation(
            "Blazor circuit closed {CircuitId} {ShortCorrelationId}",
            circuit.Id,
            corrId);

        return Task.CompletedTask;
    }

    public override Task OnConnectionDownAsync(Circuit circuit, CancellationToken cancellationToken)
    {
        correlationIds.TryGetValue(circuit.Id, out var corrId);

        logger.LogWarning(
            "Blazor circuit connection down {CircuitId} {ShortCorrelationId}",
            circuit.Id,
            corrId);

        return Task.CompletedTask;
    }

    public override Func<CircuitInboundActivityContext, Task> CreateInboundActivityHandler(
        Func<CircuitInboundActivityContext, Task> next)
    {
        return async context =>
        {
            var circuitId = context.Circuit.Id;
            var corrId = correlationIds.GetOrAdd(circuitId, _ => CreateCorrelationId());

            using (LogContext.PushProperty("CircuitId", circuitId))
            using (LogContext.PushProperty("ShortCorrelationId", corrId))
            using (LogContext.PushProperty("UserLogin", currentUser.Login))
            {
                await next(context);
            }
        };
    }

    private static string CreateCorrelationId() => Guid.NewGuid().ToString("N")[..8];
}
