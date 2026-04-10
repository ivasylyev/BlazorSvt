USE mdm
GO

/*
Example:

USE mdm
GO

EXEC dbo.GetBlazorGridData 
     @PageNumber=1,
     @PageSize=10,
     @LangSuffix = 'Ru',
     @TableName = N'mdm.dbo.TransportRateSnapshot',
     @AllowedColumnsJson = N'[
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
            {"ColumnName": "CreationDate", "ColumnType": "DATE"},
            {"ColumnName": "LastChangeDate", "ColumnType": "DATE"},
            {"ColumnName": "StartDate", "ColumnType": "DATE"},
            {"ColumnName": "EndDate", "ColumnType": "DATE"},
            {"ColumnName": "IsArchive", "ColumnType": "BIT"},
            {"ColumnName": "IsDefRate", "ColumnType": "BIT"}
        ]',
     @SelectList = '
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
            [ProductNameEn]',
    @SortKey=N'NodeToNameRu',
    @SortDirection=N'ASC',
    @FilterJson=N'[
        {"PropertType":null,"PropertyName":"RateTypeIdRu","Value":"543746","Operator":1,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"казань","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProxyNodeNameRu","Value":"находка","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"IsArchive","Value":"False","Operator":1,"StringComparison":5}
        ]'

*/
CREATE OR ALTER PROCEDURE dbo.GetBlazorGridData
    @PageNumber         INT = 1,
    @PageSize           INT = 20,
    @LangSuffix         NVARCHAR(2),
    @TableName          NVARCHAR(300),           -- Полное имя таблицы, например 'mdm.dbo.TransportRateSnapshot'
    @AllowedColumnsJson NVARCHAR(MAX), 
    @SelectList         NVARCHAR(MAX),          -- Список колонок для SELECT, например 'Id, Code, StartDate'

    @SortKey            NVARCHAR(50) = NULL,      -- Колонка для сортировки
    @SortDirection      NVARCHAR(5) = NULL,-- ASC или DESC

    @FilterJson         NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AllowedColumns TABLE
    (
        ColumnName NVARCHAR(100) NOT NULL,
        ColumnType NVARCHAR(100) NOT NULL
    )
    DECLARE @FilteredColumns TABLE
    (
        ColumnName  NVARCHAR(100) NOT NULL,
        ColumnType  NVARCHAR(100) NOT NULL,
        ColumnValue NVARCHAR(100) NULL,
        Operator    NVARCHAR(100) NULL
    )
    DECLARE
        @Offset INT = (@PageNumber - 1) * @PageSize,
        @JoinClause NVARCHAR(MAX) = '',
        @WhereClause NVARCHAR(MAX) = '',
        @MainSQL NVARCHAR(MAX),
        @TotalCountSQL NVARCHAR(MAX),
        @BothSQL NVARCHAR(MAX);

    SET @PageNumber = IIF(@PageNumber < 1, 1, @PageNumber);
    SET @PageSize = IIF(@PageSize < 1, 20, @PageSize);

    INSERT INTO @AllowedColumns (ColumnName, ColumnType)
    SELECT ColumnName, ColumnType
    FROM OPENJSON(@AllowedColumnsJson)
    WITH (
        ColumnName NVARCHAR(100) '$.ColumnName',
        ColumnType NVARCHAR(100) '$.ColumnType'
    );

    
    INSERT INTO @FilteredColumns (ColumnName, ColumnType, ColumnValue, Operator)
    SELECT 
        AC.ColumnName,
        AC.ColumnType,
        ISNULL(LTRIM(RTRIM(JS.[Value])), '') AS ColumnValue,
        JS.Operator
    FROM OPENJSON(@FilterJson)
    WITH (
        PropertType      INT            '$.PropertType',
        PropertyName     NVARCHAR(100)  '$.PropertyName',
        [Value]          NVARCHAR(MAX)  '$.Value',
        Operator         INT            '$.Operator',
        StringComparison INT            '$.StringComparison'
    ) JS
    JOIN @AllowedColumns AC
        ON JS.PropertyName = AC.ColumnName AND AC.ColumnType <> 'ID'
        OR (JS.PropertyName = AC.ColumnName + @LangSuffix AND AC.ColumnType = 'ID')

     -- Логика фильтров для СТРОК c полнотекстовым поиском
     
    SELECT 
    @JoinClause = STRING_AGG(
        '
            SELECT [KEY]
            FROM CONTAINSTABLE(' + @TableName + ', ' + ColumnName + ', N''"' + ColumnValue + '*"'')
        ', 
        'INTERSECT' 
    )
    FROM @FilteredColumns
    WHERE ColumnType = 'NVARCHAR' AND LEN(ColumnValue) > 2;
    
    -- если нет полей для полнотекстового поиска, то @JoinClause в итоге останется NULL. Это так и задумано.
    SET @JoinClause = 'JOIN 
    (
    ' + @JoinClause + '
    ) X
    ON X.[KEY] = Id'

    SET @JoinClause = ISNULL(@JoinClause, '')

    
     -- Логика остальных фильтов 
    SELECT @WhereClause = STRING_AGG(
        CASE ColumnType         
            -- Логика для ID INT
            WHEN 'ID' THEN 
                N'
                    AND ' + ColumnName + ' =  ' + ColumnValue + ' '
            -- Логика для ДАТ
            WHEN 'DATE' THEN 
                CASE WHEN ISDATE(ColumnValue) = 1
                THEN
                    ISNULL(N'
                    AND  ' + ColumnName + ' ' + dbo.fn_GetDateSqlOperator(Operator) -- если функция вернет NULL, весь фильтр обнулится. И это правильное поведение
                    +''''+ ColumnValue + '''','')
                ELSE ''
                END

            -- Логика для ФЛАГОВ
            WHEN 'BIT' THEN 
                CASE WHEN ColumnValue = 'True' THEN
                    N'
                    AND ' + ColumnName + ' = 1' 
                ELSE  N'
                    AND ' + ColumnName + ' = 0' 
                END

            -- Логика по умолчанию для остальных типов
            ELSE ''
        END, 
        ''
    )
    FROM @FilteredColumns;
    SET @WhereClause = CONCAT('WHERE 1 = 1', @WhereClause)

    -- Основной SELECT с подставленными CTE и JOIN'ами

    SET @MainSQL = '
    DECLARE @TotalCount INT
    SELECT @TotalCount = COUNT(1)
    FROM 
    ' + @TableName + ' 
    ' + @JoinClause + ' 
    ' + @WhereClause + ';' + '

    ' + @SelectList + '
        FROM 
    ' + @TableName + '
    ' + @JoinClause + ' 
    ' + @WhereClause + '
    AND @TotalCount > 0
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection + ', ', '')  + '  Id 
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;

    SELECT @TotalCount AS TotalCount;
    ';

    -- Для отладки можно раскомментировать:
    PRINT @MainSQL;

    EXEC sp_executesql @MainSQL;
    

END
GO
