USE [mdm];
GO

/*
    Детекция изменившихся строк региона.

    dbo.PrimitiveEntityData_1008 — Region (основная).
    dbo.PrimitiveEntityData_1009 — Country (каскад: CountryId, в том числе Market).

    StatusElement (1010) и Federal_Districts (2120) не отслеживаются.
*/
CREATE OR ALTER PROCEDURE v2.Region_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1008 d WITH (NOLOCK) -- Region (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- Country: затронутые строки через CountryId.
    IF @Source = N'dbo.PrimitiveEntityData_1009' -- Country
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.RegionId
        FROM dbo.PrimitiveEntityData_1009 c WITH (NOLOCK) -- Country
        JOIN v2.Region_Snapshot s ON s.CountryId = c.PrimitiveEntityItemId
        WHERE c.RowVer > @Lo AND c.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.RegionId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown Region sync source.', 1;
    END
END
GO
