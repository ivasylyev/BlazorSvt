USE mdm
GO

/*
Example:

use mdm
go

-- простой поиск (это когда или нет сортировки, или менее 2х полнотекстовых фильтров)

exec v2.TransportRates_Get 
    @PageNumber=1,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=NULL,
    @SortDirection=NULL,
    @FilterJson=N'[
        {"PropertyName":"RateTypeIdRu","Value":"543746","Operator":"Equals"},
        {"PropertyName":"NodeFromNameRu","Value":"казань","Operator":"Contains"},
        {"PropertyName":"ProxyNodeNameRu","Value":"находка","Operator":"Contains"},
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
        ]'


-- сложный поиск (это когда сортировка с двумя и более полнотекстовыми фильтрами)

exec v2.TransportRates_Get 
    @PageNumber=2,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=N'StartDate',
    @SortDirection=N'ASC',
    @FilterJson=N'[{"PropertyName":"RateTypeIdRu","Value":"543746","Operator":"Equals"},
        {"PropertyName":"TransportKindIdRu","Value":"543763","Operator":"Equals"},
        {"PropertyName":"TransportTypeIdRu","Value":"9687420","Operator":"Equals"},
        {"PropertyName":"NodeFromNameRu","Value":"нижн","Operator":"Contains"},
        {"PropertyName":"ProxyNodeNameRu","Value":"нах","Operator":"Contains"},
        {"PropertyName":"ProductGroupNameRu","Value":"кау","Operator":"Contains"},
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}]'

*/
DROP PROCEDURE IF EXISTS v2.GetTransportRates;
GO

CREATE OR ALTER PROCEDURE v2.TransportRates_Get
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
            {"ColumnName": "Code", "ColumnType": "ID"},
            {"ColumnName": "RateTypeCode", "ColumnType": "ID"},
            {"ColumnName": "CurrencyId", "ColumnType": "ID"},
            {"ColumnName": "RateTypeId", "ColumnType": "ID"},
            {"ColumnName": "NodeFromCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportKindId", "ColumnType": "ID"},
            {"ColumnName": "TransportTypeId", "ColumnType": "ID"},
            {"ColumnName": "ProductGroupId", "ColumnType": "ID"},
            {"ColumnName": "ProductGroupCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductCode", "ColumnType": "ID"},
            {"ColumnName": "ProductNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ContractorCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ContractorEGRUL", "ColumnType": "NVARCHAR"},

            {"ColumnName": "CreationDate", "ColumnType": "DATE"},
            {"ColumnName": "LastChangeDate", "ColumnType": "DATE"},
            {"ColumnName": "StartDate", "ColumnType": "DATE"},
            {"ColumnName": "EndDate", "ColumnType": "DATE"},
            {"ColumnName": "IsArchive", "ColumnType": "BIT"},
            {"ColumnName": "IsDefRate", "ColumnType": "BIT"}
        ]'


    SET @SelectList = CASE WHEN @KeysOnly = 1 THEN
        N'
        SELECT
            RateId
  '
    ELSE N'
        SELECT
            Id,
            RateId,
            IsArchive,
            IsDefRate,
            CAST(StartDate AS DATE)   AS StartDate,
            CAST(EndDate AS DATE)     AS EndDate,
            CreationDate,
            LastChangeDate,

            TotalCostTon,
            TotalCostTransport,

            TransportKindId AS TransportKindIdRu, 
            TransportKindId AS TransportKindIdEn, 
            TransportTypeId AS TransportTypeIdRu,     
            TransportTypeId AS TransportTypeIdEn,
            ProductGroupId AS ProductGroupIdRu,      
            ProductGroupId AS ProductGroupIdEn, 
            ProductId,           
            RateTypeId AS RateTypeIdRu,          
            RateTypeId AS RateTypeIdEn,   
            CurrencyId,

            Code,
            CurrencyCode,
            RateTypeCode,
            NodeFromCode,
            NodeFromNameRu,
            NodeFromNameEn,
            ProxyNodeCode,
            ProxyNodeNameRu,
            ProxyNodeNameEn,
            NodeToCode,
            NodeToNameRu,
            NodeToNameEn,
            TransportKindCode,
            TransportTypeCode,
            ProductGroupCode,
            ProductGroupNameRu,
            ProductGroupNameEn,
            ProductCode,
            ProductNameRu,
            ProductNameEn
  '
    END


    EXEC v2.GetBlazorGridData 
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @LangSuffix = @LangSuffix,

        @SortKey = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson = @FilterJson,

        @TableName = N'mdm.v2.TransportRateSnapshot',
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList = @SelectList
        
        
END
GO
