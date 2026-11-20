USE [mdm];
GO

/*
    Карточка Country. Источник — проекция легаси (vw_Country_SnapshotSource).
    Ключ — CountryId.
*/

CREATE OR ALTER VIEW v2.vw_Country_Detail
AS
    SELECT
        CountryId,
        Code,
        NameRu,
        ISOCode,
        NameEn,
        PlanCode,
        Market,
        ISOCode3,
        StatusId                                                            AS StatusIdRu,
        StatusId                                                            AS StatusIdEn,
        CreationDate,
        LastChangeDate,
        IsArchive
    FROM v2.vw_Country_SnapshotSource;
GO
