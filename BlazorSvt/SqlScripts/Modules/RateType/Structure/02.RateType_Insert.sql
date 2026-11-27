USE [mdm];
GO

/*
    Первичная заливка snapshot RateType.
    Источник — v2.vw_RateType_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.RateType_Snapshot (
     RateTypeId
    ,IsArchive
    ,Code
    ,Name
    ,IsUseNomination
    ,IsDeflated
    ,Comment
    ,IsIntoSyncClass
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
)
SELECT
     RateTypeId
    ,IsArchive
    ,Code
    ,Name
    ,IsUseNomination
    ,IsDeflated
    ,Comment
    ,IsIntoSyncClass
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
FROM v2.vw_RateType_SnapshotSource;
GO
