USE [mdm];
GO

/*
    Детекция изменившихся ParityRates по одному источнику за вызов.

    Источники:
      dbo.PrimitiveEntityData_2109 — ParityRates (основная)
      dbo.PrimitiveEntityData_1014 — LocationsNodes (NodeFrom/To/ProxyNode1/2)
      dbo.PrimitiveEntityData_1013 — ProductGroup
      dbo.PrimitiveEntityData_1015 — MTR/Product

    Relevance (2108), Currency (2016), TransportType (2023) стабильны — не отслеживаются.
*/

CREATE OR ALTER PROCEDURE v2.ParityRates_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2109' -- ParityRates (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2109 d WITH (NOLOCK) -- ParityRates (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.ParityRatesId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.ProxyNode1Id = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.ProxyNode2Id = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ParityRatesId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.ParityRates_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ParityRatesId);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ParityRatesId
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.ParityRates_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ParityRatesId);
        RETURN;
    END

    ;THROW 50000, 'Unknown ParityRates sync source.', 1;
END
GO