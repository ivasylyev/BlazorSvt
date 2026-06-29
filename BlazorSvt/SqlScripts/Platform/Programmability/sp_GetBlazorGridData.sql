USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.GetBlazorGridData 
     @PageNumber=1,
     @PageSize=10,
     @LangSuffix = 'Ru',
     @TableName = N'mdm.v2.TransportRateSnapshot',
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
        {"PropertyName":"RateTypeIdRu","Value":"543746","Operator":"Equals"},
        {"PropertyName":"NodeFromNameRu","Value":"казань","Operator":"Contains"},
        {"PropertyName":"ProxyNodeNameRu","Value":"находка","Operator":"Contains"},
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
        ]'

*/
CREATE OR ALTER PROCEDURE v2.GetBlazorGridData
    @PageNumber         INT = 1,
    @PageSize           INT = 20,
    @LangSuffix         NVARCHAR(2),
    @TableName          NVARCHAR(300),           -- Полное имя таблицы, например 'mdm.v2.TransportRateSnapshot'
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
        ColumnName SYSNAME NOT NULL,
        ColumnType NVARCHAR(20) NOT NULL
    )
    DECLARE @FilteredColumns TABLE
    (
        ColumnName    SYSNAME NOT NULL,
        SqlColumnName NVARCHAR(258) NOT NULL,
        ColumnType    NVARCHAR(20) NOT NULL,
        ColumnValue   NVARCHAR(4000) NULL,
        Operator      NVARCHAR(50) NULL,
        SqlOperator   NVARCHAR(2) NULL,
        SqlValue      NVARCHAR(4000) NULL,
        FtsValue      NVARCHAR(4000) NULL,
        FilterPart    NVARCHAR(MAX) NULL
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
        @FulltextFilterCount INT,
        @DatabaseName SYSNAME,
        @SchemaName SYSNAME,
        @ObjectName SYSNAME,
        @SafeTableName NVARCHAR(776),
        @TableObjectId INT,
        @SortColumn SYSNAME,
        @OrderByClause NVARCHAR(300) = N'ORDER BY [Id]';

    SET @PageNumber = IIF(@PageNumber < 1, 1, @PageNumber);
    SET @PageSize = IIF(@PageSize < 1, 20, @PageSize);
    SET @LangSuffix = CASE WHEN @LangSuffix = N'Ru' THEN N'Ru' ELSE N'En' END;

    SET @DatabaseName = PARSENAME(@TableName, 3);
    SET @SchemaName = PARSENAME(@TableName, 2);
    SET @ObjectName = PARSENAME(@TableName, 1);

    IF PARSENAME(@TableName, 4) IS NOT NULL OR @SchemaName IS NULL OR @ObjectName IS NULL
        THROW 50000, 'Invalid table name.', 1;

    IF @DatabaseName IS NOT NULL AND @DatabaseName <> DB_NAME()
        THROW 50000, 'Table name must refer to the current database.', 1;

    SET @SafeTableName = CASE
        WHEN @DatabaseName IS NULL THEN QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@ObjectName)
        ELSE QUOTENAME(@DatabaseName) + N'.' + QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@ObjectName)
    END;

    SET @TableObjectId = OBJECT_ID(@SafeTableName, 'U');

    IF @TableObjectId IS NULL
        THROW 50000, 'Table does not exist.', 1;

    INSERT INTO @AllowedColumns (ColumnName, ColumnType)
    SELECT ColumnName, UPPER(ColumnType)
    FROM OPENJSON(@AllowedColumnsJson)
    WITH (
        ColumnName SYSNAME '$.ColumnName',
        ColumnType NVARCHAR(20) '$.ColumnType'
    )
    WHERE ColumnName IS NOT NULL
      AND UPPER(ColumnType) IN (N'NVARCHAR', N'ID', N'DATE', N'BIT');

    SET @SortKey = NULLIF(LTRIM(RTRIM(@SortKey)), N'');
    SET @SortDirection = UPPER(NULLIF(LTRIM(RTRIM(@SortDirection)), N''));

    IF @SortKey IS NOT NULL
    BEGIN
        SET @SortDirection = ISNULL(@SortDirection, N'ASC');

        IF @SortDirection NOT IN (N'ASC', N'DESC')
            THROW 50000, 'Invalid sort direction.', 1;

        SELECT TOP (1)
            @SortColumn = CASE
                WHEN AC.ColumnType = N'ID' AND @SortKey = AC.ColumnName + @LangSuffix THEN AC.ColumnName
                ELSE @SortKey
            END
        FROM @AllowedColumns AC
        WHERE AC.ColumnName = @SortKey
           OR (AC.ColumnType = N'ID' AND @SortKey = AC.ColumnName + @LangSuffix);

        IF @SortColumn IS NULL
           AND EXISTS (
                SELECT 1
                FROM sys.columns
                WHERE object_id = @TableObjectId
                  AND name = @SortKey
           )
        BEGIN
            SET @SortColumn = @SortKey;
        END

        IF @SortColumn IS NULL
           OR NOT EXISTS (
                SELECT 1
                FROM sys.columns
                WHERE object_id = @TableObjectId
                  AND name = @SortColumn
           )
        BEGIN
            THROW 50000, 'Invalid sort column.', 1;
        END

        SET @OrderByClause = N'ORDER BY ' + QUOTENAME(@SortColumn) + N' ' + @SortDirection + N', [Id]';
    END;

    
    INSERT INTO @FilteredColumns (ColumnName, SqlColumnName, ColumnType, ColumnValue, Operator)
    SELECT 
        AC.ColumnName,
        QUOTENAME(AC.ColumnName),
        AC.ColumnType,
        ISNULL(LTRIM(RTRIM(JS.[Value])), N'') AS ColumnValue,
        JS.Operator
    FROM OPENJSON(@FilterJson)
    WITH (
        PropertyName     NVARCHAR(100)  '$.PropertyName',
        [Value]          NVARCHAR(MAX)  '$.Value',
        Operator         NVARCHAR(50)   '$.Operator'
    ) JS
    JOIN @AllowedColumns AC
        ON JS.PropertyName = AC.ColumnName -- AND AC.ColumnType <> 'ID'
        OR (JS.PropertyName = AC.ColumnName + @LangSuffix AND AC.ColumnType = 'ID')

    UPDATE @FilteredColumns
    SET SqlOperator = v2.fn_GetDateSqlOperator(Operator),
        SqlValue = CASE ColumnType
            WHEN N'ID' THEN CONVERT(NVARCHAR(30), TRY_CONVERT(BIGINT, ColumnValue))
            WHEN N'DATE' THEN CONVERT(NVARCHAR(10), TRY_CONVERT(DATE, ColumnValue), 23)
            ELSE ColumnValue
        END,
        FtsValue = NULLIF(LTRIM(RTRIM(
            REPLACE(
                REPLACE(
                    REPLACE(
                        REPLACE(
                            REPLACE(
                                REPLACE(
                                    REPLACE(ColumnValue, N'''', N''''''),
                                N'"', N' '),
                            N'*', N' '),
                        N'(', N' '),
                    N')', N' '),
                N'&', N' '),
            N'|', N' ')
        )), N'');

    UPDATE @FilteredColumns
    SET FilterPart = CASE ColumnType         
            -- Логика для СТРОК c полнотекстовым поиском 
            WHEN N'NVARCHAR' THEN 
                CASE WHEN LEN(FtsValue) > 2 THEN
                    N'
                    AND CONTAINS(' + SqlColumnName + ', N''"' + FtsValue + '*"'') '
                ELSE '' 
                END      
                
            -- Логика для ID INT
            WHEN N'ID' THEN
                CASE WHEN SqlOperator IS NOT NULL AND SqlValue IS NOT NULL THEN
                    N'
                    AND ' + SqlColumnName + N' ' + SqlOperator + N' ' + SqlValue + N' '
                ELSE ''
                END
            -- Логика для ДАТ
            WHEN N'DATE' THEN 
                CASE WHEN SqlOperator IS NOT NULL AND SqlValue IS NOT NULL THEN
                    N'
                    AND ' + SqlColumnName + N' ' + SqlOperator + N' ''' + SqlValue + N''' '
                ELSE ''
                END

            -- Логика для ФЛАГОВ
            WHEN N'BIT' THEN 
                CASE WHEN ColumnValue IN (N'True', N'true', N'1') THEN
                    N'
                    AND ' + SqlColumnName + N' = 1'
                WHEN ColumnValue IN (N'False', N'false', N'0') THEN
                    N'
                    AND ' + SqlColumnName + N' = 0'
                ELSE ''
                END

            -- Логика по умолчанию для остальных типов
            ELSE ''
        END
    FROM @FilteredColumns;


    SELECT @FulltextFilterCount = COUNT(1) 
    FROM  @FilteredColumns
    WHERE ColumnType = N'NVARCHAR'
      AND LEN(FtsValue) > 2

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
                SqlColumnName,
                FtsValue,
                RN = ROW_NUMBER() OVER (ORDER BY ColumnName)
            FROM @FilteredColumns
            WHERE ColumnType = N'NVARCHAR' 
                  AND FtsValue IS NOT NULL
                  AND LEN(FtsValue) > 2
        )

        SELECT @TempTableSQL = STRING_AGG(Batch, '')
        FROM
        (
            SELECT 
                RN,
                Batch =
                    CASE 
                        WHEN RN = 1 THEN -- первове условие используется как основа для соединения с остальными
                            'FROM CONTAINSTABLE(' + @SafeTableName + ', ' + SqlColumnName + ', N''"' + FtsValue + '*"'') AS T1'
                        ELSE
                            '
                            JOIN CONTAINSTABLE(' + @SafeTableName + ', ' + SqlColumnName + ', N''"' + FtsValue + '*"'') AS T' + CAST(RN AS NVARCHAR) + '
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
        FROM ' + @SafeTableName + '
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
    ' + @SafeTableName + '
    ' + @JoinClause + ' 
    ' + @MainWhereClause + '
    ' + @HasRowsClause + '
    ' + @OrderByClause + '
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;
    ';

    -- Для отладки  раскомментировать:
    PRINT @MainSQL;

    EXEC sp_executesql @MainSQL;

    SELECT @TotalCount AS TotalCount
    

END
GO
