USE [mdm];
GO

/*
    Первичная заливка snapshot Currency.
    Источник — v2.vw_Currency_SnapshotSource.
    SyncState не инициализируется: инкрементального job нет.
*/

INSERT INTO v2.Currency_Snapshot (
     CurrencyId
    ,IsArchive
    ,Code
    ,Name
    ,Symbolic
    ,Digital
    ,Code_NSI
    ,CreationDate
    ,LastChangeDate
)
SELECT
     CurrencyId
    ,IsArchive
    ,Code
    ,Name
    ,Symbolic
    ,Digital
    ,Code_NSI
    ,CreationDate
    ,LastChangeDate
FROM v2.vw_Currency_SnapshotSource;
GO
