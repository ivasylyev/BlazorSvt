USE [mdm];
GO

SET ANSI_NULLS ON;
GO
SET QUOTED_IDENTIFIER ON;
GO

/*
    Каскад ETSNG в синхронизацию MTR.

    1) RowVer на dbo.PrimitiveEntityData_2019 (ETSNG). Аддитивно, Sch-M лок
       на время ALTER; таблица маленькая (~5 тыс. строк).
    2) Индекс по скрытому FK v2.MTR_Snapshot.EtsngId для ветки каскада
       в v2.MTR_PopulateAffectedKeys.
    3) Курсор v2.SyncState (MTR, dbo.PrimitiveEntityData_2019) на текущую
       границу версий. Ставится после ALTER: иначе все строки ETSNG получили бы
       версии выше курсора и первый инкремент пересобрал бы весь снимок MTR.

    Идемпотентно. Выполнять после 012.02.MTR_Insert.sql и до снимка
    programmability (процедура читает r.RowVer).
*/

IF OBJECT_ID(N'dbo.PrimitiveEntityData_2019', N'U') IS NULL
BEGIN
    RAISERROR(N'Таблица dbo.PrimitiveEntityData_2019 (ETSNG) не найдена — пропуск RowVer.', 10, 1);
END
ELSE IF NOT EXISTS (
    SELECT 1 FROM sys.columns
    WHERE object_id = OBJECT_ID(N'dbo.PrimitiveEntityData_2019') AND name = N'RowVer')
BEGIN
    PRINT N'Добавляю RowVer в dbo.PrimitiveEntityData_2019 (ETSNG)';
    EXEC sys.sp_executesql N'SET QUOTED_IDENTIFIER ON; SET ANSI_NULLS ON; ALTER TABLE dbo.PrimitiveEntityData_2019 ADD RowVer rowversion NOT NULL;';
END
ELSE
BEGIN
    PRINT N'RowVer уже есть в dbo.PrimitiveEntityData_2019 — пропуск.';
END
GO

DROP INDEX IF EXISTS [ix_MTR_Snapshot_EtsngId] ON [v2].[MTR_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_MTR_Snapshot_EtsngId]
ON v2.MTR_Snapshot (EtsngId)
WHERE EtsngId IS NOT NULL;
GO

IF OBJECT_ID(N'v2.SyncState', N'U') IS NOT NULL
BEGIN
    DECLARE @Hi BINARY(8) =
        CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1);

    MERGE v2.SyncState AS tgt
    USING (SELECT N'MTR' AS Entity, N'dbo.PrimitiveEntityData_2019' AS SourceName) AS src -- ETSNG
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc)
        VALUES (src.Entity, src.SourceName, @Hi, SYSUTCDATETIME());
END
GO
