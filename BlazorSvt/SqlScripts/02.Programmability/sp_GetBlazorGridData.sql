USE mdm
GO

/*
Example:

 exec dbo.GetBlazorGridData 
     @PageNumber=1,
     @PageSize=10,
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
     @SortKey=N'NodeToNameRu',
     @SortDirection=N'ASC',
     @FilterJson=N'[{"PropertType":null,"PropertyName":"Code","Value":"2763157","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"NodeFromNameRu","Value":"каз","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"ProductGroupName","Value":"полиоле","Operator":7,"StringComparison":5},
        {"PropertType":null,"PropertyName":"StartDate","Value":"2026-02-28","Operator":4,"StringComparison":5}]'

*/
CREATE OR ALTER PROCEDURE dbo.GetBlazorGridData
    @PageNumber         INT = 1,
    @PageSize           INT = 20,
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
        @CTEs NVARCHAR(MAX) = '',
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
        Value            NVARCHAR(MAX)  '$.Value',
        Operator         INT            '$.Operator',
        StringComparison INT            '$.StringComparison'
    ) JS
    JOIN @AllowedColumns AC
    ON JS.PropertyName = AC.ColumnName

     -- Логика фильтров для СТРОК c полнотекстовым поиском
    SELECT 
    @CTEs = STRING_AGG(
        '
        ,FTE_' + ColumnName + ' AS (
            SELECT [KEY]
            FROM CONTAINSTABLE(' + @TableName + ', ' + ColumnName + ', N''"' + ColumnValue + '*"'')
        )', '' 
    ),
    @JoinClause = STRING_AGG(
        '
        JOIN FTE_' + ColumnName + ' ON FTE_' + ColumnName + '.[KEY] = Id', '' 
    )
    FROM @FilteredColumns
    WHERE ColumnType = 'NVARCHAR' AND LEN(ColumnValue) > 2;

    SET @CTEs = ISNULL(@CTEs, '')   
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
    WITH CTE AS (SELECT 1 AS TST)
    ' + @CTEs + '
    SELECT @TotalCount = COUNT(1)
    FROM 
    ' + @TableName + ' 
    ' + @JoinClause + ' 
    ' + @WhereClause + ';' + '

    WITH CTE AS (SELECT 1 AS TST)
    ' + @CTEs + '
    ' + @SelectList + '
        FROM 
    ' + @TableName + '
    ' + @JoinClause + ' 
    ' + @WhereClause + '
    AND @TotalCount > 0
    ORDER BY '+ ISNULL(@SortKey + ' ' + @SortDirection + ', ', '')  + '  Id DESC
    OFFSET ' + CAST(@Offset AS NVARCHAR(20)) + ' ROWS
    FETCH NEXT ' + CAST(@PageSize AS NVARCHAR(20)) + ' ROWS ONLY;

    SELECT @TotalCount AS TotalCount;
    ';

    -- Для отладки можно раскомментировать:
    PRINT @MainSQL;

    EXEC sp_executesql @MainSQL;
    

END
GO
