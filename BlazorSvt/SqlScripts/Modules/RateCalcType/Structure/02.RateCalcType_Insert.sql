USE [mdm];
GO

/*
    Первичная заливка snapshot RateCalcType.
    Источник — v2.vw_RateCalcType_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.RateCalcType_Snapshot (
     RateCalcTypeId
    ,IsArchive
    ,Code
    ,Name
    ,SearchCode
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
)
SELECT
     RateCalcTypeId
    ,IsArchive
    ,Code
    ,Name
    ,SearchCode
    ,CreationUserName
    ,CreationDate
    ,LastChangeUserName
    ,LastChangeDate
FROM v2.vw_RateCalcType_SnapshotSource;
GO
