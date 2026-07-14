USE [mdm];
GO

/*
    Детекция изменившихся узлов LocationsNodes по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (LocationsNodesId) затронутых узлов.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_1014 — LocationsNodes (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1008 — Region    (каскад через RegionId).
      dbo.PrimitiveEntityData_1009 — Country   (каскад через CountryId).

    TypePlace (1007) и TypeNode (2132) стабильны — не отслеживаются.

    Каскад — эквиджойн по одной FK-колонке snapshot: позволяет использовать
    NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.LocationsNodes_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- LocationsNodes (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи узлов.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1014 d WITH (NOLOCK) -- LocationsNodes (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- Region: затронутые узлы через RegionId.
    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.LocationsNodesId
        FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
        JOIN v2.LocationsNodes_Snapshot s ON s.RegionId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.LocationsNodesId);
        RETURN;
    END

    -- Country: затронутые узлы через CountryId.
    IF @Source = N'dbo.PrimitiveEntityData_1009' -- Country
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.LocationsNodesId
        FROM dbo.PrimitiveEntityData_1009 r WITH (NOLOCK) -- Country
        JOIN v2.LocationsNodes_Snapshot s ON s.CountryId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.LocationsNodesId);
        RETURN;
    END

    ;THROW 50000, 'Unknown LocationsNodes sync source.', 1;
END
GO
