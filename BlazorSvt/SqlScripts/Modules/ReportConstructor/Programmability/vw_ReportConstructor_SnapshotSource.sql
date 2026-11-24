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

        r.CreationDate                                                           AS CreationDate,
        ISNULL(r.LastChangeDate, r.CreationDate)                                 AS LastChangeDate
    FROM dbo.vw_ReportConstructor r (NOLOCK)
    LEFT JOIN dbo.vw_LocationsNodes nf (NOLOCK) ON r.NodeFrom = nf.Id
    LEFT JOIN dbo.vw_LocationsNodes nt (NOLOCK) ON r.NodeTo = nt.Id
    LEFT JOIN dbo.vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    LEFT JOIN dbo.vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    LEFT JOIN dbo.vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN dbo.vw_MTR p (NOLOCK) ON r.Product = p.Id;
GO
