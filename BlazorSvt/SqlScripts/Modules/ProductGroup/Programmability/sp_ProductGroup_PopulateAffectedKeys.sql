USE [mdm];
GO

/*
    Детекция изменившихся транспортных групп ProductGroup по одному источнику
    за вызов. По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно)
    добавляет в #AffectedKeys бизнес-ключи (ProductGroupId).

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_1013 — ProductGroup (основная): сами изменившиеся Id.

    RateSyncClass (2127) в гриде — enum по RateSyncClassId, не отслеживается.

    Каскады ставок на эту же таблицу (TransportRate, AverageRateLevel3,
    ParityRates) живут в их собственных PopulateAffectedKeys.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.ProductGroup_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- ProductGroup (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи групп.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1013 d WITH (NOLOCK) -- ProductGroup (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    ;THROW 50000, 'Unknown ProductGroup sync source.', 1;
END
GO
