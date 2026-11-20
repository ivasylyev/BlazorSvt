USE [mdm];
GO

/*
    Детекция изменившихся строк транспортного маршрута.

    dbo.PrimitiveEntityData_2010 — TransportRoute (основная).
    dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад: StartNodeId / EndNodeId / ProxyNodeId).
    dbo.PrimitiveEntityData_1008 — Region (каскад: RegionFromId / RegionToId).

    TransportKind (2008), TransportType_level_3 (2023) и TransportLeg (2007)
    не отслеживаются.
*/
CREATE OR ALTER PROCEDURE v2.TransportRoute_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2010' -- TransportRoute (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2010 d WITH (NOLOCK) -- TransportRoute (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- LocationsNodes: затронутые строки через StartNodeId / EndNodeId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportRouteId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 n WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRoute_Snapshot s ON s.StartNodeId = n.PrimitiveEntityItemId
            WHERE n.RowVer > @Lo AND n.RowVer <= @Hi
            UNION
            SELECT s.TransportRouteId
            FROM dbo.PrimitiveEntityData_1014 n WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRoute_Snapshot s ON s.EndNodeId = n.PrimitiveEntityItemId
            WHERE n.RowVer > @Lo AND n.RowVer <= @Hi
            UNION
            SELECT s.TransportRouteId
            FROM dbo.PrimitiveEntityData_1014 n WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRoute_Snapshot s ON s.ProxyNodeId = n.PrimitiveEntityItemId
            WHERE n.RowVer > @Lo AND n.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- Region: затронутые строки через RegionFromId / RegionToId.
    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportRouteId AS EntityKey
            FROM dbo.PrimitiveEntityData_1008 g WITH (NOLOCK) -- Region
            JOIN v2.TransportRoute_Snapshot s ON s.RegionFromId = g.PrimitiveEntityItemId
            WHERE g.RowVer > @Lo AND g.RowVer <= @Hi
            UNION
            SELECT s.TransportRouteId
            FROM dbo.PrimitiveEntityData_1008 g WITH (NOLOCK) -- Region
            JOIN v2.TransportRoute_Snapshot s ON s.RegionToId = g.PrimitiveEntityItemId
            WHERE g.RowVer > @Lo AND g.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown TransportRoute sync source.', 1;
    END
END
GO
