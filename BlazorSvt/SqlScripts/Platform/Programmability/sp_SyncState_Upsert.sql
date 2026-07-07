USE [mdm];
GO

/*
    Создаёт/обновляет курсор источника после успешной обработки цикла.
    Продвижение идемпотентно: повтор с тем же @RowVersion безопасен.

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_Upsert
    @Entity         NVARCHAR(100),
    @SourceName     NVARCHAR(200),
    @RowVersion     BINARY(8),
    @AffectedCount  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE v2.SyncState AS tgt
    USING (SELECT @Entity AS Entity, @SourceName AS SourceName) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @RowVersion,
                   LastRunUtc = SYSUTCDATETIME(),
                   LastAffectedCount = @AffectedCount
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc, LastAffectedCount)
        VALUES (@Entity, @SourceName, @RowVersion, SYSUTCDATETIME(), @AffectedCount);
END
GO
