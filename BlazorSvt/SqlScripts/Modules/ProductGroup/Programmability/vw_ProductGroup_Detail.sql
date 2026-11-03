USE [mdm];
GO

/*
    Карточка ProductGroup. Поля совпадают с длинным списком атрибутов MDM
    плюс системные. Источник — та же проекция, что и снимок.
*/

CREATE OR ALTER VIEW v2.vw_ProductGroup_Detail
AS
    SELECT
        ProductGroupId,
        IsArchive,
        Code,
        Name,
        ShortName,
        GroupCode,
        ProductGroupCBD,
        NameEnCode,
        NameEnRu,
        Code_NSI,
        SVTUseProductGroupWhenNoProductRateExists,
        NameEn,
        RateSyncClassId AS RateSyncClassIdRu,
        RateSyncClassId AS RateSyncClassIdEn,
        CreationDate,
        LastChangeDate
    FROM v2.vw_ProductGroup_SnapshotSource;
GO
