USE [mdm];
GO

SET QUOTED_IDENTIFIER ON;
GO

SET ANSI_NULLS ON;
GO

/*
    Партиционно-безопасный upsert затронутых ключей из проекции в snapshot.
    Generic: набор колонок читается из sys.columns целевой таблицы (кроме
    суррогатного Id), поэтому одна процедура обслуживает любой справочник.

    Требует, чтобы вызывающая сессия предварительно создала и наполнила
    временную таблицу #AffectedKeys(EntityKey BIGINT) — динамический SQL
    выполняется в той же сессии и видит её.

    Snapshot партиционирован по IsArchive, PK = (IsArchive, Id). Обе операции
    ограничены #AffectedKeys -> идут по индексу бизнес-ключа, без скана таблицы:
      1. Pre-DELETE строк, у которых сменился IsArchive (переход между
         партициями): старая строка удаляется, чтобы MERGE вставил новую.
         Записи с неизменным IsArchive не трогаются -> их суррогатный Id
         сохраняется при UPDATE.
      2. MERGE по (ключ, IsArchive): UPDATE на месте либо INSERT новой/
         перенесённой строки. NOT MATCHED BY SOURCE сознательно НЕ используется
         (вызвал бы скан всей таблицы) — физические удаления чинит Sync_Reconcile.

    Возвращает число строк, затронутых MERGE.
    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_UpsertAffected
    @SnapshotTable  NVARCHAR(300),
    @SourceView     NVARCHAR(300),
    @KeyColumn      SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SnapObjId INT = OBJECT_ID(@SnapshotTable);

    IF @SnapObjId IS NULL
        THROW 50000, 'Snapshot table not found.', 1;
    IF OBJECT_ID(@SourceView) IS NULL
        THROW 50000, 'Source projection view not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @SnapObjId AND name = @KeyColumn)
        THROW 50000, 'Key column not found in snapshot table.', 1;

    DECLARE @Archive SYSNAME = N'IsArchive';

    DECLARE @SafeSnap    NVARCHAR(400) = QUOTENAME(PARSENAME(@SnapshotTable, 2)) + N'.' + QUOTENAME(PARSENAME(@SnapshotTable, 1));
    DECLARE @SafeView    NVARCHAR(400) = QUOTENAME(PARSENAME(@SourceView, 2)) + N'.' + QUOTENAME(PARSENAME(@SourceView, 1));
    DECLARE @SafeKey     NVARCHAR(258) = QUOTENAME(@KeyColumn);
    DECLARE @SafeArchive NVARCHAR(258) = QUOTENAME(@Archive);

    DECLARE @InsertCols NVARCHAR(MAX), @InsertVals NVARCHAR(MAX), @UpdateSet NVARCHAR(MAX);

    -- Все физические колонки snapshot (в порядке определения), кроме суррогатного Id.
    SELECT @InsertCols = STUFF((
        SELECT N', ' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId AND c.name <> N'Id'
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    SELECT @InsertVals = STUFF((
        SELECT N', src.' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId AND c.name <> N'Id'
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    -- В UPDATE не трогаем бизнес-ключ и партиционную колонку IsArchive.
    SELECT @UpdateSet = STUFF((
        SELECT N', ' + QUOTENAME(c.name) + N' = src.' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId
          AND c.name NOT IN (N'Id', @KeyColumn, @Archive)
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    DECLARE @Sql NVARCHAR(MAX) = N'
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
DELETE tgt
FROM ' + @SafeSnap + N' AS tgt
WHERE tgt.' + @SafeKey + N' IN (SELECT EntityKey FROM #AffectedKeys)
  AND NOT EXISTS (
      SELECT 1 FROM ' + @SafeView + N' AS src
      WHERE src.' + @SafeKey + N' = tgt.' + @SafeKey + N'
        AND src.' + @SafeArchive + N' = tgt.' + @SafeArchive + N');

MERGE ' + @SafeSnap + N' WITH (HOLDLOCK) AS tgt
USING (
    SELECT src.* FROM ' + @SafeView + N' AS src
    WHERE src.' + @SafeKey + N' IN (SELECT EntityKey FROM #AffectedKeys)
) AS src
ON tgt.' + @SafeKey + N' = src.' + @SafeKey + N'
   AND tgt.' + @SafeArchive + N' = src.' + @SafeArchive + N'
WHEN MATCHED THEN UPDATE SET ' + @UpdateSet + N'
WHEN NOT MATCHED BY TARGET THEN INSERT (' + @InsertCols + N')
    VALUES (' + @InsertVals + N');
SET @AffectedOut = @@ROWCOUNT;';

    DECLARE @Affected INT;
    EXEC sp_executesql @Sql, N'@AffectedOut INT OUTPUT', @AffectedOut = @Affected OUTPUT;

    SELECT @Affected AS Affected;
END
GO
