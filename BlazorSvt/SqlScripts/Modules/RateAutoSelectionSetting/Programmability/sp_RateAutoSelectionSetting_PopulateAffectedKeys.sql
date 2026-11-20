USE [mdm];
GO

/*
    Детекция изменившихся настроек автоподбора.
    Источник: dbo.PrimitiveEntityData_2101 — RateAutoSelectionSetting (основная).
    Каскада нет.
*/
CREATE OR ALTER PROCEDURE v2.RateAutoSelectionSetting_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2101' -- RateAutoSelectionSetting (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2101 d WITH (NOLOCK) -- RateAutoSelectionSetting (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown RateAutoSelectionSetting sync source.', 1;
    END
END
GO
