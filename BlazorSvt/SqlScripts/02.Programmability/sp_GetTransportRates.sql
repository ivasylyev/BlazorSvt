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
            {"ColumnName": "CurrencyCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RateTypeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RateTypeName", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportKindCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportKindName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportTypeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "TransportTypeName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductName' + @LangSuffix + '", "ColumnType": "NVARCHAR"},
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
            [TransportKindId],     
            [TransportTypeId],     
            [ProductGroupId],      
            [ProductId],           
            [RateTypeId],          

            [Code],
            [CurrencyCode],
            [RateTypeCode],
            [RateTypeName],
            [NodeFromCode],
            [NodeFromName' + @LangSuffix + '] AS [NodeFromName] ,
            [ProxyNodeCode],
            [ProxyNodeName' + @LangSuffix + '] AS [ProxyNodeName],
            [NodeToCode],
            [NodeToName' + @LangSuffix + '] AS [NodeToName],
            [TransportKindCode],
            [TransportKindName' + @LangSuffix + '] AS [TransportKindName],
            [TransportTypeCode],
            [TransportTypeName' + @LangSuffix + '] AS [TransportTypeName],
            [ProductGroupCode],
            [ProductGroupName' + @LangSuffix + '] AS [ProductGroupName],
            [ProductCode],
            [ProductName' + @LangSuffix + '] AS [ProductName],
            [ContractorCode],
            [ContractorEGRUL]
            '

    SET @FilterJson = REPLACE(@FilterJson, N'NodeFromName', N'NodeFromName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'NodeToName', N'NodeToName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'ProxyNodeName', N'ProxyNodeName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'TransportKindName', N'TransportKindName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'TransportTypeName', N'TransportTypeName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'ProductGroupName', N'ProductGroupName' + @LangSuffix)
    SET @FilterJson = REPLACE(@FilterJson, N'ProductName', N'ProductName' + @LangSuffix)

    SET @SortKey = REPLACE(@SortKey, N'NodeFromName', N'NodeFromName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'NodeToName', N'NodeToName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'ProxyNodeName', N'ProxyNodeName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'TransportKindName', N'TransportKindName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'TransportTypeName', N'TransportTypeName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'ProductGroupName', N'ProductGroupName' + @LangSuffix)
    SET @SortKey = REPLACE(@SortKey, N'ProductName', N'ProductName' + @LangSuffix)
    --print @FilterJson

   --return

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
