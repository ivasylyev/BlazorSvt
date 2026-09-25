USE [mdm];
GO

/*
    Карточка RateType. Поля совпадают с длинным списком атрибутов MDM
    плюс системные. Источник — та же проекция, что и снимок.
*/

CREATE OR ALTER VIEW v2.vw_RateType_Detail
AS
    SELECT
        RateTypeId,
        IsArchive,
        Code,
        Name,
        IsUseNomination,
        IsDeflated,
        Comment,
        IsIntoSyncClass,
        CreationDate,
        LastChangeDate
    FROM v2.vw_RateType_SnapshotSource;
GO
