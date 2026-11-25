USE [mdm];
GO

/*
    Первичная заливка snapshot RateSyncClass.
    Источник — v2.vw_RateSyncClass_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.RateSyncClass_Snapshot (
     RateSyncClassId
    ,IsArchive
    ,Code
    ,Name
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
)
SELECT
     RateSyncClassId
    ,IsArchive
    ,Code
    ,Name
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
FROM v2.vw_RateSyncClass_SnapshotSource;
GO
