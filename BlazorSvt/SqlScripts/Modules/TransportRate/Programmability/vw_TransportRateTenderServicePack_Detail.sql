USE [mdm];
GO

/*
    Таблица «Тендерный пакет услуг» на карточке TransportRate.
    Источник — dbo.vw_TransportRateTenderServicePack (dbo.PrimitiveEntityData_2089), дочерний примитив TransportRate.
    Отбор по TransportRateId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_TransportRateTenderServicePack_Detail
AS
    SELECT
        CAST(p.TransportRateId AS BIGINT)       AS TransportRateId,
        CAST(p.Id AS BIGINT)                    AS Id,
        p.PrimitiveEntityDataStateId            AS PrimitiveEntityDataStateId,
        LEFT(p.Code, 50)                        AS Code,
        LEFT(p.TenderServicePack, 100)          AS TenderServicePack,
        CAST(p.LotRate AS DECIMAL(19, 2))       AS LotRate,
        CAST(p.LotCurrency AS BIGINT)           AS LotCurrencyId
    FROM dbo.vw_TransportRateTenderServicePack p (NOLOCK);
GO
