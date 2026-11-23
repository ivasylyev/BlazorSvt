USE [mdm];
GO

/*
    Таблица «Адреса контрагентов» на карточке Contractor.
    Источник — dbo.vw_Address (dbo.PrimitiveEntityData_2027), дочерний примитив Contractor.
    Ключ родителя во view — LegalTypeId.
    Вид адреса — dbo.vw_AddressType.Name. Регион — dbo.vw_Region. Страна — dbo.vw_Country.
    Имена колонок координат в источнике начинаются с кириллической С.
    Отбор по ContractorId и состоянию строки — в запросе секции, не здесь.
*/

CREATE OR ALTER VIEW v2.vw_Address_Detail
AS
    SELECT
        CAST(a.LegalTypeId AS BIGINT)            AS ContractorId,
        CAST(a.Id AS BIGINT)                     AS Id,
        a.PrimitiveEntityDataStateId             AS PrimitiveEntityDataStateId,
        LEFT(a.Code, 50)                         AS Code,
        LEFT(a.AddressType, 4000)                AS AddressType,
        LEFT(a.City, 4000)                       AS City,
        CAST(a.Status AS BIGINT)                 AS StatusIdRu,
        CAST(a.Status AS BIGINT)                 AS StatusIdEn,
        LEFT(at.Name, 4000)                      AS AddressKindName,
        LEFT(a.Pobox, 10)                        AS Pobox,
        LEFT(a.RegionCode, 50)                   AS RegionCode,
        LEFT(a.District_ru, 255)                 AS District_ru,
        LEFT(a.Street, 255)                      AS Street,
        LEFT(a.House, 50)                        AS House,
        LEFT(a.OfficeApart, 50)                  AS OfficeApart,
        LEFT(a.FullAddress_ru, 255)              AS FullAddress_ru,
        CAST(a.[СoordinateW] AS DECIMAL(10, 6))         AS CoordinateW,
        CAST(a.[СoordinateL] AS DECIMAL(10, 6))         AS CoordinateL,
        LEFT(a.FIASCodeRegion, 36)               AS FIASCodeRegion,
        LEFT(a.FIASCodeDistrict, 36)             AS FIASCodeDistrict,
        LEFT(a.FIASCodeSettlement, 36)           AS FIASCodeSettlement,
        rg.Name_ru                               AS RegionNameRu,
        rg.Name_en                               AS RegionNameEn,
        co.Name_ru                               AS CountryNameRu,
        co.Name_en                               AS CountryNameEn
    FROM dbo.vw_Address a (NOLOCK)
    LEFT JOIN dbo.vw_AddressType at (NOLOCK)
        ON at.Id = a.AddressKind
    LEFT JOIN dbo.vw_Region rg (NOLOCK)
        ON rg.Id = a.Region_ru
    LEFT JOIN dbo.vw_Country co (NOLOCK)
        ON co.Id = a.CountryCode;
GO
