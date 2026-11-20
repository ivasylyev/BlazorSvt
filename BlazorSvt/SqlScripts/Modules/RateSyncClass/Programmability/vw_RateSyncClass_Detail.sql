USE [mdm];
GO

/*
    Карточка RateSyncClass. Источник — та же проекция, что и снимок
    (dbo.vw_RateSyncClass). Ключ — RateSyncClassId.
*/

CREATE OR ALTER VIEW v2.vw_RateSyncClass_Detail
AS
    SELECT
        RateSyncClassId,
        IsArchive,
        Code,
        Name,
        CreationDate,
        LastChangeDate
    FROM v2.vw_RateSyncClass_SnapshotSource;
GO
