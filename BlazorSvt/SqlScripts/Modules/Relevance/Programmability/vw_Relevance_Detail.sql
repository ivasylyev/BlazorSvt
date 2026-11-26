USE [mdm];
GO

/*
    Карточка Relevance. Источник — та же проекция, что и снимок
    (dbo.vw_Relevance). Ключ — RelevanceId.
*/

CREATE OR ALTER VIEW v2.vw_Relevance_Detail
AS
    SELECT
        RelevanceId,
        Code,
        Name,
        CreationUserName,
        CreationDate,
        LastChangeUserName,
        LastChangeDate,
        IsArchive
    FROM v2.vw_Relevance_SnapshotSource;
GO
