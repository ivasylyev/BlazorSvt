namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Базовая реализация <see cref="ISnapshotSyncJob"/> с именами объектов по конвенции skill:
/// Entity → v2.{Entity}_Snapshot / {Entity}Id / vw_{Entity}_SnapshotSource / {Entity}_PopulateAffectedKeys.
/// Модуль задаёт только системное имя и список источников изменений.
/// </summary>
public abstract class SnapshotSyncJob : ISnapshotSyncJob
{
    protected SnapshotSyncJob(string entity, params string[] sources)
    {
        ArgumentException.ThrowIfNullOrWhiteSpace(entity);
        ArgumentNullException.ThrowIfNull(sources);
        if (sources.Length == 0)
            throw new ArgumentException("At least one sync source is required.", nameof(sources));

        Entity = entity;
        Sources = sources.Select(static s => new SnapshotSyncSource(s)).ToArray();
    }

    public string Entity { get; }

    public string SnapshotTable => $"v2.{Entity}_Snapshot";

    public string EntityKeyColumn => $"{Entity}Id";

    public string SourceProjectionView => $"v2.vw_{Entity}_SnapshotSource";

    public string PopulateAffectedKeysProc => $"v2.{Entity}_PopulateAffectedKeys";

    public IReadOnlyList<SnapshotSyncSource> Sources { get; }
}
