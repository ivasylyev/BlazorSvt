USE mdm
GO

/*
Example:

use mdm
go

exec v2.LocationsNodes_Get
    @PageNumber=1,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=NULL,
    @SortDirection=NULL,
    @FilterJson=N'[
        {"PropertyName":"NameRu","Value":"казань","Operator":"Contains"},
        {"PropertyName":"RegionNameRu","Value":"татар","Operator":"Contains"},
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
        ]'

exec v2.LocationsNodes_Get
    @PageNumber=1,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=N'CreationDate',
    @SortDirection=N'ASC',
    @FilterJson=N'[
        {"PropertyName":"LocationTypeIdRu","Value":"42854","Operator":"Equals"},
        {"PropertyName":"NameRu","Value":"нижн","Operator":"Contains"},
        {"PropertyName":"RegionNameRu","Value":"моск","Operator":"Contains"},
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
        ]'

*/
DROP PROCEDURE IF EXISTS v2.GetLocationsNodes;
GO

DROP PROCEDURE IF EXISTS v2.LocationNodes_Get;
GO

CREATE OR ALTER PROCEDURE v2.LocationsNodes_Get
    @PageNumber     INT = 1,
    @PageSize       INT = 20,
    @Lang           NVARCHAR(2),
    @SortKey        NVARCHAR(50) = NULL,
    @SortDirection  NVARCHAR(5) = NULL,

    @FilterJson     NVARCHAR(MAX) = NULL,
    @KeysOnly       BIT = 0
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LangSuffix NVARCHAR(2),
            @AllowedColumnsJson NVARCHAR(MAX),
            @SelectList NVARCHAR(MAX)

    SET @LangSuffix = CASE
                        WHEN @Lang = N'ru' THEN N'Ru'
                        ELSE N'En'
                      END

    SET @AllowedColumnsJson = N'[
            {"ColumnName": "Code", "ColumnType": "NVARCHAR"},

            {"ColumnName": "IsArchive", "ColumnType": "BIT"},

            {"ColumnName": "NameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "LocationTypeId", "ColumnType": "ID"},
            {"ColumnName": "LocationTypeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "LocationTypeNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "LocationTypeNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "TypeNodeId", "ColumnType": "ID"},
            {"ColumnName": "TypeNodeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TypeNodeNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TypeNodeNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "RegionId", "ColumnType": "ID"},
            {"ColumnName": "RegionCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionRU", "ColumnType": "NVARCHAR"},

            {"ColumnName": "CountryId", "ColumnType": "ID"},
            {"ColumnName": "CountryCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "CountryNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "CountryNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "CreationDate", "ColumnType": "DATE"},
            {"ColumnName": "LastChangeDate", "ColumnType": "DATE"}
        ]'

    SET @SelectList = CASE WHEN @KeysOnly = 1 THEN
        N'
        SELECT
            LocationsNodesId
  '
    ELSE N'
        SELECT
            Id,
            LocationsNodesId,
            Code,

            IsArchive,

            NameRu,
            NameEn,

            LocationTypeId AS LocationTypeIdRu,
            LocationTypeId AS LocationTypeIdEn,
            LocationTypeCode,
            LocationTypeNameRu,
            LocationTypeNameEn,

            TypeNodeId AS TypeNodeIdRu,
            TypeNodeId AS TypeNodeIdEn,
            TypeNodeCode,
            TypeNodeNameRu,
            TypeNodeNameEn,

            RegionId AS RegionIdRu,
            RegionId AS RegionIdEn,
            RegionCode,
            RegionNameRu,
            RegionNameEn,
            RegionRU,

            CountryId AS CountryIdRu,
            CountryId AS CountryIdEn,
            CountryCode,
            CountryNameRu,
            CountryNameEn,

            CreationDate,
            LastChangeDate
  '
    END

    EXEC v2.GetBlazorGridData
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @LangSuffix = @LangSuffix,

        @SortKey = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson = @FilterJson,

        @TableName = N'mdm.v2.LocationsNodes_Snapshot',
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList = @SelectList
END
GO
