USE [mdm];
GO

/*
    Карточка Region. Источник — проекция легаси (vw_Region_SnapshotSource).
    Ключ — RegionId. CountryId выводится как CountryIdRu / CountryIdEn.
*/

CREATE OR ALTER VIEW v2.vw_Region_Detail
AS
    SELECT
        RegionId,
        Code,
        CreationDate,
        LastChangeDate,
        IsArchive,
        NameRu,
        RegionCode,
        CountryNameRu,
        CountryNameEn,
        StatusId                                                            AS StatusIdRu,
        StatusId                                                            AS StatusIdEn,
        NameEn,
        RegionCodeRF,
        RegionCodePlanning,
        Market,
        Code_NSI,
        FederalDistrictsId                                                  AS FederalDistrictsIdRu,
        FederalDistrictsId                                                  AS FederalDistrictsIdEn,
        CountryId                                                           AS CountryIdRu,
        CountryId                                                           AS CountryIdEn
    FROM v2.vw_Region_SnapshotSource;
GO
