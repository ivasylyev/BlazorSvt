USE [mdm];
GO

/*
    Первичная заливка snapshot TransportKind.
    Источник — v2.vw_TransportKind_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.TransportKind_Snapshot (
     Code
    ,Name
    ,NameEnRu
    ,StatusId
    ,Level
    ,KindCode
    ,TransportKindId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
)
SELECT
     Code
    ,Name
    ,NameEnRu
    ,StatusId
    ,Level
    ,KindCode
    ,TransportKindId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_TransportKind_SnapshotSource;
GO
