USE [mdm];
GO

/*
    RowVer для Contractor. Примитив LegalType, таблица dbo.PrimitiveEntityData_2028.
    Идемпотентно. ALTER ADD rowversion на таблице с данными — Sch-M, maintenance-окно.
*/

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2028', N'U') IS NULL
BEGIN
    RAISERROR(N'Таблица dbo.PrimitiveEntityData_2028 не найдена.', 16, 1);
END
ELSE IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2028') AND name = N'RowVer')
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    ALTER TABLE dbo.PrimitiveEntityData_2028 ADD RowVer rowversion NOT NULL;
END
GO

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2028', N'U') IS NOT NULL
   AND EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2028') AND name = N'RowVer')
   AND NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2028')
          AND name = N'ix_PrimitiveEntityData_2028_RowVer')
BEGIN
    CREATE NONCLUSTERED INDEX ix_PrimitiveEntityData_2028_RowVer
        ON dbo.PrimitiveEntityData_2028 (RowVer);
END
GO
