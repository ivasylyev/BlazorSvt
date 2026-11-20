USE [mdm];
GO

/*
    Детекция изменившихся строк конструктора отчётов.

    dbo.PrimitiveEntityData_2130 — ReportConstructor (основная).
    dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад: NodeFromId / NodeToId).
    dbo.PrimitiveEntityData_1013 — ProductGroup (каскад: ProductGroupId).
    dbo.PrimitiveEntityData_1015 — MTR (Product) (каскад: ProductId).

    ReportDictionaryTypes (2129), TransportKind (2008) и TransportType_level_3 (2023)
    не отслеживаются.
*/
CREATE OR ALTER PROCEDURE v2.ReportConstructor_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2130' -- ReportConstructor (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2130 d WITH (NOLOCK) -- ReportConstructor (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- LocationsNodes: затронутые строки через NodeFromId / NodeToId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.ReportConstructorId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ReportConstructor_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ReportConstructorId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ReportConstructor_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- ProductGroup: затронутые строки через ProductGroupId.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ReportConstructorId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.ReportConstructor_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ReportConstructorId);
        RETURN;
    END

    -- MTR/Product: затронутые строки через ProductId.
    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ReportConstructorId
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.ReportConstructor_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ReportConstructorId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown ReportConstructor sync source.', 1;
    END
END
GO
