USE [mdm];
GO

/*
    Первичная заливка snapshot TransportTypeLevel3.
    Источник — v2.vw_TransportTypeLevel3_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.TransportTypeLevel3_Snapshot (
     TransportTypeLevel3Id
    ,IsArchive
    ,IsBasic_forCBD
    ,Code
    ,TransportKindId
    ,TransportTypeId
    ,Name
    ,NameEnRu
    ,TypeCode
    ,SAPCode
    ,OrionCode
    ,CreationDate
    ,LastChangeDate
)
SELECT
     TransportTypeLevel3Id
    ,IsArchive
    ,IsBasic_forCBD
    ,Code
    ,TransportKindId
    ,TransportTypeId
    ,Name
    ,NameEnRu
    ,TypeCode
    ,SAPCode
    ,OrionCode
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_TransportTypeLevel3_SnapshotSource;
GO
