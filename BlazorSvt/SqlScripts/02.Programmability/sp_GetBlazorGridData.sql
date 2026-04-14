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

    --waitfor delay '00:00:20' -- debug

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
        Operator    NVARCHAR(100) NULL,
        FilterPart  NVARCHAR(MAX) NULL
    )
    DECLARE
        @Offset INT = (@PageNumber - 1) * @PageSize,
        @TempTableSQL NVARCHAR(MAX) = '',
        @JoinClause NVARCHAR(MAX) = '',
        @TotalCountWhereClause NVARCHAR(MAX) = '',
        @MainWhereClause NVARCHAR(MAX) = '',
        @MainSQL NVARCHAR(MAX),
        @TotalCountSQL NVARCHAR(MAX),
        @BothSQL NVARCHAR(MAX),
        @TotalCount INT,
        @HasRowsClause NVARCHAR(50) = '',
        @HavingRowsClause NVARCHAR(50) = '',
        @IsComplexQuery BIT,
        @FulltextFilterCount INT;

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
        ON JS.PropertyName = AC.ColumnName -- AND AC.ColumnType <> 'ID'
        OR (JS.PropertyName = AC.ColumnName + @LangSuffix AND AC.ColumnType = 'ID')

    UPDATE @FilteredColumns
    SET FilterPart = CASE ColumnType         
            -- Логика для СТРОК c полнотекстовым поиском 
            WHEN 'NVARCHAR' THEN 
                CASE WHEN LEN(ColumnValue) > 2 THEN
                    N'
                    AND CONTAINS(' + ColumnName + ', N''"' + ColumnValue + '*"'') '
                ELSE '' 
                END      
                
            -- Логика для ID INT
            WHEN 'ID' THEN 
                N'
                    AND ' + ColumnName + dbo.fn_GetDateSqlOperator(Operator) + ColumnValue + ' '
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
        END
    FROM @FilteredColumns;


    SELECT @FulltextFilterCount = COUNT(1) 
    FROM  @FilteredColumns
    WHERE ColumnType = 'NVARCHAR'

    -- Запрос считается сложным, если есть 2 и более полнотекстовых фильтров с сортировкой.
    -- В этом случае планировщик сходит с ума. Приходится идти на дорогую операцию:создание временной таблицы для фиксации промежуточных результатов.
    SET @IsComplexQuery = CASE 
                            WHEN  (@FulltextFilterCount > 1) AND (ISNULL(@SortKey, '' ) <> '') THEN 1
                            ELSE 0
                          END
        
    -- Логика для СТРОК c полнотекстовым поиском включается для получения общего числа строк, подпадающих под критерий
    SELECT @TotalCountWhereClause = STRING_AGG(FilterPart, '')
    FROM @FilteredColumns;

    SET @TotalCountWhereClause = CONCAT('WHERE 1 = 1', @TotalCountWhereClause)

    -- Логика для СТРОК c полнотекстовым поиском включается только для простых запросов. 
    -- Для сложных запросов полнотекстовый поиск исключается из условия Where и включается в специаьно созданную для результатов поиска промежуточную таблицу
    SELECT @MainWhereClause = STRING_AGG(FilterPart, '')
    FROM @FilteredColumns
    WHERE (ColumnType <> 'NVARCHAR' OR @IsComplexQuery = 0)

    SET @MainWhereClause = CONCAT('WHERE 1 = 1', @MainWhereClause)

  
     -- Логика фильтров для СТРОК c полнотекстовым поиском для сложных условий
    IF (@IsComplexQuery = 1)  
    BEGIN

        WITH CTE AS
        (
            SELECT 
                ColumnName,
                ColumnValue,
                RN = ROW_NUMBER() OVER (ORDER BY ColumnName)
            FROM @FilteredColumns
            WHERE ColumnType = 'NVARCHAR' 
                  AND ColumnValue IS NOT NULL
                  AND LEN(ColumnValue) > 2
        )

        SELECT @TempTableSQL = STRING_AGG(Batch, '')
        FROM
        (
            SELECT 
                RN,
                Batch =
                    CASE 
                        WHEN RN = 1 THEN -- первове условие используется как основа для соединения с остальными
                            'FROM CONTAINSTABLE(' + @TableName + ', ' + ColumnName + ', N''"' + ColumnValue + '*"'') AS T1'
                        ELSE
                            '
                            JOIN CONTAINSTABLE(' + @TableName + ', ' + ColumnName + ', N''"' + ColumnValue + '*"'') AS T' + CAST(RN AS NVARCHAR) + '
                              ON T' + CAST(RN AS NVARCHAR) + '.[KEY] = T1.[KEY]'
                    END
            FROM CTE
        ) q;

        SET @TempTableSQL = 
        '        
        DROP TABLE IF EXISTS #FTSResult;
        CREATE TABLE #FTSResult
        (
            [KEY] INT PRIMARY KEY
        );
        
        INSERT INTO #FTSResult([KEY])
        SELECT T1.[KEY]
        ' +  @TempTableSQL +'
        GROUP BY T1.[KEY]
        ';  

        SET @JoinClause = 
            '
            JOIN #FTSResult f ON f.[KEY] = Id
            '
    END

    SET @TempTableSQL = ISNULL(@TempTableSQL, '')   
    SET @JoinClause = ISNULL(@JoinClause, '')

   
    -- Логика для СТРОК c полнотекстовым поиском включается только для простых запросов
    SELECT @TotalCountWhereClause = STRING_AGG(FilterPart, '')
    FROM @FilteredColumns;

    SET @TotalCountWhereClause = CONCAT('WHERE 1 = 1', @TotalCountWhereClause)

    SET @TotalCountSQL = '
        SELECT @TotalCount_OUT = COUNT(1)
        FROM ' + @TableName + '
        ' + @TotalCountWhereClause + ';';

    -- Для отладки  раскомментировать:
    PRINT @TotalCountSQL;

    EXEC sp_executesql
        @TotalCountSQL,
        N'@TotalCount_OUT INT OUTPUT',
        @TotalCount_OUT = @TotalCount OUTPUT;

    SET @HasRowsClause = CASE 
            WHEN @TotalCount > 0 THEN ' AND 1 = 1' 
            ELSE ' AND 0 = 1' END;
    IF (@IsComplexQuery = 1)  
    BEGIN
        SET @HavingRowsClause = CASE 
                WHEN @TotalCount > 0 THEN ' HAVING 1 = 1' 
                ELSE ' HAVING 0 = 1' END;
    END

    SET @MainSQL = ' 
    ' + @TempTableSQL + '
    ' + @HavingRowsClause + ';
    ' + @SelectList + '
        FROM 
    ' + @TableName + '
    ' + @JoinClause + ' 
    ' + @MainWhereClause + '
    ' + @HasRowsClause + '
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection + ', ', '')  + '  Id 
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;
    ';

    -- Для отладки  раскомментировать:
    PRINT @MainSQL;

    EXEC sp_executesql @MainSQL;

    SELECT @TotalCount AS TotalCount
    

END
GO
