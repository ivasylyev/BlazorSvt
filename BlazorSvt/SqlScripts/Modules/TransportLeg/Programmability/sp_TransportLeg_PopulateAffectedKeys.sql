USE [mdm];
GO

/*
    Детекция изменившихся плеч TransportLeg по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (TransportLegId) затронутых плеч.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_2007 — TransportLeg (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1008 — Region  (каскад через RegionFrom/To/Proxy).
      dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад через NodeFrom/To/Proxy).

    TransportKind (2008) стабилен — не отслеживается.
    ShipmentTypeCodeT — сырой multi-code атрибут без FK; каскад по нему не нужен.

    Каскад — UNION эквиджойнов по одной FK-колонке snapshot: позволяет
    использовать NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.TransportLeg_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- TransportLeg (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи плеч.
    IF @Source = N'dbo.PrimitiveEntityData_2007' -- TransportLeg (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2007 d WITH (NOLOCK) -- TransportLeg (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- Region: затронутые плечи через RegionFromId / RegionToId / ProxyRegionId.
    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportLegId AS EntityKey
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.RegionFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.RegionToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.ProxyRegionId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- LocationsNodes: затронутые плечи через NodeFromId / NodeToId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportLegId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.ProxyNodeId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    ;THROW 50000, 'Unknown TransportLeg sync source.', 1;
END
GO
