USE [mdm];
GO

/*
    RowVer для Coefficient. Таблица dbo.PrimitiveEntityData_2081.
    Идемпотентно. ALTER ADD rowversion на таблице с данными — Sch-M, maintenance-окно.
    Индекс по RowVer не ставится: таблица мала.
*/

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2081', N'U') IS NULL
BEGIN
    RAISERROR(N'Таблица dbo.PrimitiveEntityData_2081 не найдена.', 16, 1);
END
ELSE IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2081') AND name = N'RowVer')
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    ALTER TABLE dbo.PrimitiveEntityData_2081 ADD RowVer rowversion NOT NULL;
END
GO
