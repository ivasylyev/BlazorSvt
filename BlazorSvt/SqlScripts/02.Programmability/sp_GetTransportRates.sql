USE mdm
GO

/*
Example:

 EXEC dbo.GetTransportRates 
     @PageNumber=1,
     @PageSize=10,
     @SortKey=N'NodeToNameRu',
     @SortDirection=N'ASC',
     @FilterJson=N'[{"PropertType":null,"PropertyName":"Code","Value":"2763157","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"каз","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProductGroupName","Value":"полиоле","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"StartDate","Value":"2026-02-28","Operator":4,"StringComparison":5}]'

*/
CREATE OR ALTER PROCEDURE dbo.GetTransportRates
    @PageNumber INT = 1,
    @PageSize INT = 20,

    @SortKey NVARCHAR(100) = NULL,
    @SortDirection NVARCHAR(100) = NULL,

    @FilterJson NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    exec dbo.GetBlazorGridData 
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @SortKey = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson = @FilterJson,

        @TableName = N'mdm.dbo.TransportRateSnapshot',
        @AllowedColumnsJson = N'[
            {"ColumnName": "StartDate", "ColumnType": "DATE"},
            {"ColumnName": "EndDate", "ColumnType": "DATE"},
            {"ColumnName": "CreationDate", "ColumnType": "DATE"},
            {"ColumnName": "LastChangeDate", "ColumnType": "DATE"},
            {"ColumnName": "NodeFromNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RateTypeName", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProductGroupName", "ColumnType": "NVARCHAR"},
            {"ColumnName": "IsArchive", "ColumnType": "BIT"},
            {"ColumnName": "IsDefRate", "ColumnType": "BIT"}
        ]',
        @SelectList = '
        SELECT
            [Id],
            [IsArchive],
            [Code],
            [IsDefRate],
            CAST([StartDate] AS DATE) StartDate,
            CAST([EndDate] AS DATE) EndDate,
            [CreationDate],
            [LastChangeDate],
            [TotalCostTon],
            [TotalCostTransport],
            [RateTypeCode],
            [RateTypeName],
            [NodeFromCode],
            [NodeFromNameEn],
            [NodeFromNameRu],
            [ProxyNodeCode],
            [ProxyNodeNameEn],
            [ProxyNodeNameRu],
            [NodeToCode],
            [NodeToNameEn],
            [NodeToNameRu],
            [TransportKindCode],
            [TransportKindNameRu],
            [TransportTypeCode],
            [TransportTypeNameRu],
            [ProductGroupCode],
            [ProductGroupName],
            [ContractorCode],
            [ContractorEGRUL],
            [CurrencyCode],
            [CurrencyName]'

        
END
GO
