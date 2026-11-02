USE [mdm];
GO

/*
    Детекция изменившихся продуктов MTR по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (MTRId).

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_1015 — MTR (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1013 — ProductGroup (каскад через ProductGroupId).

    Unit (1011), DangerClass (2018) и StatusElement (1010) в гриде — enum,
    не отслеживаются.

    Каскады ставок на PrimitiveEntityData_1015 живут в PopulateAffectedKeys
    TransportRate, AverageRateLevel3 и ParityRates. Этот job их не меняет.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.MTR_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- MTR (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи продуктов.
    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1015 d WITH (NOLOCK) -- MTR (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- ProductGroup: затронутые продукты через ProductGroupId.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.MTRId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.MTR_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.MTRId);
        RETURN;
    END

    ;THROW 50000, 'Unknown MTR sync source.', 1;
END
GO
