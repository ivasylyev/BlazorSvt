USE [mdm];
GO

/*
    RowVer для TransportRoute. Таблица dbo.PrimitiveEntityData_2010.
    Идемпотентно. ALTER ADD rowversion на таблице с данными — Sch-M, maintenance-окно.
    NC-индекс по RowVer: таблица крупная (около 69 тыс. строк), детекция своего job.
*/

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2010', N'U') IS NULL
BEGIN
    RAISERROR(N'Таблица dbo.PrimitiveEntityData_2010 не найдена.', 16, 1);
END
ELSE IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2010') AND name = N'RowVer')
BEGIN
    SET QUOTED_IDENTIFIER ON;
    SET ANSI_NULLS ON;
    ALTER TABLE dbo.PrimitiveEntityData_2010 ADD RowVer rowversion NOT NULL;
END
GO

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2010', N'U') IS NOT NULL
   AND EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2010') AND name = N'RowVer')
   AND NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2010')
          AND name = N'ix_PrimitiveEntityData_2010_RowVer')
BEGIN
    CREATE NONCLUSTERED INDEX ix_PrimitiveEntityData_2010_RowVer
        ON dbo.PrimitiveEntityData_2010 (RowVer);
END
GO
