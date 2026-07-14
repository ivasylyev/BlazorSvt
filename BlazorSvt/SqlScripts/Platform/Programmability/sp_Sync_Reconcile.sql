USE [mdm];
GO

/*
    Reconciliation: удаляет из snapshot "фантомы" — записи, физически удалённые
    в legacy (их нет в проекции). Ловит то, что инкремент по rowversion не видит
    (жёсткие delete). Generic по имени snapshot/проекции/ключа.

    Тяжёлый anti-join через проекцию (JOIN всех legacy-вью) — запускается редко
    (ночью, в blackout-окне инкремента), не в основном цикле.

    Возвращает число удалённых строк.
    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_Reconcile
    @SnapshotTable  NVARCHAR(300),
    @SourceView     NVARCHAR(300),
    @KeyColumn      SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID(@SnapshotTable) IS NULL
        THROW 50000, 'Snapshot table not found.', 1;
    IF OBJECT_ID(@SourceView) IS NULL
        THROW 50000, 'Source projection view not found.', 1;

    DECLARE @SafeSnap NVARCHAR(400) = QUOTENAME(PARSENAME(@SnapshotTable, 2)) + N'.' + QUOTENAME(PARSENAME(@SnapshotTable, 1));
    DECLARE @SafeView NVARCHAR(400) = QUOTENAME(PARSENAME(@SourceView, 2)) + N'.' + QUOTENAME(PARSENAME(@SourceView, 1));
    DECLARE @SafeKey  NVARCHAR(258) = QUOTENAME(@KeyColumn);

    DECLARE @Sql NVARCHAR(MAX) = N'
DELETE tgt
FROM ' + @SafeSnap + N' AS tgt
WHERE NOT EXISTS (
    SELECT 1 FROM ' + @SafeView + N' AS src
    WHERE src.' + @SafeKey + N' = tgt.' + @SafeKey + N');
SET @DeletedOut = @@ROWCOUNT;';

    DECLARE @Deleted INT;
    EXEC sp_executesql @Sql, N'@DeletedOut INT OUTPUT', @DeletedOut = @Deleted OUTPUT;

    SELECT @Deleted AS Deleted;
END
GO
