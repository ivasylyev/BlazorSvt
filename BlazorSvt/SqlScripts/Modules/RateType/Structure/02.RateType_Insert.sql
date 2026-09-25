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
    ,CreationDate
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
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_RateType_SnapshotSource;
GO
