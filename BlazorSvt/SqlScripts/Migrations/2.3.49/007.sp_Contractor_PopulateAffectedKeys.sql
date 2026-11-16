USE [mdm];
GO

/*
    Детекция изменившихся контрагентов.
    Источник: dbo.PrimitiveEntityData_2028 — LegalType (основная), реестр Contractor.

    Address, StatusElement, LegalForm и LevelOrganization в каскад не входят:
    на экран Contractor они не выводятся.
*/
CREATE OR ALTER PROCEDURE v2.Contractor_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2028' -- LegalType (основная), реестр Contractor
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2028 d WITH (NOLOCK) -- LegalType (основная), реестр Contractor
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown Contractor sync source.', 1;
    END
END
GO
