USE [mdm];
GO

/*
    Наибольшая гарантированно закоммиченная версия строк (граница цикла @Hi).

    MIN_ACTIVE_ROWVERSION() возвращает наименьшую версию ещё открытой транзакции
    (или @@DBTS+1, если открытых нет). Всё, что строго меньше — уже закоммичено,
    поэтому @Hi = MIN_ACTIVE_ROWVERSION() - 1. Строки незавершённых транзакций
    исключаются -> нет пропусков и не нужен RCSI.

    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_GetHighWatermark
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1) AS Hi;
END
GO
