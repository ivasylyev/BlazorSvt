USE [mdm];
GO

/*
    Возвращает курсор (LastRowVersion) источника или пусто, если строки ещё нет
    (справочник не проинициализирован — требуется первичная заливка/seed).

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_Get
    @Entity     NVARCHAR(100),
    @SourceName NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT LastRowVersion
    FROM v2.SyncState
    WHERE Entity = @Entity AND SourceName = @SourceName;
END
GO
