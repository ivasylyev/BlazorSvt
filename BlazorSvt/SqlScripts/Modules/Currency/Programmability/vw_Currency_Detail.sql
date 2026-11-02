USE [mdm];
GO

/*
    Карточка Currency. Поля совпадают с длинным списком атрибутов MDM
    плюс системные. Источник — та же проекция, что и снимок.
*/

CREATE OR ALTER VIEW v2.vw_Currency_Detail
AS
    SELECT
        CurrencyId,
        IsArchive,
        Code,
        Name,
        Symbolic,
        Digital,
        Code_NSI,
        CreationDate,
        LastChangeDate
    FROM v2.vw_Currency_SnapshotSource;
GO
