USE mdm
GO

/*
Example:

use mdm
go

SELECT *
FROM v2.vw_LocationsNodes_Detail
WHERE LocationsNodesId = 2254605

*/
DROP VIEW IF EXISTS v2.vw_LocationNodes_Detail;
GO

DROP VIEW IF EXISTS v2.vw_LocationsNodesDetail;
GO

CREATE OR ALTER VIEW v2.vw_LocationsNodes_Detail
AS
    SELECT
        CAST(n.Id AS BIGINT) AS LocationsNodesId,
        n.Code AS Code,

        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        n.NameZD AS NameZD,
        n.Seaport AS Seaport,
        n.AutoNode AS AutoNode,
        n.FIASRegion AS FIASRegion,
        n.FIASDistrict AS FIASDistrict,
        n.FIASCity AS FIASCity,
        n.Terminal AS Terminal,
        n.OpenStreetMap AS OpenStreetMap,
        n.Virtual AS Virtual,
        n.IsFactory AS IsFactory,
        n.IsPortShip AS IsPortShip,
        n.IsPortStore AS IsPortStore,
        n.IsStore AS IsStore,
        ped.a_3862 AS CoordinateW,
        ped.a_3863 AS CoordinateL,
        n.Code_NSI AS Code_NSI,
        n.Status AS Status,
        n.PointTube AS PointTube,
        n.Street AS Street,
        n.House AS House,
        n.OfficeApart AS OfficeApart,
        n.ZDCodeN AS ZDCodeN,
        n.NodesCode AS NodesCode,

        n.Name_ru AS NameRu,
        n.Name_en AS NameEn,

        CAST(n.LocationType AS BIGINT) AS LocationTypeIdRu,
        CAST(n.LocationType AS BIGINT) AS LocationTypeIdEn,
        tp.Code AS LocationTypeCode,
        tp.Name AS LocationTypeNameRu,
        tp.NameEnRu AS LocationTypeNameEn,

        CAST(n.TypeNode AS BIGINT) AS TypeNodeIdRu,
        CAST(n.TypeNode AS BIGINT) AS TypeNodeIdEn,
        tn.Code AS TypeNodeCode,
        tn.NameRu AS TypeNodeNameRu,
        tn.NameEn AS TypeNodeNameEn,

        CAST(n.Region AS BIGINT) AS RegionIdRu,
        CAST(n.Region AS BIGINT) AS RegionIdEn,
        r.Code AS RegionCode,
        r.Name_ru AS RegionNameRu,
        r.Name_en AS RegionNameEn,
        n.RegionRU AS RegionRU,

        CAST(n.Country AS BIGINT) AS CountryIdRu,
        CAST(n.Country AS BIGINT) AS CountryIdEn,
        c.Code AS CountryCode,
        c.Name_ru AS CountryNameRu,
        c.Name_en AS CountryNameEn,

        n.City AS City,
        n.FullAddress AS FullAddress,
        n.BorderCrossing AS BorderCrossing,
        n.LocationTypeCodeNSI AS LocationTypeCodeNSI,
        n.TypeNodeCodeNSI AS TypeNodeCodeNSI,
        n.RegionNSIEN AS RegionNSIEN,
        n.RegionCodeNSI AS RegionCodeNSI,
        n.RegionCodeDPO AS RegionCodeDPO,
        n.CountryRU AS CountryRU,
        n.NameCountryEN AS NameCountryEN,
        n.CountryISO2 AS CountryISO2,
        n.CountryISO3 AS CountryISO3,
        n.CountryCodeDPO AS CountryCodeDPO,
        n.MarketNameRU AS MarketNameRU,
        n.MarketNameEN AS MarketNameEN,
        n.MarketCode AS MarketCode,
        n.MarketCodeDPO AS MarketCodeDPO,
        n.CodeZDroad AS CodeZDroad,
        n.NameZDroadRU AS NameZDroadRU,
        n.NameZDroadEN AS NameZDroadEN,
        n.NameZDEN AS NameZDEN,
        n.AddressCountryISO2 AS AddressCountryISO2,
        n.AddressCountryISO3 AS AddressCountryISO3,
        n.AddressCountryCodeDPO AS AddressCountryCodeDPO,
        n.AddressNameCountryRU AS AddressNameCountryRU,
        n.AddressNameCountryEN AS AddressNameCountryEN,
        n.Pobox AS Pobox,
        n.NameFederalDistrictRU AS NameFederalDistrictRU,
        n.NameDistrictRU AS NameDistrictRU,
        n.NameCityRU AS NameCityRU,
        n.NameCityDistrictRU AS NameCityDistrictRU,
        n.IsKladr AS IsKladr,
        n.AddressLanguage AS AddressLanguage,
        n.RegionCodeRF AS RegionCodeRF,
        n.AddressRegionISO AS AddressRegionISO,
        n.FIASCodeCity AS FIASCodeCity,
        n.FIASStreet AS FIASStreet,
        n.FIASHouse AS FIASHouse,
        n.OKTMOCode AS OKTMOCode,
        n.FIASCodeAddress AS FIASCodeAddress,
        n.IsDadata AS IsDadata,
        n.IsArchive AS CannotDeliver,
        n.BorderCountryISO2 AS BorderCountryISO2,
        n.BorderCountryISO3 AS BorderCountryISO3,
        n.BorderCountryCodeDPO AS BorderCountryCodeDPO,
        n.BorderNameCountryRU AS BorderNameCountryRU,
        n.BorderNameCountryEN AS BorderNameCountryEN,
        n.StatusNSI AS StatusNSI,
        n.IsPlanning AS IsPlanning,
        n.IsPlan AS IsPlan,
        n.[4_level_CityRU] AS Level4CityRU,
        n.[4_level_CityEN] AS Level4CityEN,
        n.[4_level_City_FIAS] AS Level4CityFias,

        n.CreationDate AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    JOIN dbo.PrimitiveEntityData_1014 ped (NOLOCK) ON ped.PrimitiveEntityItemId = n.Id
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id
GO
