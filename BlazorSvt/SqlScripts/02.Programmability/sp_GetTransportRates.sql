USE mdm
GO

/*
Example:

 EXEC dbo.GetTransportRates 
     @PageNumber=1,
     @PageSize=10,
     @Lang = N'ru',
     @SortKey=N'NodeToName',
     @SortDirection=N'ASC',
     @FilterJson=N'[{"PropertType":null,"PropertyName":"Code","Value":"2763157","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromName","Value":"каз","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProductGroupName","Value":"полиоле","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"StartDate","Value":"2026-02-28","Operator":4,"StringComparison":5}]'

*/
CREATE OR ALTER PROCEDURE dbo.GetTransportRates
    @PageNumber     INT = 1,
    @PageSize       INT = 20,
    @Lang           NVARCHAR(2),
    @SortKey        NVARCHAR(50) = NULL,
    @SortDirection  NVARCHAR(5) = NULL,

    @FilterJson     NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @LangSuffix NVARCHAR(2),
            @AllowedColumnsJson NVARCHAR(MAX),
            @SelectList NVARCHAR(MAX)

    SET @LangSuffix = CASE WHEN @Lang = N'ru' THEN N'Ru' ELSE N'En' END
    SET @AllowedColumnsJson = N'[
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
            {"ColumnName": "TransportKindCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportTypeId", "ColumnType": "ID"},
            {"ColumnName": "TransportTypeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupId", "ColumnType": "ID"},
            {"ColumnName": "ProductCode", "ColumnType": "NVARCHAR"},
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
    SET @SelectList = N'
        SELECT
            [Id],
            [IsArchive],
            [IsDefRate],
            CAST([StartDate] AS DATE)   AS [StartDate],
            CAST([EndDate] AS DATE)     AS [EndDate],
            [CreationDate],
            [LastChangeDate],

            [TotalCostTon],
            [TotalCostTransport],

            [NodeFromId],          
            [ProxyNodeId],         
            [NodeToId],            
            [TransportKindId] AS [TransportKindIdRu], 
            [TransportKindId] AS [TransportKindIdEn], 
            [TransportTypeId] AS [TransportTypeIdRu],     
            [TransportTypeId] AS [TransportTypeIdEn],
            [ProductGroupId] AS [ProductGroupIdRu],      
            [ProductGroupId] AS [ProductGroupIdEn], 
            [ProductId],           
            [RateTypeId] AS [RateTypeIdRu],          
            [RateTypeId] AS [RateTypeIdEn],   
            [CurrencyId],

            [Code],
            [CurrencyCode],
            [RateTypeCode],
            [NodeFromCode],
            [NodeFromNameRu],
            [NodeFromNameEn],
            [ProxyNodeCode],
            [ProxyNodeNameRu],
            [ProxyNodeNameEn],
            [NodeToCode],
            [NodeToNameRu],
            [NodeToNameEn],
            [TransportKindCode],
            [TransportTypeCode],
            [ProductGroupCode],
            [ProductGroupNameRu],
            [ProductGroupNameEn],
            [ProductCode],
            [ProductNameRu],
            [ProductNameEn],
            [ContractorCode],
            [ContractorEGRUL]
  
  '
   SET @FilterJson = REPLACE(@FilterJson, N'TransportKindId' + @LangSuffix, N'TransportKindId') 
   SET @FilterJson = REPLACE(@FilterJson, N'TransportTypeId' + @LangSuffix, N'TransportTypeId')  
   SET @FilterJson = REPLACE(@FilterJson, N'ProductGroupId' + @LangSuffix, N'ProductGroupId')
   SET @FilterJson = REPLACE(@FilterJson, N'RateTypeId' + @LangSuffix, N'RateTypeId')


    EXEC dbo.GetBlazorGridData 
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @SortKey = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson = @FilterJson,

        @TableName = N'mdm.dbo.TransportRateSnapshot',
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList = @SelectList
        
END
GO
