USE [mdm];
GO

/*
    Карточка RateCalcType. Источник — та же проекция, что и снимок
    (dbo.vw_RateCalcType). Ключ — RateCalcTypeId.
*/

CREATE OR ALTER VIEW v2.vw_RateCalcType_Detail
AS
    SELECT
        RateCalcTypeId,
        Code,
        CreationDate,
        LastChangeDate,
        IsArchive,
        Name,
        SearchCode
    FROM v2.vw_RateCalcType_SnapshotSource;
GO
