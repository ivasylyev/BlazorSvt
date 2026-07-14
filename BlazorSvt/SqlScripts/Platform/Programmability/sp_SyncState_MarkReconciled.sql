USE [mdm];
GO

/*
    Фиксирует время последней reconciliation по всем источникам справочника.
    Поле LastReconcileUtc — только для наблюдаемости (лаг/алерты).

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_MarkReconciled
    @Entity NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE v2.SyncState
    SET LastReconcileUtc = SYSUTCDATETIME()
    WHERE Entity = @Entity;
END
GO
