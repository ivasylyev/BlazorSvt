USE mdm
GO

/*
Example:

use mdm
go

-- простой поиск (это когда или нет сортировки, или менее 2х полнотекстовых фильтров)

exec v2.GetTransportLegs 
    @PageNumber=1,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=NULL,
    @SortDirection=NULL,
    @FilterJson=N'[
    {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"казань","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProxyNodeNameRu","Value":"находка","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"IsArchive","Value":"False","Operator":1,"StringComparison":5}
        ]'


-- сложный поиск (это когда сортировка с двумя и более полнотекстовыми фильтрами)

exec v2.GetTransportLegs 
    @PageNumber=2,
    @PageSize=10,
    @Lang=N'ru',
    @SortKey=N'CreationDate',
    @SortDirection=N'ASC',
    @FilterJson=N'[
        {"PropertType":null,"PropertyName":"TransportKindIdRu","Value":"543763","Operator":1,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"нижн","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProxyNodeNameRu","Value":"нах","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"IsArchive","Value":"False","Operator":1,"StringComparison":5}
        ]'

*/
CREATE OR ALTER PROCEDURE v2.GetTransportLegs
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

    SET @LangSuffix = CASE 
                        WHEN @Lang = N'ru' THEN N'Ru' 
                        ELSE N'En' 
                      END

    SET @AllowedColumnsJson = N'[
            {"ColumnName": "Code", "ColumnType": "NVARCHAR"},

            {"ColumnName": "IsArchive", "ColumnType": "BIT"},
            {"ColumnName": "CanBeUsed", "ColumnType": "BIT"},

            {"ColumnName": "ShipmentTypeId", "ColumnType": "ID"},

            {"ColumnName": "TransportKindId", "ColumnType": "ID"},

            {"ColumnName": "NodeFromCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeFromNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionFromCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionFromNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionFromNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "ProxyNodeCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyNodeNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyRegionCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyRegionNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "ProxyRegionNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "NodeToCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "NodeToNameEn", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionToCode", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionToNameRu", "ColumnType": "NVARCHAR"},
            {"ColumnName": "RegionToNameEn", "ColumnType": "NVARCHAR"},

            {"ColumnName": "CreationDate", "ColumnType": "DATE"},
            {"ColumnName": "LastChangeDate", "ColumnType": "DATE"}
        ]'


    SET @SelectList = N'
        SELECT
            Id,
            LegId,
            Code,

            IsArchive,
            CanBeUsed,

            ShipmentTypeId AS ShipmentTypeIdRu, 
            ShipmentTypeId AS ShipmentTypeIdEn, 

            TransportKindId AS TransportKindIdRu, 
            TransportKindId AS TransportKindIdEn, 

            TransportKindCode,

            SearchTimeT,         
            LoadTimeT,           
            TravelTimeT,         
            DaysWaitingT,        
            UnLoadTimeT,         
            TransportationTimeT, 

            NodeFromCode,       
            NodeFromNameEn,     
            NodeFromNameRu,     
            RegionFromCode,     
            RegionFromNameEn,   
            RegionFromNameRu,   
    
            ProxyNodeCode,      
            ProxyNodeNameEn,    
            ProxyNodeNameRu,    
            ProxyRegionCode,    
            ProxyRegionNameEn,  
            ProxyRegionNameRu,  
    
            NodeToCode,         
            NodeToNameEn,       
            NodeToNameRu,       
            RegionToCode,       
            RegionToNameEn,     
            RegionToNameRu,     
    
            CreationDate,       
            LastChangeDate
  '


    EXEC v2.GetBlazorGridData 
        @PageNumber = @PageNumber,
        @PageSize = @PageSize,
        @LangSuffix = @LangSuffix,

        @SortKey = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson = @FilterJson,

        @TableName = N'mdm.v2.TransportLegSnapshot',
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList = @SelectList
        
        
END
GO
