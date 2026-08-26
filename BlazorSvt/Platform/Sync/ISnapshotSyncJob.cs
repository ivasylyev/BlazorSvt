namespace BlazorSvt.Platform.Sync;

/// <summary>
/// Декларативное описание синхронизации одного справочника legacy -> v2 snapshot.
/// Механику (детекция по rowversion, партиционно-безопасный upsert, reconciliation)
/// выполняет <see cref="SnapshotSyncExecutor"/> одинаково для всех справочников
/// через generic-процедуры v2.Sync_*. Реализация модуля лишь описывает snapshot,
/// проекцию, источники изменений и имя per-entity процедуры детекции/каскада.
/// </summary>
public interface ISnapshotSyncJob
{
    /// <summary>Системное имя справочника, напр. "TransportLeg". Ключ в v2.SyncState.</summary>
    string Entity { get; }

    /// <summary>Snapshot-таблица read-модели, напр. "v2.TransportLeg_Snapshot".</summary>
    string SnapshotTable { get; }

    /// <summary>Бизнес-ключ snapshot, напр. "TransportLegId".</summary>
    string EntityKeyColumn { get; }

    /// <summary>Проекция-источник (единый SELECT для полной заливки и инкремента),
    /// напр. "v2.vw_TransportLeg_SnapshotSource".</summary>
    string SourceProjectionView { get; }

    /// <summary>
    /// Процедура детекции затронутых ключей по одному источнику за вызов.
    /// Сигнатура: <c>(@Source NVARCHAR, @Lo BINARY(8), @Hi BINARY(8))</c>;
    /// наполняет существующую temp-таблицу <c>#AffectedKeys(EntityKey BIGINT)</c>.
    /// Напр. "v2.TransportLeg_PopulateAffectedKeys".
    /// </summary>
    string PopulateAffectedKeysProc { get; }

    /// <summary>Источники изменений: основная таблица + справочные (каскад).</summary>
    IReadOnlyList<SnapshotSyncSource> Sources { get; }
}

/// <summary>
/// Один источник изменений для справочника.
/// </summary>
/// <param name="Name">
/// Уникальное имя источника в рамках справочника — ключ в v2.SyncState.SourceName
/// и значение параметра <c>@Source</c> процедуры детекции. Используем полное имя
/// базовой таблицы, напр. "dbo.PrimitiveEntityData_2007".
/// </param>
public sealed record SnapshotSyncSource(string Name);
