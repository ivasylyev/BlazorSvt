USE [mdm];
GO

/*
    Таблица «Лидтаймы» на карточке TransportLeg.
    Источник — dbo.vw_Leadtime (dbo.PrimitiveEntityData_2126), дочерний примитив TransportLeg.
    Тип отправки — dbo.vw_ShipmentType.NameRu / NameEn.
    Отбор по TransportLegId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_Leadtime_Detail
AS
    SELECT
        CAST(lt.TransportLegId AS BIGINT)              AS TransportLegId,
        CAST(lt.Id AS BIGINT)                          AS Id,
        lt.PrimitiveEntityDataStateId                  AS PrimitiveEntityDataStateId,
        LEFT(lt.Code, 50)                              AS Code,
        CAST(lt.StartDate AS DATE)                     AS StartDate,
        CAST(lt.EndDate AS DATE)                       AS EndDate,
        LEFT(st.NameRu, 50)                            AS ShipmentTypeNameRu,
        LEFT(st.NameEn, 50)                            AS ShipmentTypeNameEn,
        CAST(lt.LeadTimeIsActive AS BIT)               AS LeadTimeIsActive,
        CAST(lt.CommercialLT AS DECIMAL(3, 0))         AS CommercialLT,
        CAST(lt.SearchTime AS DECIMAL(3, 0))           AS SearchTime,
        CAST(lt.LoadTime AS DECIMAL(3, 0))             AS LoadTime,
        CAST(lt.TravelTime AS DECIMAL(3, 0))           AS TravelTime,
        CAST(lt.DaysWaiting AS DECIMAL(3, 0))          AS DaysWaiting,
        CAST(lt.UnLoadTime AS DECIMAL(3, 0))           AS UnLoadTime,
        CAST(lt.TransportationTime AS DECIMAL(3, 0))   AS TransportationTime,
        CAST(lt.Distance AS DECIMAL(19, 0))            AS Distance,
        CAST(lt.Leg1_SearchTime AS DECIMAL(3, 0))      AS Leg1_SearchTime,
        CAST(lt.Leg1_LoadTime AS DECIMAL(3, 0))        AS Leg1_LoadTime,
        CAST(lt.Leg1_TravelTime AS DECIMAL(3, 0))      AS Leg1_TravelTime,
        CAST(lt.Leg1_DaysWaiting AS DECIMAL(3, 0))     AS Leg1_DaysWaiting,
        CAST(lt.Leg1_TransportationTime AS DECIMAL(3, 0)) AS Leg1_TransportationTime,
        CAST(lt.Leg1_Distance AS DECIMAL(19, 0))       AS Leg1_Distance,
        CAST(lt.Leg2_TravelTime AS DECIMAL(3, 0))      AS Leg2_TravelTime,
        CAST(lt.Leg2_DaysWaiting AS DECIMAL(3, 0))     AS Leg2_DaysWaiting,
        CAST(lt.Leg2_UploadTime AS DECIMAL(3, 0))      AS Leg2_UploadTime,
        CAST(lt.Leg2_TransportationTime AS DECIMAL(3, 0)) AS Leg2_TransportationTime,
        CAST(lt.Leg2_Distance AS DECIMAL(19, 0))       AS Leg2_Distance
    FROM dbo.vw_Leadtime lt (NOLOCK)
    LEFT JOIN dbo.vw_ShipmentType st (NOLOCK)
        ON st.Id = lt.ShipmentType;
GO
