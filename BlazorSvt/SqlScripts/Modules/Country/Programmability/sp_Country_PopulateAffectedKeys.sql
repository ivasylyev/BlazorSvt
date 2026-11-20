USE [mdm];
GO

/*
    Детекция изменившихся строк страны.

    dbo.PrimitiveEntityData_1009 — Country (основная).
    StatusElement (1010) не отслеживается.
*/
CREATE OR ALTER PROCEDURE v2.Country_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_1009' -- Country (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1009 d WITH (NOLOCK) -- Country (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown Country sync source.', 1;
    END
END
GO
