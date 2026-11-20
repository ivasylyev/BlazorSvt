USE [mdm];
GO

/*
    Карточка и полный Excel ReportConstructor.
    Источник — проекция легаси (те же соединения, что у снимка).
    Ключ — ReportConstructorId. Скрытые FK каскада в карточку не выводятся.
    ProxyNode исключён.
*/

CREATE OR ALTER VIEW v2.vw_ReportConstructor_Detail
AS
    SELECT
        ReportConstructorId,
        Code,
        ReportTypeId                                                         AS ReportTypeIdRu,
        ReportTypeId                                                         AS ReportTypeIdEn,
        DateFrom,
        DateTo,
        NodeFromCode,
        NodeFromNameRu,
        NodeFromNameEn,
        NodeToCode,
        NodeToNameRu,
        NodeToNameEn,
        TransportKindId                                                      AS TransportKindIdRu,
        TransportKindId                                                      AS TransportKindIdEn,
        TransportKindCode,
        TransportTypeId                                                      AS TransportTypeIdRu,
        TransportTypeId                                                      AS TransportTypeIdEn,
        TransportTypeCode,
        ProductGroupCode,
        ProductGroupNameRu,
        ProductGroupNameEn,
        ProductCode,
        ProductNameRu,
        ProductNameEn,
        CreationDate,
        LastChangeDate,
        IsArchive
    FROM v2.vw_ReportConstructor_SnapshotSource;
GO
