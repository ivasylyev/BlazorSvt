USE [mdm];
GO

/*
    Карточка RateAutoSelectionSetting.
    Источник — та же проекция, что и снимок (dbo.vw_RateAutoSelectionSetting).
    Ключ — RateAutoSelectionSettingId.
*/

CREATE OR ALTER VIEW v2.vw_RateAutoSelectionSetting_Detail
AS
    SELECT
        RateAutoSelectionSettingId,
        Code,
        DestinationNodeToRegion,
        DestinationNodeToCountry,
        ProductToProductGroup,
        AnyTransportKind,
        CreationDate,
        LastChangeDate,
        IsArchive
    FROM v2.vw_RateAutoSelectionSetting_SnapshotSource;
GO
