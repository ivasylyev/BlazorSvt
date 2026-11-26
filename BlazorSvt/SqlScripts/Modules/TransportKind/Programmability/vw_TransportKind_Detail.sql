USE [mdm];
GO

/*
    Карточка TransportKind. Источник — та же проекция, что и снимок
    (dbo.vw_TransportKind). Ключ — TransportKindId.
*/

CREATE OR ALTER VIEW v2.vw_TransportKind_Detail
AS
    SELECT
        TransportKindId,
        Code,
        Name,
        NameEnRu,
        StatusId        AS StatusIdRu,
        StatusId        AS StatusIdEn,
        Level,
        KindCode,
        CreationUserName,
        CreationDate,
        LastChangeUserName,
        LastChangeDate,
        IsArchive
    FROM v2.vw_TransportKind_SnapshotSource;
GO
