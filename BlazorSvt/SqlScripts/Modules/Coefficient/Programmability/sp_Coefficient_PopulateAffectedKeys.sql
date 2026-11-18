USE [mdm];
GO

/*
    Детекция изменившихся коэффициентов.
    Источник: dbo.PrimitiveEntityData_2081 — Coefficient (основная).

    CoefficientType (2077) и RateType (2048) — короткие словари с enum,
    не отслеживаются. Дочерние CoefficientConditionValue (2080) и
    CoefficientComponentValue (2083) на экран не выводятся и в каскад не входят.
*/
CREATE OR ALTER PROCEDURE v2.Coefficient_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2081' -- Coefficient (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2081 d WITH (NOLOCK) -- Coefficient (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    BEGIN
        ;THROW 50000, 'Unknown Coefficient sync source.', 1;
    END
END
GO
