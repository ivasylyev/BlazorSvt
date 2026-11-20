USE [mdm];
GO

/*
    Первичная заливка snapshot Relevance.
    Источник — v2.vw_Relevance_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.Relevance_Snapshot (
     Code
    ,Name
    ,RelevanceId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
)
SELECT
     Code
    ,Name
    ,RelevanceId
    ,IsArchive
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Relevance_SnapshotSource;
GO
