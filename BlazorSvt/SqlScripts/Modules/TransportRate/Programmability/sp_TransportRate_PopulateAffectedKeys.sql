USE [mdm];
GO

/*
    Детекция изменившихся рейтов TransportRate по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (TransportRateId) затронутых рейтов.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_2012 — TransportRate (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад: NodeFrom/To/Proxy).
      dbo.PrimitiveEntityData_1013 — ProductGroup   (каскад: ProductGroupId).
      dbo.PrimitiveEntityData_1015 — MTR/Product     (каскад: ProductId).

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE проекции. Источник 1015 обязателен — иначе
    вход/выход строки из snapshot отследит только суточный reconcile.

    RateType (2048), TransportKind (2008), TransportType (2023) и Currency (2016)
    стабильны — не отслеживаются.

    Каскад — эквиджойн по одной FK-колонке snapshot (для 1014 — UNION по трём):
    позволяет использовать NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.TransportRate_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- TransportRate (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи рейтов.
    IF @Source = N'dbo.PrimitiveEntityData_2012' -- TransportRate (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2012 d WITH (NOLOCK) -- TransportRate (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- LocationsNodes: затронутые рейты через NodeFromId / NodeToId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportRateId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportRateId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportRateId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.ProxyNodeId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- ProductGroup: затронутые рейты через ProductGroupId.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.TransportRateId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.TransportRate_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.TransportRateId);
        RETURN;
    END

    -- MTR/Product: затронутые рейты через ProductId (важно для WHERE-членства).
    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.TransportRateId
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.TransportRate_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.TransportRateId);
        RETURN;
    END

    ;THROW 50000, 'Unknown TransportRate sync source.', 1;
END
GO
