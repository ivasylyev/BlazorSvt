--------------------------------------------------------------------------------
-- [1/21] AverageRateLevel3 :: vw_AverageRateLevel3_SnapshotSource.sql
-- Source: Modules\AverageRateLevel3\Programmability\vw_AverageRateLevel3_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot AverageRateLevel3 из legacy-вью — единый источник истины
    для read-модели. Используется первичной заливкой и инкрементальной синхронизацией.

    Членство в snapshot:
      - RateLevel3 IS NOT NULL
      - валидные Code / RateType / Product (как TransportRate)
      - EXISTS хотя бы одной связанной TransportRate с тем же PrimitiveEntityDataStateId
*/

CREATE OR ALTER VIEW v2.vw_AverageRateLevel3_SnapshotSource
AS
    SELECT
        CAST(ar.Id AS INT)                                              AS AverageRateLevel3Id,
        CASE WHEN ISNULL(ar.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(ar.IsDefRate, 0)                                         AS IsDefRate,
        ar.StartDate                                                    AS StartDate,
        ar.EndDate                                                      AS EndDate,
        cu.DisplayName                                                  AS CreationUserName,
        ar.CreationDate                                                 AS CreationDate,
        lu.DisplayName                                                  AS LastChangeUserName,
        ISNULL(ar.LastChangeDate, ar.CreationDate)                        AS LastChangeDate,
        ar.RateLevel3                                                   AS RateLevel3,
        ISNULL(ar.EffectiveLoadOfTransportType, 0)                      AS EffectiveLoadOfTransportType,
        CAST(ar.TransportKind AS INT)                                   AS TransportKindId,
        CAST(ar.TransportType AS INT)                                   AS TransportTypeId,
        CAST(ar.RateType AS INT)                                        AS RateTypeId,
        CAST(ar.CurrencyStandard AS INT)                                AS CurrencyId,
        TRY_CAST(LEFT(ar.Code, 10) AS INT)                              AS Code,
        TRY_CAST(LEFT(rt.Code, 2) AS INT)                               AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                                AS ProductCode,
        LEFT(cur.Code, 3)                                               AS CurrencyCode,
        LEFT(nf.Code, 10)                                               AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                            AS NodeFromNameRu,
        LEFT(np.Code, 10)                                               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                            AS ProxyNodeNameRu,
        LEFT(nt.Code, 10)                                               AS NodeToCode,
        LEFT(nt.Name_en, 30)                                            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                            AS NodeToNameRu,
        LEFT(tk.Code, 5)                                                AS TransportKindCode,
        LEFT(tt.Code, 20)                                               AS TransportTypeCode,
        LEFT(pg.Code, 5)                                                AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100)         AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)             AS ProductGroupNameEn,
        LEFT(p.NameShort_ru, 100)                                       AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                                       AS ProductNameEn,
        CAST(nf.Id AS INT)                                              AS NodeFromId,
        CAST(nt.Id AS INT)                                              AS NodeToId,
        CAST(np.Id AS INT)                                              AS ProxyNodeId,
        CAST(pg.Id AS INT)                                              AS ProductGroupId,
        CAST(p.Id AS INT)                                               AS ProductId
    FROM vw_AverageRateLevel3 ar (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON ar.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON ar.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON ar.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON ar.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON ar.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON ar.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON ar.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON ar.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON ar.CurrencyStandard = cur.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = ar.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(ar.LastChangeUserId, ar.CreationUserId)
    WHERE ar.RateLevel3 IS NOT NULL
      AND TRY_CAST(LEFT(ar.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
      AND EXISTS (
          SELECT 1
          FROM vw_RatesOfAverageRate rar (NOLOCK)
          JOIN vw_TransportRate r (NOLOCK) ON r.Id = rar.Rate
          WHERE rar.AverageRateLevel3Id = ar.Id
            AND rar.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
            AND r.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
      );
GO

--------------------------------------------------------------------------------
-- [2/21] Coefficient :: vw_Coefficient_SnapshotSource.sql
-- Source: Modules\Coefficient\Programmability\vw_Coefficient_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Coefficient из dbo.vw_Coefficient — единый источник
    для первичной заливки (02.Coefficient_Insert.sql) и инкрементального MERGE.

    EntityInfo Coefficient = 2108. Примитив Coefficient = 2081,
    таблица dbo.PrimitiveEntityData_2081.

    CoefficientType (2077) и RateType (2048) — короткие словари с enum,
    в Sources не входят. Дочерние CoefficientConditionValue (2080) и
    CoefficientComponentValue (2083) в снимок не входят.
*/

CREATE OR ALTER VIEW v2.vw_Coefficient_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS CoefficientId,
        TRY_CAST(c.Code AS INT)                                                 AS Code,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        CAST(c.[Type] AS BIGINT)                                                 AS CoefficientTypeId,
        LEFT(ct.Code, 50)                                                        AS CoefficientTypeCode,
        LEFT(ct.Name, 50)                                                        AS CoefficientTypeNameRu,

        LEFT(c.Name, 50)                                                         AS Name,
        CAST(c.StartDate AS DATETIME)                                            AS StartDate,
        CAST(c.FinishDate AS DATETIME)                                           AS FinishDate,
        ISNULL(c.Active, 0)                                                      AS Active,
        ISNULL(c.OverallValue, 0)                                                AS OverallValue,

        CAST(c.RateType AS BIGINT)                                               AS RateTypeId,
        LEFT(rt.Code, 50)                                                        AS RateTypeCode,
        LEFT(rt.Name, 100)                                                       AS RateTypeNameRu,

        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Coefficient c (NOLOCK)
    LEFT JOIN dbo.vw_CoefficientType ct (NOLOCK) ON c.[Type] = ct.Id
    LEFT JOIN dbo.vw_RateType rt (NOLOCK) ON c.RateType = rt.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [3/21] Contractor :: vw_Contractor_SnapshotSource.sql
-- Source: Modules\Contractor\Programmability\vw_Contractor_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Contractor из dbo.vw_LegalType.
    vw_Contractor устарело и не используется.

    EntityInfo Contractor = 2054. Примитив LegalType = 2028,
    таблица dbo.PrimitiveEntityData_2028.

    Address, Status, LegalForm и LevelOrg в снимок не входят.
*/

CREATE OR ALTER VIEW v2.vw_Contractor_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS ContractorId,
        TRY_CAST(c.Code AS INT)                                                 AS Code,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        LEFT(c.NameSearch, 4000)                                                 AS NameSearch,
        ISNULL(c.Consignee, 0)                                                   AS Consignee,
        ISNULL(c.IsTrader, 0)                                                    AS IsTrader,
        ISNULL(c.IsProcessing, 0)                                                AS IsProcessing,
        ISNULL(c.IsSender, 0)                                                    AS IsSender,
        ISNULL(c.IsDebtor, 0)                                                    AS IsDebtor,
        ISNULL(c.IsCreditor, 0)                                                  AS IsCreditor,
        LEFT(c.INN, 4000)                                                        AS INN,

        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_LegalType c
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [4/21] Country :: vw_Country_SnapshotSource.sql
-- Source: Modules\Country\Programmability\vw_Country_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Country из dbo.vw_Country.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo Country = 2067. Примитив = 1009,
    таблица dbo.PrimitiveEntityData_1009.

    StatusElement (1010) в каскад не входит. RowVer на 1009 уже есть.
    Своих размножающих соединений нет.
*/

CREATE OR ALTER VIEW v2.vw_Country_SnapshotSource
AS
    SELECT
        TRY_CAST(c.Code AS INT)                                                 AS Code,
        LEFT(c.Name_ru, 4000)                                                    AS NameRu,
        CAST(c.ISOCode AS INT)                                                   AS ISOCode,
        LEFT(c.Name_en, 4000)                                                    AS NameEn,
        LEFT(c.PlanCode, 4000)                                                   AS PlanCode,
        LEFT(c.Market, 4000)                                                     AS Market,
        LEFT(c.ISOCode3, 4000)                                                   AS ISOCode3,
        CAST(c.Status AS BIGINT)                                                 AS StatusId,
        CAST(c.Id AS BIGINT)                                                     AS CountryId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        c.LastChangeDate                                                         AS LastChangeDate
    FROM dbo.vw_Country c (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [5/21] Currency :: vw_Currency_SnapshotSource.sql
-- Source: Modules\Currency\Programmability\vw_Currency_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Currency из legacy-вью.
    Используется только первичной заливкой (02.Currency_Insert.sql).

    Currency — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2016 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.Currency_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_Currency_SnapshotSource
AS
    SELECT
        CAST(c.Id AS BIGINT)                                                     AS CurrencyId,
        CASE WHEN ISNULL(c.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        LEFT(c.Code, 50)                                                         AS Code,
        LEFT(c.Name, 4000)                                                       AS Name,
        LEFT(c.Symbolic, 4000)                                                   AS Symbolic,
        CAST(c.Digital AS INT)                                                   AS Digital,
        TRY_CAST(c.Code_NSI AS INT)                                              AS Code_NSI,
        cu.DisplayName                                                           AS CreationUserName,
        c.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(c.LastChangeDate, c.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Currency c
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = c.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(c.LastChangeUserId, c.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [6/21] LocationsNodes :: vw_LocationsNodes_SnapshotSource.sql
-- Source: Modules\LocationsNodes\Programmability\vw_LocationsNodes_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot LocationsNodes из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.LocationsNodes_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.LocationsNodes_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    FK-колонки (LocationTypeId / TypeNodeId / RegionId / CountryId) входят в
    snapshot и попадают в грид, но также используются синхронизацией для
    каскадной инвалидации (изменение справочника -> затронутые узлы).
*/

CREATE OR ALTER VIEW v2.vw_LocationsNodes_SnapshotSource
AS
    SELECT
        CAST(n.Id AS BIGINT) AS LocationsNodesId,
        LEFT(n.Code, 50) AS Code,
        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        LEFT(n.Name_ru, 100) AS NameRu,
        LEFT(n.Name_en, 100) AS NameEn,

        CAST(n.LocationType AS BIGINT) AS LocationTypeId,
        LEFT(tp.Code, 50) AS LocationTypeCode,
        LEFT(tp.Name, 100) AS LocationTypeNameRu,
        LEFT(tp.NameEnRu, 100) AS LocationTypeNameEn,

        CAST(n.TypeNode AS BIGINT) AS TypeNodeId,
        LEFT(tn.Code, 50) AS TypeNodeCode,
        LEFT(tn.NameRu, 100) AS TypeNodeNameRu,
        LEFT(tn.NameEn, 100) AS TypeNodeNameEn,

        CAST(n.Region AS BIGINT) AS RegionId,
        LEFT(r.Code, 50) AS RegionCode,
        LEFT(r.Name_ru, 100) AS RegionNameRu,
        LEFT(r.Name_en, 100) AS RegionNameEn,
        LEFT(n.RegionRU, 100) AS RegionRU,

        CAST(n.Country AS BIGINT) AS CountryId,
        LEFT(c.Name_ru, 100) AS CountryNameRu,
        LEFT(c.Name_en, 100) AS CountryNameEn,

        cu.DisplayName AS CreationUserName,
        n.CreationDate AS CreationDate,
        lu.DisplayName AS LastChangeUserName,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = n.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(n.LastChangeUserId, n.CreationUserId)
GO

--------------------------------------------------------------------------------
-- [7/21] MTR :: vw_MTR_SnapshotSource.sql
-- Source: Modules\MTR\Programmability\vw_MTR_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot MTR из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.MTR_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.MTR_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    TransportName — ссылка на ProductGroup (whitelist: код + имена).
    ETSNGCode — ссылка на длинный словарь: в снимке имя. У ETSNG нет английского
    имени, EtsngNameEn повторяет Name. Таблица ETSNG в sync не входит.
    Unit и DangerClass — короткие словари, в гриде enum по Id.
    В каскад sync входит ProductGroup.

    Локальный PrimitiveEntityInfo.Id = 1015, таблица dbo.PrimitiveEntityData_1015.
    В реестре скоупа URL masterdata/2005.
*/

CREATE OR ALTER VIEW v2.vw_MTR_SnapshotSource
AS
    SELECT
        CAST(m.Id AS BIGINT)                                                     AS MTRId,
        TRY_CAST(m.Code AS INT)                                                 AS Code,
        CASE WHEN ISNULL(m.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,

        LEFT(m.NameFull_ru, 4000)                                                AS NameFullRu,
        LEFT(m.NameShort_en, 4000)                                               AS NameShortEn,

        CAST(m.TransportName AS BIGINT)                                          AS ProductGroupId,
        LEFT(pg.Code, 50)                                                        AS ProductGroupCode,
        LEFT(pg.Name, 4000)                                                      AS ProductGroupNameRu,
        LEFT(pg.NameEn, 4000)                                                    AS ProductGroupNameEn,

        CAST(m.ETSNGCode AS BIGINT)                                              AS EtsngId,
        LEFT(e.Name, 4000)                                                       AS EtsngNameRu,
        LEFT(e.Name, 4000)                                                       AS EtsngNameEn,

        LEFT(m.GNGCode, 4000)                                                    AS GngCode,

        CAST(m.Unit AS BIGINT)                                                   AS UnitId,
        LEFT(u.Code, 50)                                                         AS UnitCode,
        LEFT(u.Name_ru, 4000)                                                    AS UnitNameRu,
        LEFT(u.Name_en, 4000)                                                    AS UnitNameEn,

        LEFT(m.DPOCode, 4000)                                                    AS DpoCode,
        LEFT(m.MTRType, 4000)                                                    AS MtrType,

        CAST(m.DangerClass AS BIGINT)                                            AS DangerClassId,
        LEFT(dc.Code, 50)                                                        AS DangerClassCode,
        LEFT(dc.Name, 4000)                                                      AS DangerClassNameRu,

        cu.DisplayName                                                           AS CreationUserName,
        m.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(m.LastChangeDate, m.CreationDate)                                 AS LastChangeDate

    FROM dbo.vw_MTR m (NOLOCK)
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK) ON m.TransportName = pg.Id
    LEFT JOIN dbo.vw_ETSNG e (NOLOCK) ON m.ETSNGCode = e.Id
    LEFT JOIN dbo.vw_Unit u (NOLOCK) ON m.Unit = u.Id
    LEFT JOIN dbo.vw_DangerClass dc (NOLOCK) ON m.DangerClass = dc.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = m.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(m.LastChangeUserId, m.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [8/21] ParityRates :: vw_ParityRates_SnapshotSource.sql
-- Source: Modules\ParityRates\Programmability\vw_ParityRates_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot ParityRates из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.ParityRates_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Членство в snapshot:
      - Code NOT NULL / непустой
      - StartDate / EndDate / TotalCostTon / TotalCostTransport NOT NULL
      - NodeFrom / NodeTo / ProductGroup / Relevance NOT NULL
      - INNER JOIN на обязательные справочники (валюта, тип транспорта и т.д.)

    Relevance (2108), Currency (2016), TransportType (2023) — стабильные,
    в sync-каскад не входят.
*/

CREATE OR ALTER VIEW v2.vw_ParityRates_SnapshotSource
AS
    SELECT
        CAST(pr.Id AS INT)                                      AS ParityRatesId,
        CASE WHEN ISNULL(pr.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        pr.StartDate                                            AS StartDate,
        pr.EndDate                                              AS EndDate,
        cu.DisplayName                                          AS CreationUserName,
        pr.CreationDate                                         AS CreationDate,
        lu.DisplayName                                          AS LastChangeUserName,
        ISNULL(pr.LastChangeDate, pr.CreationDate)              AS LastChangeDate,

        LEFT(pr.Code, 50)                                       AS Code,
        CAST(pr.Relevance AS INT)                               AS RelevanceId,
        CAST(pr.TransportTypeCode AS INT)                       AS TransportTypeId,
        CAST(pr.CurrencyStandard AS INT)                        AS CurrencyId,

        pr.TotalCostTon                                         AS TotalCostTon,
        pr.TotalCostTransport                                   AS TotalCostTransport,
        pr.LoadOfTransport                                      AS LoadOfTransport,
        pr.Level_Danger_Product                                 AS Level_Danger_Product,
        pr.FactRate                                             AS FactRate,
        pr.BusinessPlanningRate                                 AS BusinessPlanningRate,

        LEFT(cur.Code, 3)                                       AS CurrencyCode,
        LEFT(tt.Code, 20)                                       AS TransportTypeCode,

        LEFT(nf.Code, 10)                                       AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                    AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                    AS NodeFromNameRu,

        LEFT(np1.Code, 10)                                      AS ProxyNode1Code,
        LEFT(np1.Name_en, 30)                                   AS ProxyNode1NameEn,
        LEFT(np1.Name_ru, 30)                                   AS ProxyNode1NameRu,

        LEFT(np2.Code, 10)                                      AS ProxyNode2Code,
        LEFT(np2.Name_en, 30)                                   AS ProxyNode2NameEn,
        LEFT(np2.Name_ru, 30)                                   AS ProxyNode2NameRu,

        LEFT(nt.Code, 10)                                       AS NodeToCode,
        LEFT(nt.Name_en, 30)                                    AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                    AS NodeToNameRu,

        LEFT(pg.Code, 5)                                        AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        TRY_CAST(LEFT(p.Code, 7) AS INT)                        AS ProductCode,
        LEFT(p.NameShort_ru, 100)                               AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                               AS ProductNameEn,

        LEFT(pr.Comment, 1000)                                  AS Comment,
        LEFT(pr.DataSource, 4000)                               AS DataSource,
        LEFT(pr.DepartmentResponsibilityArea, 4000)             AS DepartmentResponsibilityArea,
        LEFT(pr.EmployeeResponsibilityArea, 4000)               AS EmployeeResponsibilityArea,
        LEFT(pr.Methodology, 4000)                              AS Methodology,
        LEFT(pr.PriorityText, 4000)                             AS PriorityText,
        LEFT(pr.MarketingDataStructure, 4000)                   AS MarketingDataStructure,

        CAST(nf.Id AS INT)                                      AS NodeFromId,
        CAST(nt.Id AS INT)                                      AS NodeToId,
        CAST(np1.Id AS INT)                                     AS ProxyNode1Id,
        CAST(np2.Id AS INT)                                     AS ProxyNode2Id,
        CAST(pg.Id AS INT)                                      AS ProductGroupId,
        CAST(p.Id AS INT)                                       AS ProductId

    FROM vw_ParityRates pr (NOLOCK)
    JOIN vw_Relevance rel (NOLOCK) ON pr.Relevance = rel.Id
    JOIN vw_LocationsNodes nf (NOLOCK) ON pr.NodeFromCode = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON pr.NodeToCode = nt.Id
    LEFT JOIN vw_LocationsNodes np1 (NOLOCK) ON pr.ProxyNode1 = np1.Id
    LEFT JOIN vw_LocationsNodes np2 (NOLOCK) ON pr.ProxyNode2 = np2.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON pr.TransportTypeCode = tt.Id
    JOIN vw_ProductGroup pg (NOLOCK) ON pr.ProductGroupCode = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON pr.Product = p.Id
    JOIN vw_Currency cur (NOLOCK) ON pr.CurrencyStandard = cur.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = pr.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(pr.LastChangeUserId, pr.CreationUserId)

    WHERE pr.Code IS NOT NULL
      AND LTRIM(RTRIM(pr.Code)) <> N''
      AND pr.StartDate IS NOT NULL
      AND pr.EndDate IS NOT NULL
      AND pr.TotalCostTon IS NOT NULL
      AND pr.TotalCostTransport IS NOT NULL
      AND pr.LoadOfTransport IS NOT NULL
      AND pr.NodeFromCode IS NOT NULL
      AND pr.NodeToCode IS NOT NULL
      AND pr.ProductGroupCode IS NOT NULL
      AND pr.Relevance IS NOT NULL
GO

--------------------------------------------------------------------------------
-- [9/21] ProductGroup :: vw_ProductGroup_SnapshotSource.sql
-- Source: Modules\ProductGroup\Programmability\vw_ProductGroup_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot ProductGroup из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.ProductGroup_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.ProductGroup_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    RateSyncClass (dbo.PrimitiveEntityData_2127) — короткий словарь, в гриде enum
    по RateSyncClassId; в каскад синхронизации не входит.
*/

CREATE OR ALTER VIEW v2.vw_ProductGroup_SnapshotSource
AS
    SELECT
        CAST(pg.Id AS BIGINT)                                                    AS ProductGroupId,
        LEFT(pg.Code, 50)                                                        AS Code,
        CASE WHEN ISNULL(pg.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        LEFT(pg.Name, 4000)                                                      AS Name,
        LEFT(pg.ShortName, 4000)                                                 AS ShortName,
        LEFT(pg.GroupCode, 4000)                                                 AS GroupCode,
        LEFT(pg.ProductGroupCBD, 4000)                                           AS ProductGroupCBD,
        LEFT(pg.NameEnCode, 4000)                                                AS NameEnCode,
        LEFT(pg.NameEnRu, 4000)                                                  AS NameEnRu,
        LEFT(pg.Code_NSI, 50)                                                    AS Code_NSI,
        pg.SVTUseProductGroupWhenNoProductRateExists                             AS SVTUseProductGroupWhenNoProductRateExists,
        LEFT(pg.NameEn, 4000)                                                    AS NameEn,

        CAST(pg.RateSyncClass AS BIGINT)                                         AS RateSyncClassId,
        LEFT(rsc.Code, 50)                                                       AS RateSyncClassCode,
        LEFT(rsc.Name, 2000)                                                     AS RateSyncClassNameRu,

        cu.DisplayName                                                           AS CreationUserName,
        pg.CreationDate                                                          AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(pg.LastChangeDate, pg.CreationDate)                               AS LastChangeDate

    FROM dbo.vw_ProductGroup pg (NOLOCK)
    LEFT JOIN dbo.vw_RateSyncClass rsc (NOLOCK) ON pg.RateSyncClass = rsc.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = pg.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(pg.LastChangeUserId, pg.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [10/21] RateAutoSelectionSetting :: vw_RateAutoSelectionSetting_SnapshotSource.sql
-- Source: Modules\RateAutoSelectionSetting\Programmability\vw_RateAutoSelectionSetting_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot RateAutoSelectionSetting из dbo.vw_RateAutoSelectionSetting.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo RateAutoSelectionSetting = 2122. Примитив = 2101,
    таблица dbo.PrimitiveEntityData_2101. Других источников job нет.
*/

CREATE OR ALTER VIEW v2.vw_RateAutoSelectionSetting_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateAutoSelectionSettingId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        r.DestinationNodeToRegion                                                AS DestinationNodeToRegion,
        r.DestinationNodeToCountry                                               AS DestinationNodeToCountry,
        r.ProductToProductGroup                                                  AS ProductToProductGroup,
        r.AnyTransportKind                                                       AS AnyTransportKind,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        r.LastChangeDate                                                         AS LastChangeDate
    FROM dbo.vw_RateAutoSelectionSetting r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [11/21] RateCalcType :: vw_RateCalcType_SnapshotSource.sql
-- Source: Modules\RateCalcType\Programmability\vw_RateCalcType_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot RateCalcType из dbo.vw_RateCalcType.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2092 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateCalcType_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateCalcType_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateCalcTypeId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        LEFT(r.Code, 50)                                                         AS Code,
        LEFT(r.Name, 50)                                                         AS Name,
        LEFT(r.SearchCode, 50)                                                   AS SearchCode,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_RateCalcType r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [12/21] RateSyncClass :: vw_RateSyncClass_SnapshotSource.sql
-- Source: Modules\RateSyncClass\Programmability\vw_RateSyncClass_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot RateSyncClass из dbo.vw_RateSyncClass.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2127 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateSyncClass_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateSyncClass_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS RateSyncClassId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        LEFT(r.Name, 2000)                                                       AS Name,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_RateSyncClass r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [13/21] RateType :: vw_RateType_SnapshotSource.sql
-- Source: Modules\RateType\Programmability\vw_RateType_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot RateType из legacy-вью.
    Используется только первичной заливкой (02.RateType_Insert.sql).

    RateType — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2048 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.RateType_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_RateType_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                            AS RateTypeId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        TRY_CAST(r.Code AS INT)                                         AS Code,
        LEFT(r.Name, 4000)                                              AS Name,
        ISNULL(r.IsUseNomination, 0)                                    AS IsUseNomination,
        ISNULL(r.IsDeflated, 0)                                         AS IsDeflated,
        LEFT(r.Comment, 4000)                                           AS Comment,
        ISNULL(r.IsIntoSyncClass, 0)                                    AS IsIntoSyncClass,
        cu.DisplayName                                                  AS CreationUserName,
        r.CreationDate                                                  AS CreationDate,
        lu.DisplayName                                                  AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                        AS LastChangeDate
    FROM dbo.vw_RateType r
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId)
    WHERE r.Name IS NOT NULL;
GO

--------------------------------------------------------------------------------
-- [14/21] Region :: vw_Region_SnapshotSource.sql
-- Source: Modules\Region\Programmability\vw_Region_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Region из dbo.vw_Region.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo Region = 1003. Примитив = 1008,
    таблица dbo.PrimitiveEntityData_1008.

    Market — не колонка vw_Region: JOIN dbo.vw_Country.Market.
    StatusElement (1010) и Federal_Districts (2120) в каскад не входят.
    RowVer на dbo.PrimitiveEntityData_1008 уже есть.
*/

CREATE OR ALTER VIEW v2.vw_Region_SnapshotSource
AS
    SELECT
        LEFT(r.RegionCode, 4000)                                                 AS RegionCode,
        LEFT(r.Code_NSI, 255)                                                    AS Code_NSI,
        LEFT(r.RegionCodePlanning, 4000)                                         AS RegionCodePlanning,
        LEFT(r.RegionCodeRF, 4000)                                               AS RegionCodeRF,
        LEFT(r.Name_ru, 4000)                                                    AS NameRu,
        LEFT(r.Name_en, 4000)                                                    AS NameEn,
        LEFT(c.Name_ru, 4000)                                                    AS CountryNameRu,
        LEFT(c.Name_en, 4000)                                                    AS CountryNameEn,
        CAST(r.Country AS BIGINT)                                                AS CountryId,
        LEFT(c.Market, 4000)                                                     AS Market,
        CAST(r.Status AS INT)                                                    AS StatusId,
        CAST(r.Federal_Districts AS INT)                                         AS FederalDistrictsId,
        LEFT(fd.Code, 50)                                                        AS FederalDistrictsCode,
        LEFT(fd.FullName, 255)                                                   AS FederalDistrictsNameRu,
        LEFT(fd.FullNameEn, 255)                                                 AS FederalDistrictsNameEn,
        CAST(r.Id AS BIGINT)                                                     AS RegionId,
        LEFT(r.Code, 50)                                                         AS Code,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Region r (NOLOCK)
    LEFT JOIN dbo.vw_Country c (NOLOCK) ON r.Country = c.Id
    LEFT JOIN dbo.vw_Federal_Districts fd (NOLOCK) ON r.Federal_Districts = fd.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [15/21] Relevance :: vw_Relevance_SnapshotSource.sql
-- Source: Modules\Relevance\Programmability\vw_Relevance_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot Relevance из dbo.vw_Relevance.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2108 не добавляется.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.Relevance_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_Relevance_SnapshotSource
AS
    SELECT
        LEFT(r.Code, 50)                                                         AS Code,
        LEFT(r.Name, 100)                                                        AS Name,
        CAST(r.Id AS BIGINT)                                                     AS RelevanceId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_Relevance r (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [16/21] ReportConstructor :: vw_ReportConstructor_SnapshotSource.sql
-- Source: Modules\ReportConstructor\Programmability\vw_ReportConstructor_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot ReportConstructor из dbo.vw_ReportConstructor.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo ReportConstructor = 2149. Примитив = 2130,
    таблица dbo.PrimitiveEntityData_2130.

    Имена длинных словарей — NVARCHAR(255), без склейки «(код) имя».
    ProxyNode в проекцию не входит. ReportDictionaryTypes, TransportKind
    и TransportType_level_3 в каскад не входят: в снимке только Id и код.
*/

CREATE OR ALTER VIEW v2.vw_ReportConstructor_SnapshotSource
AS
    SELECT
        CAST(r.Id AS BIGINT)                                                     AS ReportConstructorId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        CAST(r.ReportType AS BIGINT)                                             AS ReportTypeId,
        CAST(r.DateFrom AS DATETIME2(7))                                         AS DateFrom,
        CAST(r.DateTo AS DATETIME2(7))                                           AS DateTo,

        LEFT(nf.Code, 50)                                                        AS NodeFromCode,
        LEFT(nf.Name_ru, 255)                                                    AS NodeFromNameRu,
        LEFT(nf.Name_en, 255)                                                    AS NodeFromNameEn,
        CAST(r.NodeFrom AS BIGINT)                                               AS NodeFromId,

        LEFT(nt.Code, 50)                                                        AS NodeToCode,
        LEFT(nt.Name_ru, 255)                                                    AS NodeToNameRu,
        LEFT(nt.Name_en, 255)                                                    AS NodeToNameEn,
        CAST(r.NodeTo AS BIGINT)                                                 AS NodeToId,

        LEFT(tk.Code, 50)                                                        AS TransportKindCode,
        CAST(r.TransportKind AS BIGINT)                                          AS TransportKindId,

        LEFT(tt.Code, 50)                                                        AS TransportTypeCode,
        CAST(r.TransportType AS BIGINT)                                          AS TransportTypeId,

        LEFT(pg.Code, 50)                                                        AS ProductGroupCode,
        LEFT(pg.ShortName, 255)                                                  AS ProductGroupNameRu,
        LEFT(pg.NameEn, 255)                                                     AS ProductGroupNameEn,
        CAST(r.ProductGroup AS BIGINT)                                           AS ProductGroupId,

        LEFT(p.Code, 50)                                                         AS ProductCode,
        LEFT(p.NameShort_ru, 255)                                                AS ProductNameRu,
        LEFT(p.NameShort_en, 255)                                                AS ProductNameEn,
        CAST(r.Product AS BIGINT)                                                AS ProductId,

        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_ReportConstructor r (NOLOCK)
    LEFT JOIN dbo.vw_LocationsNodes nf (NOLOCK) ON r.NodeFrom = nf.Id
    LEFT JOIN dbo.vw_LocationsNodes nt (NOLOCK) ON r.NodeTo = nt.Id
    LEFT JOIN dbo.vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    LEFT JOIN dbo.vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN dbo.vw_MTR p (NOLOCK) ON r.Product = p.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [17/21] TransportKind :: vw_TransportKind_SnapshotSource.sql
-- Source: Modules\TransportKind\Programmability\vw_TransportKind_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportKind из dbo.vw_TransportKind.
    Разовый снимок: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2008 не добавляется.

    StatusId — Id enum StatusElement; в каскад не входит.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportKind_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_TransportKind_SnapshotSource
AS
    SELECT
        LEFT(t.Code, 50)                                                         AS Code,
        LEFT(t.Name, 4000)                                                       AS Name,
        LEFT(t.NameEnRu, 4000)                                                   AS NameEnRu,
        CAST(t.Status AS BIGINT)                                                 AS StatusId,
        LEFT(t.Level, 4000)                                                      AS Level,
        LEFT(t.KindCode, 4000)                                                   AS KindCode,
        CAST(t.Id AS BIGINT)                                                     AS TransportKindId,
        CASE WHEN ISNULL(t.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        t.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(t.LastChangeDate, t.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportKind t (NOLOCK)
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = t.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(t.LastChangeUserId, t.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [18/21] TransportLeg :: vw_TransportLeg_SnapshotSource.sql
-- Source: Modules\TransportLeg\Programmability\vw_TransportLeg_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportLeg из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportLeg_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportLeg_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение Region/Node -> затронутые плечи). В грид не отдаются.

    ShipmentTypeCodeT — сырой legacy-атрибут (один или несколько кодов
    vw_ShipmentType через '/'). JOIN на vw_ShipmentType невозможен: связь 1-много.
*/

CREATE OR ALTER VIEW v2.vw_TransportLeg_SnapshotSource
AS
    SELECT
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        LEFT(NULLIF(LTRIM(RTRIM(l.ShipmentTypeCodeT)), N''), 30) AS ShipmentTypeCodeT,

        CAST(l.TransportKind AS INT)    AS TransportKindId,
        LEFT(tk.Code, 5)                AS TransportKindCode,

        LEFT(l.SearchTimeT, 20)         AS SearchTimeT,
        LEFT(l.LoadTimeT, 20)           AS LoadTimeT,
        LEFT(l.TravelTimeT, 20)         AS TravelTimeT,
        LEFT(l.DaysWaitingT, 20)        AS DaysWaitingT,
        LEFT(l.UnLoadTimeT, 20)         AS UnLoadTimeT,
        LEFT(l.TransportationTimeT, 20) AS TransportationTimeT,

        LEFT(nf.Code, 10)               AS NodeFromCode,
        LEFT(nf.Name_en, 30)            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)            AS NodeFromNameRu,
        LEFT(rf.Code, 10)               AS RegionFromCode,
        LEFT(rf.Name_en, 60)            AS RegionFromNameEn,
        LEFT(rf.Name_ru, 60)            AS RegionFromNameRu,

        LEFT(np.Code, 10)               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)            AS ProxyNodeNameRu,
        LEFT(rp.Code, 10)               AS ProxyRegionCode,
        LEFT(rp.Name_en, 60)            AS ProxyRegionNameEn,
        LEFT(rp.Name_ru, 60)            AS ProxyRegionNameRu,

        LEFT(nt.Code, 10)               AS NodeToCode,
        LEFT(nt.Name_en, 30)            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)            AS NodeToNameRu,
        LEFT(rt.Code, 10)               AS RegionToCode,
        LEFT(rt.Name_en, 60)            AS RegionToNameEn,
        LEFT(rt.Name_ru, 60)            AS RegionToNameRu,

        cu.DisplayName                  AS CreationUserName,
        l.CreationDate                  AS CreationDate,
        lu.DisplayName                  AS LastChangeUserName,
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)              AS NodeFromId,
        CAST(nt.Id AS INT)              AS NodeToId,
        CAST(np.Id AS INT)              AS ProxyNodeId,
        CAST(rf.Id AS INT)              AS RegionFromId,
        CAST(rt.Id AS INT)              AS RegionToId,
        CAST(rp.Id AS INT)              AS ProxyRegionId

    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = l.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(l.LastChangeUserId, l.CreationUserId)

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    LEFT JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    LEFT JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
GO

--------------------------------------------------------------------------------
-- [19/21] TransportRate :: vw_TransportRate_SnapshotSource.sql
-- Source: Modules\TransportRate\Programmability\vw_TransportRate_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportRate из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportRate_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT (включая WHERE-фильтр членства) для обоих путей
    гарантирует, что инкремент и полная пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportRate_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение LocationsNodes / ProductGroup / MTR -> затронутые рейты).
    В грид и DTO не отдаются.

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE. Источник MTR (1015) обязателен в каскаде
    процедуры детекции — иначе вход/выход строки отследит только суточный reconcile.
    RateType (2048) — стабильный справочник, в Sources не входит;
    смена членства по RateType ловится только reconcile.
*/

CREATE OR ALTER VIEW v2.vw_TransportRate_SnapshotSource
AS
    SELECT
        CAST(r.Id AS INT)                                   AS TransportRateId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(r.IsDefRate, 0)                              AS IsDefRate,
        r.StartDate                                         AS StartDate,
        r.EndDate                                           AS EndDate,
        cu.DisplayName                                      AS CreationUserName,
        r.CreationDate                                      AS CreationDate,
        lu.DisplayName                                      AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)            AS LastChangeDate,

        r.TotalCostTon                                      AS TotalCostTon,
        r.TotalCostTransport                                AS TotalCostTransport,

        CAST(r.TransportKind AS INT)                        AS TransportKindId,
        CAST(r.TransportType AS INT)                        AS TransportTypeId,
        CAST(r.RateType AS INT)                             AS RateTypeId,
        CAST(r.CurrencyStandard AS INT)                     AS CurrencyId,

        TRY_CAST(LEFT(r.Code, 10) AS INT)                   AS Code,

        TRY_CAST(LEFT(rt.Code, 2) AS INT)                   AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                    AS ProductCode,
        LEFT(cur.Code, 3)                                   AS CurrencyCode,

        LEFT(nf.Code, 10)                                   AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                AS NodeFromNameRu,

        LEFT(np.Code, 10)                                   AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                AS ProxyNodeNameRu,

        LEFT(nt.Code, 10)                                   AS NodeToCode,
        LEFT(nt.Name_en, 30)                                AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                AS NodeToNameRu,

        LEFT(tk.Code, 5)                                    AS TransportKindCode,
        LEFT(tt.Code, 20)                                   AS TransportTypeCode,

        LEFT(pg.Code, 5)                                    AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        LEFT(p.NameShort_ru, 100)                           AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                           AS ProductNameEn,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)                                  AS NodeFromId,
        CAST(nt.Id AS INT)                                  AS NodeToId,
        CAST(np.Id AS INT)                                  AS ProxyNodeId,
        CAST(pg.Id AS INT)                                  AS ProductGroupId,
        CAST(p.Id AS INT)                                   AS ProductId

    FROM vw_TransportRate r (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON r.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON r.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON r.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON r.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId)

    WHERE r.TotalCostTon IS NOT NULL
      AND r.TotalCostTransport IS NOT NULL
      AND TRY_CAST(LEFT(r.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
GO

--------------------------------------------------------------------------------
-- [20/21] TransportRoute :: vw_TransportRoute_SnapshotSource.sql
-- Source: Modules\TransportRoute\Programmability\vw_TransportRoute_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportRoute из dbo.vw_TransportRoute.
    Единый источник первичной заливки и инкрементального MERGE.

    EntityInfo TransportRoute = 2037. Примитив = 2010,
    таблица dbo.PrimitiveEntityData_2010.

    RegionFrom / RegionTo — не колонки view: узел -> LocationsNodes.Region -> vw_Region.
    TransportKind (2008) и TransportType_level_3 (2023) в каскад не входят.
    Плечи Leg_01..03 в снимок не денормализуются.
*/

CREATE OR ALTER VIEW v2.vw_TransportRoute_SnapshotSource
AS
    SELECT
        TRY_CAST(r.Code AS INT)                                                 AS Code,
        LEFT(r.CodeSAP, 50)                                                      AS CodeSAP,
        LEFT(r.Route, 4000)                                                      AS Route,

        CAST(r.StartNode AS BIGINT)                                              AS StartNodeId,
        LEFT(sn.Code, 50)                                                        AS StartNodeCode,
        LEFT(sn.Name_ru, 4000)                                                   AS StartNodeNameRu,
        LEFT(sn.Name_en, 4000)                                                   AS StartNodeNameEn,

        CAST(sn.Region AS BIGINT)                                                AS RegionFromId,
        LEFT(rf.Code, 50)                                                        AS RegionFromCode,
        LEFT(rf.Name_ru, 4000)                                                   AS RegionFromNameRu,
        LEFT(rf.Name_en, 4000)                                                   AS RegionFromNameEn,

        CAST(r.ProxyNode AS BIGINT)                                              AS ProxyNodeId,
        LEFT(pn.Code, 50)                                                        AS ProxyNodeCode,
        LEFT(pn.Name_ru, 4000)                                                   AS ProxyNodeNameRu,
        LEFT(pn.Name_en, 4000)                                                   AS ProxyNodeNameEn,

        CAST(r.EndNode AS BIGINT)                                                AS EndNodeId,
        LEFT(en.Code, 50)                                                        AS EndNodeCode,
        LEFT(en.Name_ru, 4000)                                                   AS EndNodeNameRu,
        LEFT(en.Name_en, 4000)                                                   AS EndNodeNameEn,

        CAST(en.Region AS BIGINT)                                                AS RegionToId,
        LEFT(rt.Code, 50)                                                        AS RegionToCode,
        LEFT(rt.Name_ru, 4000)                                                   AS RegionToNameRu,
        LEFT(rt.Name_en, 4000)                                                   AS RegionToNameEn,

        CAST(r.RouteTransportKind AS BIGINT)                                    AS RouteTransportKindId,
        LEFT(tk.Code, 50)                                                        AS RouteTransportKindCode,
        LEFT(tk.Name, 4000)                                                      AS RouteTransportKindName,
        LEFT(tk.NameEnRu, 4000)                                                  AS RouteTransportKindNameEnRu,

        r.IsActive                                                               AS IsActive,

        CAST(r.Leg1_TransportType AS BIGINT)                                    AS Leg1_TransportTypeId,
        LEFT(l1.Code, 50)                                                        AS Leg1_TransportTypeCode,
        LEFT(l1.Name, 4000)                                                      AS Leg1_TransportTypeName,
        LEFT(l1.NameEnRu, 4000)                                                  AS Leg1_TransportTypeNameEnRu,

        CAST(r.Leg2_TransportType AS BIGINT)                                    AS Leg2_TransportTypeId,
        LEFT(l2.Code, 50)                                                        AS Leg2_TransportTypeCode,
        LEFT(l2.Name, 4000)                                                      AS Leg2_TransportTypeName,
        LEFT(l2.NameEnRu, 4000)                                                  AS Leg2_TransportTypeNameEnRu,

        LEFT(r.ChainNodes, 4000)                                                 AS ChainNodes,

        CAST(r.Id AS BIGINT)                                                     AS TransportRouteId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END  AS IsArchive,
        cu.DisplayName                                                           AS CreationUserName,
        r.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportRoute r (NOLOCK)
    LEFT JOIN dbo.vw_LocationsNodes sn (NOLOCK) ON r.StartNode = sn.Id
    LEFT JOIN dbo.vw_Region rf (NOLOCK) ON sn.Region = rf.Id
    LEFT JOIN dbo.vw_LocationsNodes pn (NOLOCK) ON r.ProxyNode = pn.Id
    LEFT JOIN dbo.vw_LocationsNodes en (NOLOCK) ON r.EndNode = en.Id
    LEFT JOIN dbo.vw_Region rt (NOLOCK) ON en.Region = rt.Id
    LEFT JOIN dbo.vw_TransportKind tk (NOLOCK) ON r.RouteTransportKind = tk.Id
    LEFT JOIN dbo.vw_TransportType_level_3 l1 (NOLOCK) ON r.Leg1_TransportType = l1.Id
    LEFT JOIN dbo.vw_TransportType_level_3 l2 (NOLOCK) ON r.Leg2_TransportType = l2.Id
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId);
GO

--------------------------------------------------------------------------------
-- [21/21] TransportTypeLevel3 :: vw_TransportTypeLevel3_SnapshotSource.sql
-- Source: Modules\TransportTypeLevel3\Programmability\vw_TransportTypeLevel3_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportTypeLevel3 из legacy-вью dbo.vw_TransportType_level_3.
    Используется только первичной заливкой (02.TransportTypeLevel3_Insert.sql).

    TransportTypeLevel3 — стабильный словарь с enum: инкрементальной синхронизации нет,
    RowVer на dbo.PrimitiveEntityData_2023 не добавляется.

    TransportKindId (ParentCode_1), TransportTypeId (ParentCode_2) и StatusId (Status) —
    Id enum TransportKind, TransportType и StatusElement; в гриде показываются через enum, без JOIN.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportTypeLevel3_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).
*/

CREATE OR ALTER VIEW v2.vw_TransportTypeLevel3_SnapshotSource
AS
    SELECT
        CAST(t.Id AS BIGINT)                                                     AS TransportTypeLevel3Id,
        CASE WHEN ISNULL(t.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        t.IsBasic_forCBD                                                         AS IsBasic_forCBD,
        t.Code                                                                   AS Code,
        CAST(t.ParentCode_1 AS INT)                                              AS TransportKindId,
        CAST(t.ParentCode_2 AS INT)                                              AS TransportTypeId,
        LEFT(t.Name, 4000)                                                       AS Name,
        LEFT(t.NameEnRu, 4000)                                                   AS NameEnRu,
        LEFT(t.TypeCode, 4000)                                                   AS TypeCode,
        LEFT(t.SAPCode, 4000)                                                    AS SAPCode,
        LEFT(t.OrionCode, 4000)                                                  AS OrionCode,
        CAST(t.Status AS INT)                                                    AS StatusId,
        cu.DisplayName                                                           AS CreationUserName,
        t.CreationDate                                                           AS CreationDate,
        lu.DisplayName                                                           AS LastChangeUserName,
        ISNULL(t.LastChangeDate, t.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_TransportType_level_3 t
    LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = t.CreationUserId
    LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(t.LastChangeUserId, t.CreationUserId);
GO
