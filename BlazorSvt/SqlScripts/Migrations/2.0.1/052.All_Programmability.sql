/*
  Auto-generated: All v2 Programmability
  Generated: 2026-08-26 12:16:36
  Scripts: 24
  Order: Create-Programmability.ps1 (Platform -> Modules by name)
*/
GO

--------------------------------------------------------------------------------
-- [1/24] Platform :: fn_GetDateSqlOperator.sql
-- Source: Platform\Programmability\fn_GetDateSqlOperator.sql
--------------------------------------------------------------------------------
GO
USE [mdm]
GO



CREATE OR ALTER FUNCTION v2.fn_GetDateSqlOperator (@OperatorName NVARCHAR(50))
RETURNS NVARCHAR(2)
AS
BEGIN
    RETURN CASE UPPER(@OperatorName)
        WHEN 'EQUALS' THEN '='
        WHEN '1' THEN '='
        WHEN 'NOTEQUALS' THEN '<>'
        WHEN '2' THEN '<>'
        WHEN 'LESSTHAN' THEN '<'
        WHEN '3' THEN '<'
        WHEN 'LESSTHANOREQUALS' THEN '<='
        WHEN '4' THEN '<='
        WHEN 'GREATERTHAN' THEN '>'
        WHEN '5' THEN '>'
        WHEN 'GREATERTHANOREQUALS' THEN '>='
        WHEN '6' THEN '>='
        ELSE NULL
    END
END
GO
GO

--------------------------------------------------------------------------------
-- [2/24] Platform :: sp_ExportBlazorGridDetail.sql
-- Source: Platform\Programmability\sp_ExportBlazorGridDetail.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Example:

USE mdm;
EXEC v2.ExportBlazorGridDetail
    @PageNumber = 1,
    @PageSize = 100,
    @TableName = N'v2.TransportRate_Snapshot',
    @AllowedColumnsJson = N'[{"ColumnName":"IsArchive","SqlColumnName":"IsArchive","ColumnType":"BIT"}]',
    @SelectList = N'
        SELECT
            TransportRateId
  ',
    @DetailViewName = N'v2.vw_TransportRate_Detail',
    @EntityKeyColumn = N'TransportRateId',
    @SortKey = N'StartDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
    ]'

*/
CREATE OR ALTER PROCEDURE v2.ExportBlazorGridDetail
    @PageNumber             INT = 1,
    @PageSize               INT,
    @TableName              NVARCHAR(300),
    @AllowedColumnsJson     NVARCHAR(MAX),
    @SelectList             NVARCHAR(MAX),
    @DetailViewName         NVARCHAR(300),
    @EntityKeyColumn        SYSNAME,
    @SortKey                NVARCHAR(50) = NULL,
    @SortDirection          NVARCHAR(5) = NULL,
    @FilterJson             NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE
        @DatabaseName SYSNAME,
        @SchemaName SYSNAME,
        @ObjectName SYSNAME,
        @SafeViewName NVARCHAR(776),
        @ViewObjectId INT,
        @SafeKeyColumn NVARCHAR(258),
        @DetailSQL NVARCHAR(MAX);

    SET @EntityKeyColumn = NULLIF(LTRIM(RTRIM(@EntityKeyColumn)), N'');

    IF @EntityKeyColumn IS NULL
        THROW 50000, 'Entity key column is required.', 1;

    SET @DatabaseName = PARSENAME(@DetailViewName, 3);
    SET @SchemaName = PARSENAME(@DetailViewName, 2);
    SET @ObjectName = PARSENAME(@DetailViewName, 1);

    IF PARSENAME(@DetailViewName, 4) IS NOT NULL OR @SchemaName IS NULL OR @ObjectName IS NULL
        THROW 50000, 'Invalid detail view name.', 1;

    IF @DatabaseName IS NOT NULL AND @DatabaseName <> DB_NAME()
        THROW 50000, 'Detail view name must refer to the current database.', 1;

    SET @SafeViewName = CASE
        WHEN @DatabaseName IS NULL THEN QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@ObjectName)
        ELSE QUOTENAME(@DatabaseName) + N'.' + QUOTENAME(@SchemaName) + N'.' + QUOTENAME(@ObjectName)
    END;

    SET @ViewObjectId = OBJECT_ID(@SafeViewName, 'V');

    IF @ViewObjectId IS NULL
        THROW 50000, 'Detail view does not exist.', 1;

    IF NOT EXISTS (
        SELECT 1
        FROM sys.columns
        WHERE object_id = @ViewObjectId
          AND name = @EntityKeyColumn
    )
        THROW 50000, 'Invalid entity key column.', 1;

    SET @SafeKeyColumn = QUOTENAME(@EntityKeyColumn);

    CREATE TABLE #Filtered (
        RowNum   INT IDENTITY(1,1) NOT NULL,
        EntityId BIGINT NOT NULL,
        PRIMARY KEY (EntityId)
    );

    INSERT INTO #Filtered (EntityId)
    EXEC v2.GetBlazorGridData
        @PageNumber         = @PageNumber,
        @PageSize           = @PageSize,
        @TableName          = @TableName,
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList         = @SelectList,
        @SortKey            = @SortKey,
        @SortDirection      = @SortDirection,
        @FilterJson         = @FilterJson;
-- ������ ������ ��������� ������ "ORDER BY" �� "INNER JOIN #Filtered"
-- ���� �������� - ��� ��������� �� ������ ������
    SET @DetailSQL = N'
        SELECT d.*
        FROM ' + @SafeViewName + N' d
        WHERE d.' + @SafeKeyColumn + N' IN (SELECT EntityId FROM #Filtered)
        ORDER BY (
            SELECT f.RowNum
            FROM #Filtered f
            WHERE f.EntityId = d.' + @SafeKeyColumn + N'
        );';

    EXEC sp_executesql @DetailSQL;
END
GO
GO

--------------------------------------------------------------------------------
-- [3/24] Platform :: sp_GetBlazorGridData.sql
-- Source: Platform\Programmability\sp_GetBlazorGridData.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Example:

USE mdm;
EXEC v2.GetBlazorGridData 
     @PageNumber=1,
     @PageSize=10,
     @LangSuffix = 'Ru',
     @TableName = N'mdm.v2.TransportRate_Snapshot',
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
    @TableName          NVARCHAR(300),
    @AllowedColumnsJson NVARCHAR(MAX),
    @SelectList         NVARCHAR(MAX),

    @SortKey            NVARCHAR(50) = NULL,
    @SortDirection      NVARCHAR(5) = NULL,

    @FilterJson         NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @AllowedColumns TABLE
    (
        ColumnName    SYSNAME NOT NULL,
        SqlColumnName SYSNAME NOT NULL,
        ColumnType    NVARCHAR(20) NOT NULL
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

    INSERT INTO @AllowedColumns (ColumnName, SqlColumnName, ColumnType)
    SELECT
        ColumnName,
        ISNULL(SqlColumnName, ColumnName),
        UPPER(ColumnType)
    FROM OPENJSON(@AllowedColumnsJson)
    WITH (
        ColumnName    SYSNAME      '$.ColumnName',
        SqlColumnName SYSNAME      '$.SqlColumnName',
        ColumnType    NVARCHAR(20) '$.ColumnType'
    )
    WHERE ColumnName IS NOT NULL
      AND UPPER(ColumnType) IN (N'NVARCHAR', N'ID', N'DATE', N'BIT', N'DECIMAL');

    SET @SortKey = NULLIF(LTRIM(RTRIM(@SortKey)), N'');
    SET @SortDirection = UPPER(NULLIF(LTRIM(RTRIM(@SortDirection)), N''));

    IF @SortKey IS NOT NULL
    BEGIN
        SET @SortDirection = ISNULL(@SortDirection, N'ASC');

        IF @SortDirection NOT IN (N'ASC', N'DESC')
        BEGIN
            ;THROW 50000, 'Invalid sort direction.', 1;
        END

        SELECT TOP (1)
            @SortColumn = AC.SqlColumnName
        FROM @AllowedColumns AC
        WHERE AC.ColumnName = @SortKey;

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
            ;THROW 50000, 'Invalid sort column.', 1;
        END

        SET @OrderByClause = N'ORDER BY ' + QUOTENAME(@SortColumn) + N' ' + @SortDirection + N', [Id]';
    END;

    
    INSERT INTO @FilteredColumns (ColumnName, SqlColumnName, ColumnType, ColumnValue, Operator)
    SELECT
        AC.ColumnName,
        QUOTENAME(AC.SqlColumnName),
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
        ON JS.PropertyName = AC.ColumnName

    UPDATE @FilteredColumns
    SET SqlOperator = v2.fn_GetDateSqlOperator(Operator),
        SqlValue = CASE ColumnType
            WHEN N'ID' THEN CONVERT(NVARCHAR(30), TRY_CONVERT(BIGINT, ColumnValue))
            WHEN N'DECIMAL' THEN CONVERT(NVARCHAR(50), TRY_CONVERT(DECIMAL(38, 10), ColumnValue))
            -- DATETIME range (1753-01-01..9999-12-31): reject intermediate type=date values (e.g. 0002-09-30)
            WHEN N'DATE' THEN
                CASE
                    WHEN TRY_CONVERT(DATETIME, ColumnValue) IS NOT NULL
                        THEN CONVERT(NVARCHAR(10), CONVERT(DATE, TRY_CONVERT(DATETIME, ColumnValue)), 23)
                    ELSE NULL
                END
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

            -- Логика для DECIMAL
            WHEN N'DECIMAL' THEN
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
GO

--------------------------------------------------------------------------------
-- [4/24] Platform :: sp_Sync_GetHighWatermark.sql
-- Source: Platform\Programmability\sp_Sync_GetHighWatermark.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Наибольшая гарантированно закоммиченная версия строк (граница цикла @Hi).

    MIN_ACTIVE_ROWVERSION() возвращает наименьшую версию ещё открытой транзакции
    (или @@DBTS+1, если открытых нет). Всё, что строго меньше — уже закоммичено,
    поэтому @Hi = MIN_ACTIVE_ROWVERSION() - 1. Строки незавершённых транзакций
    исключаются -> нет пропусков и не нужен RCSI.

    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_GetHighWatermark
AS
BEGIN
    SET NOCOUNT ON;

    SELECT CONVERT(BINARY(8), CONVERT(BIGINT, MIN_ACTIVE_ROWVERSION()) - 1) AS Hi;
END
GO
GO

--------------------------------------------------------------------------------
-- [5/24] Platform :: sp_Sync_Reconcile.sql
-- Source: Platform\Programmability\sp_Sync_Reconcile.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Reconciliation: удаляет из snapshot "фантомы" — записи, физически удалённые
    в legacy (их нет в проекции). Ловит то, что инкремент по rowversion не видит
    (жёсткие delete). Generic по имени snapshot/проекции/ключа.

    Тяжёлый anti-join через проекцию (JOIN всех legacy-вью) — запускается редко
    (ночью, в blackout-окне инкремента), не в основном цикле.

    Возвращает число удалённых строк.
    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_Reconcile
    @SnapshotTable  NVARCHAR(300),
    @SourceView     NVARCHAR(300),
    @KeyColumn      SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    IF OBJECT_ID(@SnapshotTable) IS NULL
        THROW 50000, 'Snapshot table not found.', 1;
    IF OBJECT_ID(@SourceView) IS NULL
        THROW 50000, 'Source projection view not found.', 1;

    DECLARE @SafeSnap NVARCHAR(400) = QUOTENAME(PARSENAME(@SnapshotTable, 2)) + N'.' + QUOTENAME(PARSENAME(@SnapshotTable, 1));
    DECLARE @SafeView NVARCHAR(400) = QUOTENAME(PARSENAME(@SourceView, 2)) + N'.' + QUOTENAME(PARSENAME(@SourceView, 1));
    DECLARE @SafeKey  NVARCHAR(258) = QUOTENAME(@KeyColumn);

    DECLARE @Sql NVARCHAR(MAX) = N'
DELETE tgt
FROM ' + @SafeSnap + N' AS tgt
WHERE NOT EXISTS (
    SELECT 1 FROM ' + @SafeView + N' AS src
    WHERE src.' + @SafeKey + N' = tgt.' + @SafeKey + N');
SET @DeletedOut = @@ROWCOUNT;';

    DECLARE @Deleted INT;
    EXEC sp_executesql @Sql, N'@DeletedOut INT OUTPUT', @DeletedOut = @Deleted OUTPUT;

    SELECT @Deleted AS Deleted;
END
GO
GO

--------------------------------------------------------------------------------
-- [6/24] Platform :: sp_Sync_UpsertAffected.sql
-- Source: Platform\Programmability\sp_Sync_UpsertAffected.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

SET QUOTED_IDENTIFIER ON;
GO

SET ANSI_NULLS ON;
GO

/*
    Партиционно-безопасный upsert затронутых ключей из проекции в snapshot.
    Generic: набор колонок читается из sys.columns целевой таблицы (кроме
    суррогатного Id), поэтому одна процедура обслуживает любой справочник.

    Требует, чтобы вызывающая сессия предварительно создала и наполнила
    временную таблицу #AffectedKeys(EntityKey BIGINT) — динамический SQL
    выполняется в той же сессии и видит её.

    Snapshot партиционирован по IsArchive, PK = (IsArchive, Id). Обе операции
    ограничены #AffectedKeys -> идут по индексу бизнес-ключа, без скана таблицы:
      1. Pre-DELETE строк, у которых сменился IsArchive (переход между
         партициями): старая строка удаляется, чтобы MERGE вставил новую.
         Записи с неизменным IsArchive не трогаются -> их суррогатный Id
         сохраняется при UPDATE.
      2. MERGE по (ключ, IsArchive): UPDATE на месте либо INSERT новой/
         перенесённой строки. NOT MATCHED BY SOURCE сознательно НЕ используется
         (вызвал бы скан всей таблицы) — физические удаления чинит Sync_Reconcile.

    Возвращает число строк, затронутых MERGE.
    Вызывается из Platform/Sync/SnapshotSyncExecutor.
*/
CREATE OR ALTER PROCEDURE v2.Sync_UpsertAffected
    @SnapshotTable  NVARCHAR(300),
    @SourceView     NVARCHAR(300),
    @KeyColumn      SYSNAME
AS
BEGIN
    SET NOCOUNT ON;

    DECLARE @SnapObjId INT = OBJECT_ID(@SnapshotTable);

    IF @SnapObjId IS NULL
        THROW 50000, 'Snapshot table not found.', 1;
    IF OBJECT_ID(@SourceView) IS NULL
        THROW 50000, 'Source projection view not found.', 1;
    IF NOT EXISTS (SELECT 1 FROM sys.columns WHERE object_id = @SnapObjId AND name = @KeyColumn)
        THROW 50000, 'Key column not found in snapshot table.', 1;

    DECLARE @Archive SYSNAME = N'IsArchive';

    DECLARE @SafeSnap    NVARCHAR(400) = QUOTENAME(PARSENAME(@SnapshotTable, 2)) + N'.' + QUOTENAME(PARSENAME(@SnapshotTable, 1));
    DECLARE @SafeView    NVARCHAR(400) = QUOTENAME(PARSENAME(@SourceView, 2)) + N'.' + QUOTENAME(PARSENAME(@SourceView, 1));
    DECLARE @SafeKey     NVARCHAR(258) = QUOTENAME(@KeyColumn);
    DECLARE @SafeArchive NVARCHAR(258) = QUOTENAME(@Archive);

    DECLARE @InsertCols NVARCHAR(MAX), @InsertVals NVARCHAR(MAX), @UpdateSet NVARCHAR(MAX);

    -- Все физические колонки snapshot (в порядке определения), кроме суррогатного Id.
    SELECT @InsertCols = STUFF((
        SELECT N', ' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId AND c.name <> N'Id'
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    SELECT @InsertVals = STUFF((
        SELECT N', src.' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId AND c.name <> N'Id'
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    -- В UPDATE не трогаем бизнес-ключ и партиционную колонку IsArchive.
    SELECT @UpdateSet = STUFF((
        SELECT N', ' + QUOTENAME(c.name) + N' = src.' + QUOTENAME(c.name)
        FROM sys.columns c
        WHERE c.object_id = @SnapObjId
          AND c.name NOT IN (N'Id', @KeyColumn, @Archive)
        ORDER BY c.column_id
        FOR XML PATH(N''), TYPE).value(N'.', N'NVARCHAR(MAX)'), 1, 2, N'');

    DECLARE @Sql NVARCHAR(MAX) = N'
SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
DELETE tgt
FROM ' + @SafeSnap + N' AS tgt
WHERE tgt.' + @SafeKey + N' IN (SELECT EntityKey FROM #AffectedKeys)
  AND NOT EXISTS (
      SELECT 1 FROM ' + @SafeView + N' AS src
      WHERE src.' + @SafeKey + N' = tgt.' + @SafeKey + N'
        AND src.' + @SafeArchive + N' = tgt.' + @SafeArchive + N');

MERGE ' + @SafeSnap + N' WITH (HOLDLOCK) AS tgt
USING (
    SELECT src.* FROM ' + @SafeView + N' AS src
    WHERE src.' + @SafeKey + N' IN (SELECT EntityKey FROM #AffectedKeys)
) AS src
ON tgt.' + @SafeKey + N' = src.' + @SafeKey + N'
   AND tgt.' + @SafeArchive + N' = src.' + @SafeArchive + N'
WHEN MATCHED THEN UPDATE SET ' + @UpdateSet + N'
WHEN NOT MATCHED BY TARGET THEN INSERT (' + @InsertCols + N')
    VALUES (' + @InsertVals + N');
SET @AffectedOut = @@ROWCOUNT;';

    DECLARE @Affected INT;
    EXEC sp_executesql @Sql, N'@AffectedOut INT OUTPUT', @AffectedOut = @Affected OUTPUT;

    SELECT @Affected AS Affected;
END
GO
GO

--------------------------------------------------------------------------------
-- [7/24] Platform :: sp_SyncState_Get.sql
-- Source: Platform\Programmability\sp_SyncState_Get.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Возвращает курсор (LastRowVersion) источника или пусто, если строки ещё нет
    (справочник не проинициализирован — требуется первичная заливка/seed).

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_Get
    @Entity     NVARCHAR(100),
    @SourceName NVARCHAR(200)
AS
BEGIN
    SET NOCOUNT ON;

    SELECT LastRowVersion
    FROM v2.SyncState
    WHERE Entity = @Entity AND SourceName = @SourceName;
END
GO
GO

--------------------------------------------------------------------------------
-- [8/24] Platform :: sp_SyncState_MarkReconciled.sql
-- Source: Platform\Programmability\sp_SyncState_MarkReconciled.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Фиксирует время последней reconciliation по всем источникам справочника.
    Поле LastReconcileUtc — только для наблюдаемости (лаг/алерты).

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_MarkReconciled
    @Entity NVARCHAR(100)
AS
BEGIN
    SET NOCOUNT ON;

    UPDATE v2.SyncState
    SET LastReconcileUtc = SYSUTCDATETIME()
    WHERE Entity = @Entity;
END
GO
GO

--------------------------------------------------------------------------------
-- [9/24] Platform :: sp_SyncState_Upsert.sql
-- Source: Platform\Programmability\sp_SyncState_Upsert.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Создаёт/обновляет курсор источника после успешной обработки цикла.
    Продвижение идемпотентно: повтор с тем же @RowVersion безопасен.

    Вызывается из Platform/Sync/SyncStateStore.
*/
CREATE OR ALTER PROCEDURE v2.SyncState_Upsert
    @Entity         NVARCHAR(100),
    @SourceName     NVARCHAR(200),
    @RowVersion     BINARY(8),
    @AffectedCount  INT = NULL
AS
BEGIN
    SET NOCOUNT ON;

    MERGE v2.SyncState AS tgt
    USING (SELECT @Entity AS Entity, @SourceName AS SourceName) AS src
        ON tgt.Entity = src.Entity AND tgt.SourceName = src.SourceName
    WHEN MATCHED THEN
        UPDATE SET LastRowVersion = @RowVersion,
                   LastRunUtc = SYSUTCDATETIME(),
                   LastAffectedCount = @AffectedCount
    WHEN NOT MATCHED THEN
        INSERT (Entity, SourceName, LastRowVersion, LastRunUtc, LastAffectedCount)
        VALUES (@Entity, @SourceName, @RowVersion, SYSUTCDATETIME(), @AffectedCount);
END
GO
GO

--------------------------------------------------------------------------------
-- [10/24] Modules\AverageRateLevel3 :: vw_AverageRateLevel3_Detail.sql
-- Source: Modules\AverageRateLevel3\Programmability\vw_AverageRateLevel3_Detail.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Purpose:
  Detail-проекция для UI/full Excel (не snapshot grid).
  Strangler Fig: может присоединять legacy dbo.vw_* / ped.a_* — богаче, чем v2.*_Snapshot.
  Grid list → snapshot; detail/export → этот view ([DetailSource] на detail-DTO).

Example:

use mdm
go

SELECT *
FROM v2.vw_AverageRateLevel3_Detail
WHERE AverageRateLevel3Id = 34041206

*/
DROP VIEW IF EXISTS v2.vw_AverageRateLevel3Detail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetAverageRateLevel3Detail;
GO

DROP VIEW IF EXISTS v2.vw_AverageRateLevel3s_Detail;
GO

CREATE OR ALTER VIEW v2.vw_AverageRateLevel3_Detail
AS
    SELECT
     CAST(ar.Id AS INT)              AS AverageRateLevel3Id
    ,CAST(ar.Code AS BIGINT)	        AS Code
    ,CASE WHEN ISNULL(ar.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END 
                                    AS IsArchive
    ,ISNULL(ar.IsDefRate, 0)         AS IsDefRate
    ,ar.CreationDate 	            AS CreationDate
    ,ar.LastChangeDate 	            AS LastChangeDate
    ,trc.TransportRateCodes          AS TransportRateCodes
    ,CAST(ar.CalcDate AS DATE)       AS CalcDate
    ,ar.RateLevel3 		            AS RateLevel3
    ,curSt.Code 		            AS CurrencyStandard
    ,CAST(ar.CurrencyRateMonth AS DATE)	        AS CurrencyRateMonth
    ,ar.EffectiveLoadOfTransportType AS EffectiveLoadOfTransportType
    ,ar.MinDailyTransportation       AS MinDailyTransportation
    ,ar.MaxDailyTransportation       AS MaxDailyTransportation
    ,CAST(ar.StartDate AS DATE) 	    AS StartDate
    ,CAST(ar.EndDate  AS DATE)	    AS EndDate
    ,rtp.Code		                AS TypeCode
    ,rtp.[Name]		                AS TypeName
    ,ar.TotalCostTonRUB 	            AS TotalCostTonRUB
    ,ar.TotalCostTonEUR 	            AS TotalCostTonEUR
    ,ar.TotalCostTonCNY 	            AS TotalCostTonCNY
    ,ar.TotalCostTonUSD 	            AS TotalCostTonUSD
    ,ar.TotalCostTransportRUB 	    AS TotalCostTransportRUB
    ,ar.TotalCostTransportEUR 	    AS TotalCostTransportEUR
    ,ar.TotalCostTransportCNY 	    AS TotalCostTransportCNY
    ,ar.TotalCostTransportUSD 	    AS TotalCostTransportUSD
    ,ar.EmptyRFSize 	                AS EmptyRFSize
    ,curEmptyRF.Code 		        AS EmptyRFCurrency
    ,ar.EmptyCISSize  	            AS EmptyCISSize
    ,curEmptyCIS.Code		        AS EmptyCISCurrency
    ,ar.ProvisionTransportSize	    AS ProvisionTransportSize
    ,curProvisionTransport.Code	    AS ProvisionTransportCurrency
    ,ar.FerryboatSize	            AS FerryboatSize
    ,curFerryboat.Code		        AS FerryboatCurrency
    ,ar.TEFromSize	                AS TEFromSize
    ,curTEFrom.Code		            AS TEFromCurrency
    ,ar.PNPFromSize	                AS PNPFromSize
    ,curPNPFrom.Code		        AS PNPFromCurrency
    ,ar.TEToSize	                    AS TEToSize
    ,curTETo.Code		            AS TEToCurrency
    ,ar.PNPToSize	                AS PNPToSize
    ,curPNPTo.Code		            AS PNPToCurrency
    ,ar.DrainLoadingSize	            AS DrainLoadingSize
    ,curDrainLoading.Code	        AS DrainLoadingCurrency
    ,ar.TransshipmentSize	        AS TransshipmentSize
    ,curTransshipment.Code		    AS TransshipmentCurrency
    ,ar.FreightSize	                AS FreightSize
    ,curFreight.Code		        AS FreightCurrency
    ,ar.AdditionalFeesCISSize	    AS AdditionalFeesCISSize
    ,curAdditionalFeesCIS.Code	    AS AdditionalFeesCISCurrency
    ,ar.LoadedCISSize	            AS LoadedCISSize
    ,curLoadedCIS.Code		        AS LoadedCISCurrency
    ,ar.LoadedRFSize	                AS LoadedRFSize
    ,curLoadedRF.Code		        AS LoadedRFCurrency
    ,ar.TEFromSize_fix	            AS TEFromSize_fix
    ,curTEFrom_fix.Code		        AS TEFromCurrency_fix
    ,ar.TEToSize_fix	                AS TEToSize_fix
    ,curTETo_fix.Code		        AS TEToCurrency_fix
    ,nf.Code		                AS NodeFromCode
    ,nf.Name_ru		                AS NodeFromNameRu
    ,nf.Name_en		                AS NodeFromNameEn
    ,rf.Code		                AS RegionFromCode
    ,rf.Name_ru		                AS RegionFromNameRu
    ,rf.Name_en		                AS RegionFromNameEn
    ,np.Code		                AS ProxyNodeCode
    ,np.Name_ru		                AS ProxyNodeNameRu
    ,np.Name_en		                AS ProxyNodeNameEn
    ,rp.Code		                AS ProxyRegionCode
    ,rp.Name_ru		                AS ProxyRegionNameRu
    ,rp.Name_en		                AS ProxyRegionNameEn
    ,nt.Code		                AS NodeToCode
    ,nt.Name_ru		                AS NodeToNameRu
    ,nt.Name_en		                AS NodeToNameEn
    ,rt.Code		                AS RegionToCode
    ,rt.Name_ru		                AS RegionToNameRu
    ,rt.Name_en		                AS RegionToNameEn
    ,bas.[Name]		                AS Basis
    ,nb.Code		                AS BasisNodeCode
    ,nb.Name_ru		                AS BasisNodeNameRu
    ,tk.Code		                AS TransportKindCode
    ,tk.[Name]		                AS TransportKindNameRu
    ,tk.NameEnRu		            AS TransportKindNameRuEn
    ,tt.Code		                AS TransportTypeCode
    ,tt.[Name]		                AS TransportTypeNameRu
    ,tt.NameEnRu		            AS TransportTypeNameRuEn
    ,pg.Code		                AS ProductGroupCode
    ,pg.NameEnRu 		            AS ProductGroupNameEnRu
    ,p.Code 		                AS ProductCode
    ,p.NameFull_ru		            AS ProductNameRu
    ,p.NameFull_en 		            AS ProductNameEn
    ,p.DPOCode 		                AS ProductDPOCOde
    ,ar.Comment	                    AS Comment
    ,lg.Code		                AS LegCode
    ,lg.LastChangeDate 		        AS LegChangeDate
    ,lt.LastChangeDate 		        AS LeadTimeChangeDate
    ,lt.StartDate 		            AS LeadTimeStartDate
    ,lt.EndDate		                AS LeadTimeEndDate
    ,lt.Code 		                AS LeadTimeCode
    ,lt.SearchTime 		            AS LeadTimeSearchTime
    ,lt.LoadTime 		            AS LeadTimeLoadTime
    ,lt.TravelTime 		            AS LeadTimeTravelTime
    ,lt.DaysWaiting		            AS LeadTimeDaysWaiting
    ,lt.UnLoadTime 		            AS LeadTimeUnLoadTime
    ,lt.TransportationTime 		    AS LeadTimeTransportationTime
    ,lt.Distance 		            AS LeadTimeDistance
    ,tt1.Code		                AS Leg1_TransportTypeCode
    ,tt1.[Name]		                AS Leg1_TransportTypeNameRu
    ,tt1.NameEnRu		            AS Leg1_TransportTypeNameRuEn
    ,ar.Leg1_EffectiveLoad	        AS Leg1_EffectiveLoad
    ,ar.Leg1_TotalCostTon	        AS Leg1_TotalCostTon
    ,ar.Leg1_TotalCostTransport	    AS Leg1_TotalCostTransport
    ,leg1_cur.Code		            AS Leg1_BaseCurrency
    ,ar.Leg1_TotalCostTonRUB	        AS Leg1_TotalCostTonRUB
    ,ar.Leg1_TotalCostTonUSD	        AS Leg1_TotalCostTonUSD
    ,ar.Leg1_TotalCostTonEUR	        AS Leg1_TotalCostTonEUR
    ,ar.Leg1_TotalCostTonCNY	        AS Leg1_TotalCostTonCNY
    ,ar.Leg1_TotalCostTransportRUB	AS Leg1_TotalCostTransportRUB
    ,ar.Leg1_TotalCostTransportUSD	AS Leg1_TotalCostTransportUSD
    ,ar.Leg1_TotalCostTransportEUR	AS Leg1_TotalCostTransportEUR
    ,ar.Leg1_TotalCostTransportCNY	AS Leg1_TotalCostTransportCNY
    ,lt.Leg1_SearchTime 		    AS LeadTimeLeg1_SearchTime
    ,lt.Leg1_LoadTime 		        AS LeadTimeLeg1_LoadTime
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg1_TravelTime
    ,lt.Leg1_DaysWaiting 		    AS LeadTimeLeg1_DaysWaiting
    ,lt.Leg1_TransportationTime     AS LeadTimeLeg1_TransportationTime
    ,lt.Leg1_Distance 		        AS LeadTimeLeg1_Distance
    ,tt2.Code		                AS Leg2_TransportTypeCode
    ,tt2.[Name]		                AS Leg2_TransportTypeNameRu
    ,tt2.NameEnRu		            AS Leg2_TransportTypeNameRuEn
    ,ar.Leg2_EffectiveLoad	        AS Leg2_EffectiveLoad
    ,ar.Leg2_TotalCostTon	        AS Leg2_TotalCostTon
    ,ar.Leg2_TotalCostTransport	    AS Leg2_TotalCostTransport
    ,leg2_cur.Code		            AS Leg2_BaseCurrency
    ,ar.Leg2_TotalCostTonRUB	        AS Leg2_TotalCostTonRUB
    ,ar.Leg2_TotalCostTonUSD	        AS Leg2_TotalCostTonUSD
    ,ar.Leg2_TotalCostTonEUR	        AS Leg2_TotalCostTonEUR
    ,ar.Leg2_TotalCostTonCNY	        AS Leg2_TotalCostTonCNY
    ,ar.Leg2_TotalCostTransportRUB	AS Leg2_TotalCostTransportRUB
    ,ar.Leg2_TotalCostTransportUSD	AS Leg2_TotalCostTransportUSD
    ,ar.Leg2_TotalCostTransportEUR	AS Leg2_TotalCostTransportEUR
    ,ar.Leg2_TotalCostTransportCNY	AS Leg2_TotalCostTransportCNY
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg2_TravelTime
    ,lt.Leg2_DaysWaiting 		    AS LeadTimeLeg2_DaysWaiting
    ,lt.Leg2_UploadTime 		    AS LeadTimeLeg2_UploadTime
    ,lt.Leg2_TransportationTime 	AS LeadTimeLeg2_TransportationTime
    ,lt.Leg2_Distance 		        AS LeadTimeLeg2_Distance
    FROM vw_AverageRateLevel3 AS ar

    OUTER APPLY (
        SELECT STRING_AGG(CAST(r.Code AS NVARCHAR(50)), ', ') AS TransportRateCodes
        FROM vw_RatesOfAverageRate rar
        JOIN vw_TransportRate r ON r.Id = rar.Rate
        WHERE rar.AverageRateLevel3Id = ar.Id
          AND rar.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
          AND r.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
    ) trc

    JOIN vw_RateType rtp 
        ON ar.RateType = rtp.Id			

    JOIN vw_Currency curSt 
        ON curSt.Id = ar.CurrencyStandard		
    LEFT JOIN vw_Currency curEmptyRF 
        ON curEmptyRF.Id = ar.EmptyRFCurrency		
    LEFT JOIN vw_Currency curEmptyCIS 
        ON curEmptyCIS.Id = ar.EmptyCISCurrency		
    LEFT JOIN vw_Currency curProvisionTransport 
        ON curProvisionTransport.Id = ar.ProvisionTransportCurrency		
    LEFT JOIN vw_Currency curFerryboat 
        ON curFerryboat.Id = ar.FerryboatCurrency		
    LEFT JOIN vw_Currency curTEFrom 
        ON curTEFrom.Id = ar.TEFromCurrency		
    LEFT JOIN vw_Currency curPNPFrom 
        ON curPNPFrom.Id = ar.PNPFromCurrency		
    LEFT JOIN vw_Currency curTETo 
        ON curTETo.Id = ar.TEToCurrency		
    LEFT JOIN vw_Currency curPNPTo 
        ON curPNPTo.Id = ar.PNPToCurrency	
    LEFT JOIN vw_Currency curDrainLoading 
        ON curDrainLoading.Id = ar.DrainLoadingCurrency		
    LEFT JOIN vw_Currency curTransshipment 
        ON curTransshipment.Id = ar.TransshipmentCurrency		
    LEFT JOIN vw_Currency curFreight  
        ON curFreight.Id = ar.FreightCurrency		
    LEFT JOIN vw_Currency curAdditionalFeesCIS  
        ON curAdditionalFeesCIS.Id = ar.AdditionalFeesCISCurrency		
    LEFT JOIN vw_Currency curLoadedCIS 
        ON curLoadedCIS.Id = ar.LoadedCISCurrency		
    LEFT JOIN vw_Currency curLoadedRF  
        ON curLoadedRF.Id = ar.LoadedRFCurrency		
    LEFT JOIN vw_Currency curTEFrom_fix 
        ON curTEFrom_fix.Id = ar.TEFromCurrency_fix		
    LEFT JOIN vw_Currency curTETo_fix 
        ON curTETo_fix.Id = ar.TEToCurrency_fix		

    JOIN vw_LocationsNodes nf 
        ON nf.Id = ar.NodeFrom			
    LEFT JOIN vw_Region rf 
        ON rf.Id = nf.Region	

    LEFT JOIN vw_LocationsNodes np 
        ON np.Id = ar.ProxyNode			
    LEFT JOIN vw_Region rp 
        ON rp.Id = np.Region			

    JOIN vw_LocationsNodes nt 
        ON nt.Id = ar.NodeTo			
    LEFT JOIN vw_Region rt 
        ON rt.Id = nt.Region			
			
    LEFT JOIN vw_Basis bas 
        ON bas.Id = ar.basis			
    LEFT JOIN vw_LocationsNodes nb 
        ON nb.Id = ar.BasisNode			
			
    JOIN vw_TransportKind tk 
        ON tk.Id = ar.TransportKind			
			
    JOIN vw_TransportType_level_3 tt 
        ON tt.Id = ar.TransportType			
			
    LEFT JOIN vw_ProductGroup pg 
        ON pg.Id = ar.ProductGroup			
    LEFT JOIN vw_MTR p  
        ON p.Id = ar.Product			

    CROSS APPLY (
        SELECT TOP 1
            lar.TransportLegId
        FROM vw_LegAverageRateLevel3 lar
        WHERE lar.Rate = ar.Id
        ORDER BY lar.Id DESC
    ) lar

    CROSS APPLY (
        SELECT TOP 1
            lg.Id,
            lg.Code,
            lg.LastChangeDate
        FROM vw_TransportLeg lg
        WHERE lg.Id = lar.TransportLegId
        ORDER BY lg.Id DESC
    ) lg
    
    OUTER APPLY (
        SELECT TOP (1)
            lit.*
        FROM vw_Leadtime AS lit
        INNER JOIN vw_ShipmentType AS st
            ON st.Id = lit.ShipmentType AND st.IsMain = 1
        WHERE lit.TransportLegId = lg.Id
          AND CAST(ar.EndDate AS DATE) >= CAST(lit.StartDate AS DATE)
          AND CAST(lit.EndDate AS DATE) >= CAST(ar.StartDate AS DATE)
        ORDER BY lit.PrimitiveEntityDataStateId ASC,
                 lit.StartDate ASC,
                 lit.LastChangeDate DESC
    ) AS lt
			
    LEFT JOIN vw_TransportType_level_3 tt1 
        ON tt1.Id = ar.Leg1_TransportType			
    LEFT JOIN vw_TransportType_level_3 tt2 
        ON tt2.Id = ar.Leg2_TransportType			
			
    LEFT JOIN vw_Currency leg1_cur 
        ON leg1_cur.Id = ar.Leg1_BaseCurrency		
    LEFT JOIN vw_Currency leg2_cur 
        ON leg2_cur.Id = ar.Leg2_BaseCurrency		
GO
GO

--------------------------------------------------------------------------------
-- [11/24] Modules\AverageRateLevel3 :: vw_AverageRateLevel3_SnapshotSource.sql
-- Source: Modules\AverageRateLevel3\Programmability\vw_AverageRateLevel3_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot AverageRateLevel3 из legacy-вью — единый источник истины
    для read-модели. Используется первичной заливкой и инкрементальной синхронизацией.

    Членство в snapshot:
      - RateLevel3 IS NOT NULL
      - валидные Code / RateType / Product (как TransportRate)
      - EXISTS хотя бы одной связанной TransportRate с тем же PrimitiveEntityDataStateId
*/

CREATE OR ALTER VIEW v2.vw_AverageRateLevel3_SnapshotSource
AS
    SELECT
        CAST(ar.Id AS INT)                                              AS AverageRateLevel3Id,
        CASE WHEN ISNULL(ar.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(ar.IsDefRate, 0)                                         AS IsDefRate,
        ar.StartDate                                                    AS StartDate,
        ar.EndDate                                                      AS EndDate,
        ar.CreationDate                                                 AS CreationDate,
        ISNULL(ar.LastChangeDate, ar.CreationDate)                        AS LastChangeDate,
        ar.RateLevel3                                                   AS RateLevel3,
        ISNULL(ar.EffectiveLoadOfTransportType, 0)                      AS EffectiveLoadOfTransportType,
        CAST(ar.TransportKind AS INT)                                   AS TransportKindId,
        CAST(ar.TransportType AS INT)                                   AS TransportTypeId,
        CAST(ar.RateType AS INT)                                        AS RateTypeId,
        CAST(ar.CurrencyStandard AS INT)                                AS CurrencyId,
        TRY_CAST(LEFT(ar.Code, 10) AS INT)                              AS Code,
        TRY_CAST(LEFT(rt.Code, 2) AS INT)                               AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                                AS ProductCode,
        LEFT(cur.Code, 3)                                               AS CurrencyCode,
        LEFT(nf.Code, 10)                                               AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                            AS NodeFromNameRu,
        LEFT(np.Code, 10)                                               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                            AS ProxyNodeNameRu,
        LEFT(nt.Code, 10)                                               AS NodeToCode,
        LEFT(nt.Name_en, 30)                                            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                            AS NodeToNameRu,
        LEFT(tk.Code, 5)                                                AS TransportKindCode,
        LEFT(tt.Code, 20)                                               AS TransportTypeCode,
        LEFT(pg.Code, 5)                                                AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100)         AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)             AS ProductGroupNameEn,
        LEFT(p.NameShort_ru, 100)                                       AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                                       AS ProductNameEn,
        CAST(nf.Id AS INT)                                              AS NodeFromId,
        CAST(nt.Id AS INT)                                              AS NodeToId,
        CAST(np.Id AS INT)                                              AS ProxyNodeId,
        CAST(pg.Id AS INT)                                              AS ProductGroupId,
        CAST(p.Id AS INT)                                               AS ProductId
    FROM vw_AverageRateLevel3 ar (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON ar.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON ar.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON ar.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON ar.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON ar.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON ar.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON ar.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON ar.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON ar.CurrencyStandard = cur.Id
    WHERE ar.RateLevel3 IS NOT NULL
      AND TRY_CAST(LEFT(ar.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
      AND EXISTS (
          SELECT 1
          FROM vw_RatesOfAverageRate rar (NOLOCK)
          JOIN vw_TransportRate r (NOLOCK) ON r.Id = rar.Rate
          WHERE rar.AverageRateLevel3Id = ar.Id
            AND rar.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
            AND r.PrimitiveEntityDataStateId = ar.PrimitiveEntityDataStateId
      );
GO
GO

--------------------------------------------------------------------------------
-- [12/24] Modules\AverageRateLevel3 :: sp_AverageRateLevel3_PopulateAffectedKeys.sql
-- Source: Modules\AverageRateLevel3\Programmability\sp_AverageRateLevel3_PopulateAffectedKeys.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Детекция изменившихся средневзвешенных ставок AverageRateLevel3 по одному
    источнику за вызов. По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно)
    добавляет в #AffectedKeys бизнес-ключи (AverageRateLevel3Id) затронутых записей.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_2057 — AverageRateLevel3 (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад: NodeFrom/To/Proxy).
      dbo.PrimitiveEntityData_1013 — ProductGroup   (каскад: ProductGroupId).
      dbo.PrimitiveEntityData_1015 — MTR/Product     (каскад: ProductId).

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE проекции. Источник 1015 обязателен — иначе
    вход/выход строки из snapshot отследит только суточный reconcile.

    RateType (2048), TransportKind (2008), TransportType (2023) и Currency (2016)
    стабильны — не отслеживаются. RatesOfAverageRate (2058) и TransportRate (2012)
    не отслеживаются: legacy меняет ставки и средние одновременно.

    Каскад — эквиджойн по одной FK-колонке snapshot (для 1014 — UNION по трём):
    позволяет использовать NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.AverageRateLevel3_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- AverageRateLevel3 (основная): изменившиеся PrimitiveEntityItemId.
    IF @Source = N'dbo.PrimitiveEntityData_2057' -- AverageRateLevel3 (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2057 d WITH (NOLOCK) -- AverageRateLevel3 (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- LocationsNodes: затронутые средние через NodeFromId / NodeToId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.AverageRateLevel3Id AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.AverageRateLevel3_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.AverageRateLevel3Id
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.AverageRateLevel3_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.AverageRateLevel3Id
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.AverageRateLevel3_Snapshot s ON s.ProxyNodeId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- ProductGroup: затронутые средние через ProductGroupId.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.AverageRateLevel3Id
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.AverageRateLevel3_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.AverageRateLevel3Id);
        RETURN;
    END

    -- MTR/Product: затронутые средние через ProductId (важно для WHERE-членства).
    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.AverageRateLevel3Id
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.AverageRateLevel3_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.AverageRateLevel3Id);
        RETURN;
    END

    ;THROW 50000, 'Unknown AverageRateLevel3 sync source.', 1;
END
GO
GO

--------------------------------------------------------------------------------
-- [13/24] Modules\LocationsNodes :: vw_LocationsNodes_Detail.sql
-- Source: Modules\LocationsNodes\Programmability\vw_LocationsNodes_Detail.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Purpose:
  Detail-проекция для UI/full Excel (не snapshot grid).
  Strangler Fig: может присоединять legacy dbo.vw_* / ped.a_* — богаче, чем v2.*_Snapshot.
  Grid list → snapshot; detail/export → этот view ([DetailSource] на detail-DTO).

Example:

use mdm
go

SELECT *
FROM v2.vw_LocationsNodes_Detail
WHERE LocationsNodesId = 2254605

*/
DROP VIEW IF EXISTS v2.vw_LocationNodes_Detail;
GO

DROP VIEW IF EXISTS v2.vw_LocationsNodesDetail;
GO

CREATE OR ALTER VIEW v2.vw_LocationsNodes_Detail
AS
    SELECT
        CAST(n.Id AS BIGINT) AS LocationsNodesId,
        n.Code AS Code,

        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        n.NameZD AS NameZD,
        n.Seaport AS Seaport,
        n.AutoNode AS AutoNode,
        n.FIASRegion AS FIASRegion,
        n.FIASDistrict AS FIASDistrict,
        n.FIASCity AS FIASCity,
        n.Terminal AS Terminal,
        n.OpenStreetMap AS OpenStreetMap,
        n.Virtual AS Virtual,
        n.IsFactory AS IsFactory,
        n.IsPortShip AS IsPortShip,
        n.IsPortStore AS IsPortStore,
        n.IsStore AS IsStore,
        ped.a_3862 AS CoordinateW,
        ped.a_3863 AS CoordinateL,
        n.Code_NSI AS Code_NSI,
        n.Status AS Status,
        n.PointTube AS PointTube,
        n.Street AS Street,
        n.House AS House,
        n.OfficeApart AS OfficeApart,
        n.ZDCodeN AS ZDCodeN,
        n.NodesCode AS NodesCode,

        n.Name_ru AS NameRu,
        n.Name_en AS NameEn,

        CAST(n.LocationType AS BIGINT) AS LocationTypeIdRu,
        CAST(n.LocationType AS BIGINT) AS LocationTypeIdEn,
        tp.Code AS LocationTypeCode,
        tp.Name AS LocationTypeNameRu,
        tp.NameEnRu AS LocationTypeNameEn,

        CAST(n.TypeNode AS BIGINT) AS TypeNodeIdRu,
        CAST(n.TypeNode AS BIGINT) AS TypeNodeIdEn,
        tn.Code AS TypeNodeCode,
        tn.NameRu AS TypeNodeNameRu,
        tn.NameEn AS TypeNodeNameEn,

        CAST(n.Region AS BIGINT) AS RegionIdRu,
        CAST(n.Region AS BIGINT) AS RegionIdEn,
        r.Code AS RegionCode,
        r.Name_ru AS RegionNameRu,
        r.Name_en AS RegionNameEn,
        n.RegionRU AS RegionRU,

        CAST(n.Country AS BIGINT) AS CountryIdRu,
        CAST(n.Country AS BIGINT) AS CountryIdEn,
        c.Code AS CountryCode,
        c.Name_ru AS CountryNameRu,
        c.Name_en AS CountryNameEn,

        n.City AS City,
        n.FullAddress AS FullAddress,
        n.BorderCrossing AS BorderCrossing,
        n.LocationTypeCodeNSI AS LocationTypeCodeNSI,
        n.TypeNodeCodeNSI AS TypeNodeCodeNSI,
        n.RegionNSIEN AS RegionNSIEN,
        n.RegionCodeNSI AS RegionCodeNSI,
        n.RegionCodeDPO AS RegionCodeDPO,
        n.CountryRU AS CountryRU,
        n.NameCountryEN AS NameCountryEN,
        n.CountryISO2 AS CountryISO2,
        n.CountryISO3 AS CountryISO3,
        n.CountryCodeDPO AS CountryCodeDPO,
        n.MarketNameRU AS MarketNameRU,
        n.MarketNameEN AS MarketNameEN,
        n.MarketCode AS MarketCode,
        n.MarketCodeDPO AS MarketCodeDPO,
        n.CodeZDroad AS CodeZDroad,
        n.NameZDroadRU AS NameZDroadRU,
        n.NameZDroadEN AS NameZDroadEN,
        n.NameZDEN AS NameZDEN,
        n.AddressCountryISO2 AS AddressCountryISO2,
        n.AddressCountryISO3 AS AddressCountryISO3,
        n.AddressCountryCodeDPO AS AddressCountryCodeDPO,
        n.AddressNameCountryRU AS AddressNameCountryRU,
        n.AddressNameCountryEN AS AddressNameCountryEN,
        n.Pobox AS Pobox,
        n.NameFederalDistrictRU AS NameFederalDistrictRU,
        n.NameDistrictRU AS NameDistrictRU,
        n.NameCityRU AS NameCityRU,
        n.NameCityDistrictRU AS NameCityDistrictRU,
        n.IsKladr AS IsKladr,
        n.AddressLanguage AS AddressLanguage,
        n.RegionCodeRF AS RegionCodeRF,
        n.AddressRegionISO AS AddressRegionISO,
        n.FIASCodeCity AS FIASCodeCity,
        n.FIASStreet AS FIASStreet,
        n.FIASHouse AS FIASHouse,
        n.OKTMOCode AS OKTMOCode,
        n.FIASCodeAddress AS FIASCodeAddress,
        n.IsDadata AS IsDadata,
        n.IsArchive AS CannotDeliver,
        n.BorderCountryISO2 AS BorderCountryISO2,
        n.BorderCountryISO3 AS BorderCountryISO3,
        n.BorderCountryCodeDPO AS BorderCountryCodeDPO,
        n.BorderNameCountryRU AS BorderNameCountryRU,
        n.BorderNameCountryEN AS BorderNameCountryEN,
        n.StatusNSI AS StatusNSI,
        n.IsPlanning AS IsPlanning,
        n.IsPlan AS IsPlan,
        n.[4_level_CityRU] AS Level4CityRU,
        n.[4_level_CityEN] AS Level4CityEN,
        n.[4_level_City_FIAS] AS Level4CityFias,

        n.CreationDate AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    JOIN dbo.PrimitiveEntityData_1014 ped (NOLOCK) ON ped.PrimitiveEntityItemId = n.Id
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id
GO
GO

--------------------------------------------------------------------------------
-- [14/24] Modules\LocationsNodes :: vw_LocationsNodes_SnapshotSource.sql
-- Source: Modules\LocationsNodes\Programmability\vw_LocationsNodes_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot LocationsNodes из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.LocationsNodes_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.LocationsNodes_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    FK-колонки (LocationTypeId / TypeNodeId / RegionId / CountryId) входят в
    snapshot и попадают в грид, но также используются синхронизацией для
    каскадной инвалидации (изменение справочника -> затронутые узлы).
*/

CREATE OR ALTER VIEW v2.vw_LocationsNodes_SnapshotSource
AS
    SELECT
        CAST(n.Id AS BIGINT) AS LocationsNodesId,
        LEFT(n.Code, 50) AS Code,
        CASE WHEN ISNULL(n.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,

        LEFT(n.Name_ru, 100) AS NameRu,
        LEFT(n.Name_en, 100) AS NameEn,

        CAST(n.LocationType AS BIGINT) AS LocationTypeId,
        LEFT(tp.Code, 50) AS LocationTypeCode,
        LEFT(tp.Name, 100) AS LocationTypeNameRu,
        LEFT(tp.NameEnRu, 100) AS LocationTypeNameEn,

        CAST(n.TypeNode AS BIGINT) AS TypeNodeId,
        LEFT(tn.Code, 50) AS TypeNodeCode,
        LEFT(tn.NameRu, 100) AS TypeNodeNameRu,
        LEFT(tn.NameEn, 100) AS TypeNodeNameEn,

        CAST(n.Region AS BIGINT) AS RegionId,
        LEFT(r.Code, 50) AS RegionCode,
        LEFT(r.Name_ru, 100) AS RegionNameRu,
        LEFT(r.Name_en, 100) AS RegionNameEn,
        LEFT(n.RegionRU, 100) AS RegionRU,

        CAST(n.Country AS BIGINT) AS CountryId,
        LEFT(c.Name_ru, 100) AS CountryNameRu,
        LEFT(c.Name_en, 100) AS CountryNameEn,

        n.CreationDate AS CreationDate,
        ISNULL(n.LastChangeDate, n.CreationDate) AS LastChangeDate

    FROM vw_LocationsNodes n (NOLOCK)
    LEFT JOIN vw_TypePlace tp (NOLOCK) ON n.LocationType = tp.Id
    LEFT JOIN vw_TypeNode tn (NOLOCK) ON n.TypeNode = tn.Id
    LEFT JOIN vw_Region r (NOLOCK) ON n.Region = r.Id
    LEFT JOIN vw_Country c (NOLOCK) ON n.Country = c.Id
GO
GO

--------------------------------------------------------------------------------
-- [15/24] Modules\LocationsNodes :: sp_LocationsNodes_PopulateAffectedKeys.sql
-- Source: Modules\LocationsNodes\Programmability\sp_LocationsNodes_PopulateAffectedKeys.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Детекция изменившихся узлов LocationsNodes по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (LocationsNodesId) затронутых узлов.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_1014 — LocationsNodes (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1008 — Region    (каскад через RegionId).
      dbo.PrimitiveEntityData_1009 — Country   (каскад через CountryId).

    TypePlace (1007) и TypeNode (2132) стабильны — не отслеживаются.

    Каскад — эквиджойн по одной FK-колонке snapshot: позволяет использовать
    NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.LocationsNodes_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- LocationsNodes (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи узлов.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_1014 d WITH (NOLOCK) -- LocationsNodes (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- Region: затронутые узлы через RegionId.
    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.LocationsNodesId
        FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
        JOIN v2.LocationsNodes_Snapshot s ON s.RegionId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.LocationsNodesId);
        RETURN;
    END

    -- Country: затронутые узлы через CountryId.
    IF @Source = N'dbo.PrimitiveEntityData_1009' -- Country
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.LocationsNodesId
        FROM dbo.PrimitiveEntityData_1009 r WITH (NOLOCK) -- Country
        JOIN v2.LocationsNodes_Snapshot s ON s.CountryId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.LocationsNodesId);
        RETURN;
    END

    ;THROW 50000, 'Unknown LocationsNodes sync source.', 1;
END
GO
GO

--------------------------------------------------------------------------------
-- [16/24] Modules\ParityRates :: vw_ParityRates_Detail.sql
-- Source: Modules\ParityRates\Programmability\vw_ParityRates_Detail.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
Purpose:
  Detail-проекция ParityRates для UI/full Excel.
  Без leadtime / leg1_* / leg2_* секций (их нет у сущности).

Example:
  SELECT * FROM v2.vw_ParityRates_Detail WHERE ParityRatesId = ...
*/

CREATE OR ALTER VIEW v2.vw_ParityRates_Detail
AS
    SELECT
        CAST(pr.Id AS INT)                                      AS ParityRatesId,
        LEFT(pr.Code, 50)                                       AS Code,
        CASE WHEN ISNULL(pr.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        pr.CreationDate                                         AS CreationDate,
        pr.LastChangeDate                                       AS LastChangeDate,
        CAST(pr.StartDate AS DATE)                              AS StartDate,
        CAST(pr.EndDate AS DATE)                                AS EndDate,

        LEFT(rel.Code, 50)                                      AS RelevanceCode,
        LEFT(rel.Name, 100)                                     AS RelevanceName,

        LEFT(nf.Code, 10)                                       AS NodeFromCode,
        LEFT(nf.Name_en, 100)                                   AS NodeFromNameEn,
        LEFT(nf.Name_ru, 100)                                   AS NodeFromNameRu,

        LEFT(np1.Code, 10)                                      AS ProxyNode1Code,
        LEFT(np1.Name_en, 100)                                  AS ProxyNode1NameEn,
        LEFT(np1.Name_ru, 100)                                  AS ProxyNode1NameRu,

        LEFT(np2.Code, 10)                                      AS ProxyNode2Code,
        LEFT(np2.Name_en, 100)                                  AS ProxyNode2NameEn,
        LEFT(np2.Name_ru, 100)                                  AS ProxyNode2NameRu,

        LEFT(nt.Code, 10)                                       AS NodeToCode,
        LEFT(nt.Name_en, 100)                                   AS NodeToNameEn,
        LEFT(nt.Name_ru, 100)                                   AS NodeToNameRu,

        LEFT(tt.Code, 20)                                       AS TransportTypeCode,
        LEFT(tt.Name, 100)                                      AS TransportTypeNameRu,
        LEFT(tt.NameEnRu, 100)                                  AS TransportTypeNameEn,

        LEFT(pg.Code, 5)                                        AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        TRY_CAST(LEFT(p.Code, 7) AS INT)                        AS ProductCode,
        LEFT(p.NameShort_ru, 100)                               AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                               AS ProductNameEn,

        pr.Level_Danger_Product                                 AS Level_Danger_Product,
        ISNULL(pr.Dangerous_Cargo, 0)                           AS Dangerous_Cargo,

        pr.TotalCostTransport                                   AS TotalCostTransport,
        pr.LoadOfTransport                                      AS LoadOfTransport,
        pr.TotalCostTon                                         AS TotalCostTon,
        LEFT(cur.Code, 3)                                       AS CurrencyStandard,

        LEFT(pr.Comment, 1000)                                  AS Comment,
        LEFT(pr.DataSource, 4000)                               AS DataSource,
        pr.FactRate                                             AS FactRate,
        pr.BusinessPlanningRate                                 AS BusinessPlanningRate,
        LEFT(pr.DepartmentResponsibilityArea, 4000)             AS DepartmentResponsibilityArea,
        LEFT(pr.EmployeeResponsibilityArea, 4000)               AS EmployeeResponsibilityArea,
        LEFT(pr.Methodology, 4000)                              AS Methodology,
        LEFT(pr.PriorityText, 4000)                             AS PriorityText,
        LEFT(pr.MarketingDataStructure, 4000)                   AS MarketingDataStructure

    FROM vw_ParityRates pr (NOLOCK)
    LEFT JOIN vw_Relevance rel (NOLOCK) ON pr.Relevance = rel.Id
    LEFT JOIN vw_LocationsNodes nf (NOLOCK) ON pr.NodeFromCode = nf.Id
    LEFT JOIN vw_LocationsNodes nt (NOLOCK) ON pr.NodeToCode = nt.Id
    LEFT JOIN vw_LocationsNodes np1 (NOLOCK) ON pr.ProxyNode1 = np1.Id
    LEFT JOIN vw_LocationsNodes np2 (NOLOCK) ON pr.ProxyNode2 = np2.Id
    LEFT JOIN vw_TransportType_level_3 tt (NOLOCK) ON pr.TransportTypeCode = tt.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON pr.ProductGroupCode = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON pr.Product = p.Id
    LEFT JOIN vw_Currency cur (NOLOCK) ON pr.CurrencyStandard = cur.Id
GO
GO

--------------------------------------------------------------------------------
-- [17/24] Modules\ParityRates :: vw_ParityRates_SnapshotSource.sql
-- Source: Modules\ParityRates\Programmability\vw_ParityRates_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot ParityRates из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.ParityRates_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Членство в snapshot:
      - Code NOT NULL / непустой
      - StartDate / EndDate / TotalCostTon / TotalCostTransport NOT NULL
      - NodeFrom / NodeTo / ProductGroup / Relevance NOT NULL
      - INNER JOIN на обязательные справочники (валюта, тип транспорта и т.д.)

    Relevance (2108), Currency (2016), TransportType (2023) — стабильные,
    в sync-каскад не входят.
*/

CREATE OR ALTER VIEW v2.vw_ParityRates_SnapshotSource
AS
    SELECT
        CAST(pr.Id AS INT)                                      AS ParityRatesId,
        CASE WHEN ISNULL(pr.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        pr.StartDate                                            AS StartDate,
        pr.EndDate                                              AS EndDate,
        pr.CreationDate                                         AS CreationDate,
        ISNULL(pr.LastChangeDate, pr.CreationDate)              AS LastChangeDate,

        LEFT(pr.Code, 50)                                       AS Code,
        CAST(pr.Relevance AS INT)                               AS RelevanceId,
        CAST(pr.TransportTypeCode AS INT)                       AS TransportTypeId,
        CAST(pr.CurrencyStandard AS INT)                        AS CurrencyId,

        pr.TotalCostTon                                         AS TotalCostTon,
        pr.TotalCostTransport                                   AS TotalCostTransport,
        pr.LoadOfTransport                                      AS LoadOfTransport,
        pr.Level_Danger_Product                                 AS Level_Danger_Product,
        pr.FactRate                                             AS FactRate,
        pr.BusinessPlanningRate                                 AS BusinessPlanningRate,

        LEFT(cur.Code, 3)                                       AS CurrencyCode,
        LEFT(tt.Code, 20)                                       AS TransportTypeCode,

        LEFT(nf.Code, 10)                                       AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                    AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                    AS NodeFromNameRu,

        LEFT(np1.Code, 10)                                      AS ProxyNode1Code,
        LEFT(np1.Name_en, 30)                                   AS ProxyNode1NameEn,
        LEFT(np1.Name_ru, 30)                                   AS ProxyNode1NameRu,

        LEFT(np2.Code, 10)                                      AS ProxyNode2Code,
        LEFT(np2.Name_en, 30)                                   AS ProxyNode2NameEn,
        LEFT(np2.Name_ru, 30)                                   AS ProxyNode2NameRu,

        LEFT(nt.Code, 10)                                       AS NodeToCode,
        LEFT(nt.Name_en, 30)                                    AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                    AS NodeToNameRu,

        LEFT(pg.Code, 5)                                        AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        TRY_CAST(LEFT(p.Code, 7) AS INT)                        AS ProductCode,
        LEFT(p.NameShort_ru, 100)                               AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                               AS ProductNameEn,

        LEFT(pr.Comment, 1000)                                  AS Comment,
        LEFT(pr.DataSource, 4000)                               AS DataSource,
        LEFT(pr.DepartmentResponsibilityArea, 4000)             AS DepartmentResponsibilityArea,
        LEFT(pr.EmployeeResponsibilityArea, 4000)               AS EmployeeResponsibilityArea,
        LEFT(pr.Methodology, 4000)                              AS Methodology,
        LEFT(pr.PriorityText, 4000)                             AS PriorityText,
        LEFT(pr.MarketingDataStructure, 4000)                   AS MarketingDataStructure,

        CAST(nf.Id AS INT)                                      AS NodeFromId,
        CAST(nt.Id AS INT)                                      AS NodeToId,
        CAST(np1.Id AS INT)                                     AS ProxyNode1Id,
        CAST(np2.Id AS INT)                                     AS ProxyNode2Id,
        CAST(pg.Id AS INT)                                      AS ProductGroupId,
        CAST(p.Id AS INT)                                       AS ProductId

    FROM vw_ParityRates pr (NOLOCK)
    JOIN vw_Relevance rel (NOLOCK) ON pr.Relevance = rel.Id
    JOIN vw_LocationsNodes nf (NOLOCK) ON pr.NodeFromCode = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON pr.NodeToCode = nt.Id
    LEFT JOIN vw_LocationsNodes np1 (NOLOCK) ON pr.ProxyNode1 = np1.Id
    LEFT JOIN vw_LocationsNodes np2 (NOLOCK) ON pr.ProxyNode2 = np2.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON pr.TransportTypeCode = tt.Id
    JOIN vw_ProductGroup pg (NOLOCK) ON pr.ProductGroupCode = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON pr.Product = p.Id
    JOIN vw_Currency cur (NOLOCK) ON pr.CurrencyStandard = cur.Id

    WHERE pr.Code IS NOT NULL
      AND LTRIM(RTRIM(pr.Code)) <> N''
      AND pr.StartDate IS NOT NULL
      AND pr.EndDate IS NOT NULL
      AND pr.TotalCostTon IS NOT NULL
      AND pr.TotalCostTransport IS NOT NULL
      AND pr.LoadOfTransport IS NOT NULL
      AND pr.NodeFromCode IS NOT NULL
      AND pr.NodeToCode IS NOT NULL
      AND pr.ProductGroupCode IS NOT NULL
      AND pr.Relevance IS NOT NULL
GO
GO

--------------------------------------------------------------------------------
-- [18/24] Modules\ParityRates :: sp_ParityRates_PopulateAffectedKeys.sql
-- Source: Modules\ParityRates\Programmability\sp_ParityRates_PopulateAffectedKeys.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Детекция изменившихся ParityRates по одному источнику за вызов.

    Источники:
      dbo.PrimitiveEntityData_2109 — ParityRates (основная)
      dbo.PrimitiveEntityData_1014 — LocationsNodes (NodeFrom/To/ProxyNode1/2)
      dbo.PrimitiveEntityData_1013 — ProductGroup
      dbo.PrimitiveEntityData_1015 — MTR/Product

    Relevance (2108), Currency (2016), TransportType (2023) стабильны — не отслеживаются.
*/

CREATE OR ALTER PROCEDURE v2.ParityRates_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    IF @Source = N'dbo.PrimitiveEntityData_2109' -- ParityRates (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2109 d WITH (NOLOCK) -- ParityRates (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.ParityRatesId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.ProxyNode1Id = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.ParityRatesId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.ParityRates_Snapshot s ON s.ProxyNode2Id = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ParityRatesId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.ParityRates_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ParityRatesId);
        RETURN;
    END

    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.ParityRatesId
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.ParityRates_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.ParityRatesId);
        RETURN;
    END

    ;THROW 50000, 'Unknown ParityRates sync source.', 1;
END
GO
GO

--------------------------------------------------------------------------------
-- [19/24] Modules\TransportLeg :: vw_TransportLeg_Detail.sql
-- Source: Modules\TransportLeg\Programmability\vw_TransportLeg_Detail.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Purpose:
  Detail-проекция для UI/full Excel (не snapshot grid).
  Strangler Fig: может присоединять legacy dbo.vw_* / ped.a_* — богаче, чем v2.*_Snapshot.
  Grid list → snapshot; detail/export → этот view ([DetailSource] на detail-DTO).

  ShipmentTypeCodeT — сырой legacy-атрибут (коды через '/'); JOIN на vw_ShipmentType нет.

Example:

use mdm
go

SELECT *
FROM v2.vw_TransportLeg_Detail
WHERE TransportLegId = 2845464

*/
DROP VIEW IF EXISTS v2.vw_TransportLegDetail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetTransportLegDetail;
GO

DROP VIEW IF EXISTS v2.vw_TransportLegs_Detail;
GO

CREATE OR ALTER VIEW v2.vw_TransportLeg_Detail
AS
    SELECT 
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        NULLIF(LTRIM(RTRIM(l.ShipmentTypeCodeT)), N'') AS ShipmentTypeCodeT,

        CAST(l.TransportKind AS INT)    AS TransportKindIdRu,     
        CAST(l.TransportKind AS INT)    AS TransportKindIdEn,     
        tk.Code                         AS TransportKindCode,

        l.SearchTimeT                   AS SearchTimeT,
        l.LoadTimeT                     AS LoadTimeT,
        l.TravelTimeT                   AS TravelTimeT,
        l.DaysWaitingT                  AS DaysWaitingT,
        l.UnLoadTimeT                   AS UnLoadTimeT,
        l.TransportationTimeT           AS TransportationTimeT,

        l.Distance                      AS Distance,
        nf.Code                         AS NodeFromCode,   
        nf.Name_en                      AS NodeFromNameEn, 
        nf.Name_ru                      AS NodeFromNameRu, 
        rf.Code                         AS RegionFromCode,   
        rf.Name_en                      AS RegionFromNameEn, 
        rf.Name_ru                      AS RegionFromNameRu, 

        np.Code                         AS ProxyNodeCode,  
        np.Name_en                      AS ProxyNodeNameEn,
        np.Name_ru                      AS ProxyNodeNameRu,
        rp.Code                         AS ProxyRegionCode,  
        rp.Name_en                      AS ProxyRegionNameEn,
        rp.Name_ru                      AS ProxyRegionNameRu,

        nt.Code                         AS NodeToCode,     
        nt.Name_en                      AS NodeToNameEn,   
        nt.Name_ru                      AS NodeToNameRu,   
        rt.Code                         AS RegionToCode,     
        rt.Name_en                      AS RegionToNameEn,   
        rt.Name_ru                      AS RegionToNameRu,   

        l.CreationDate                  AS CreationDate,
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate,
        l.Leg1_TransportType            AS Leg1_TransportTypeIdRu,
        l.Leg1_TransportType            AS Leg1_TransportTypeIdEn,
        l.Leg1_SearchTime,
        l.Leg1_LoadTime,
        l.Leg1_TravelTime,
        l.Leg1_DaysWaiting,
        l.Leg1_TransportationTime,
        l.Leg1_Distance,
        l.Leg2_TransportType            AS Leg2_TransportTypeIdRu,
        l.Leg2_TransportType            AS Leg2_TransportTypeIdEn,
        l.Leg2_UpLoadTime,
        l.Leg2_TravelTime,
        l.Leg2_DaysWaiting,
        l.Leg2_TransportationTime,
        l.Leg2_Distance
    

    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    LEFT JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    LEFT JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
GO
GO

--------------------------------------------------------------------------------
-- [20/24] Modules\TransportLeg :: vw_TransportLeg_SnapshotSource.sql
-- Source: Modules\TransportLeg\Programmability\vw_TransportLeg_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportLeg из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportLeg_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT для обоих путей гарантирует, что инкремент и полная
    пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportLeg_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение Region/Node -> затронутые плечи). В грид не отдаются.

    ShipmentTypeCodeT — сырой legacy-атрибут (один или несколько кодов
    vw_ShipmentType через '/'). JOIN на vw_ShipmentType невозможен: связь 1-много.
*/

CREATE OR ALTER VIEW v2.vw_TransportLeg_SnapshotSource
AS
    SELECT
        CAST(l.Id AS INT)      AS TransportLegId,
        l.Code                 AS Code,

        CASE WHEN ISNULL(l.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(l.LegIsActive, 0)        AS CanBeUsed,

        LEFT(NULLIF(LTRIM(RTRIM(l.ShipmentTypeCodeT)), N''), 30) AS ShipmentTypeCodeT,

        CAST(l.TransportKind AS INT)    AS TransportKindId,
        LEFT(tk.Code, 5)                AS TransportKindCode,

        LEFT(l.SearchTimeT, 20)         AS SearchTimeT,
        LEFT(l.LoadTimeT, 20)           AS LoadTimeT,
        LEFT(l.TravelTimeT, 20)         AS TravelTimeT,
        LEFT(l.DaysWaitingT, 20)        AS DaysWaitingT,
        LEFT(l.UnLoadTimeT, 20)         AS UnLoadTimeT,
        LEFT(l.TransportationTimeT, 20) AS TransportationTimeT,

        LEFT(nf.Code, 10)               AS NodeFromCode,
        LEFT(nf.Name_en, 30)            AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)            AS NodeFromNameRu,
        LEFT(rf.Code, 10)               AS RegionFromCode,
        LEFT(rf.Name_en, 60)            AS RegionFromNameEn,
        LEFT(rf.Name_ru, 60)            AS RegionFromNameRu,

        LEFT(np.Code, 10)               AS ProxyNodeCode,
        LEFT(np.Name_en, 30)            AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)            AS ProxyNodeNameRu,
        LEFT(rp.Code, 10)               AS ProxyRegionCode,
        LEFT(rp.Name_en, 60)            AS ProxyRegionNameEn,
        LEFT(rp.Name_ru, 60)            AS ProxyRegionNameRu,

        LEFT(nt.Code, 10)               AS NodeToCode,
        LEFT(nt.Name_en, 30)            AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)            AS NodeToNameRu,
        LEFT(rt.Code, 10)               AS RegionToCode,
        LEFT(rt.Name_en, 60)            AS RegionToNameEn,
        LEFT(rt.Name_ru, 60)            AS RegionToNameRu,

        l.CreationDate                  AS CreationDate,
        ISNULL(l.LastChangeDate, l.CreationDate) AS LastChangeDate,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)              AS NodeFromId,
        CAST(nt.Id AS INT)              AS NodeToId,
        CAST(np.Id AS INT)              AS ProxyNodeId,
        CAST(rf.Id AS INT)              AS RegionFromId,
        CAST(rt.Id AS INT)              AS RegionToId,
        CAST(rp.Id AS INT)              AS ProxyRegionId

    FROM vw_TransportLeg l (NOLOCK)
    JOIN vw_TransportKind tk (NOLOCK) ON l.TransportKind = tk.Id

    JOIN vw_LocationsNodes nf (NOLOCK) ON l.NodeFrom = nf.Id
    LEFT JOIN vw_Region rf (NOLOCK) ON rf.Id = nf.Region
    JOIN vw_LocationsNodes nt (NOLOCK) ON l.NodeTo = nt.Id
    LEFT JOIN vw_Region rt (NOLOCK) ON rt.Id = nt.Region
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON l.ProxyNode = np.Id
    LEFT JOIN vw_Region rp (NOLOCK) ON rp.Id = np.Region
GO
GO

--------------------------------------------------------------------------------
-- [21/24] Modules\TransportLeg :: sp_TransportLeg_PopulateAffectedKeys.sql
-- Source: Modules\TransportLeg\Programmability\sp_TransportLeg_PopulateAffectedKeys.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Детекция изменившихся плеч TransportLeg по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (TransportLegId) затронутых плеч.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_2007 — TransportLeg (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1008 — Region  (каскад через RegionFrom/To/Proxy).
      dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад через NodeFrom/To/Proxy).

    TransportKind (2008) стабилен — не отслеживается.
    ShipmentTypeCodeT — сырой multi-code атрибут без FK; каскад по нему не нужен.

    Каскад — UNION эквиджойнов по одной FK-колонке snapshot: позволяет
    использовать NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.TransportLeg_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- TransportLeg (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи плеч.
    IF @Source = N'dbo.PrimitiveEntityData_2007' -- TransportLeg (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2007 d WITH (NOLOCK) -- TransportLeg (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- Region: затронутые плечи через RegionFromId / RegionToId / ProxyRegionId.
    IF @Source = N'dbo.PrimitiveEntityData_1008' -- Region
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportLegId AS EntityKey
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.RegionFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.RegionToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1008 r WITH (NOLOCK) -- Region
            JOIN v2.TransportLeg_Snapshot s ON s.ProxyRegionId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- LocationsNodes: затронутые плечи через NodeFromId / NodeToId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportLegId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportLegId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportLeg_Snapshot s ON s.ProxyNodeId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    ;THROW 50000, 'Unknown TransportLeg sync source.', 1;
END
GO
GO

--------------------------------------------------------------------------------
-- [22/24] Modules\TransportRate :: vw_TransportRate_Detail.sql
-- Source: Modules\TransportRate\Programmability\vw_TransportRate_Detail.sql
--------------------------------------------------------------------------------
GO
USE mdm
GO

/*
Purpose:
  Detail-проекция для UI/full Excel (не snapshot grid).
  Strangler Fig: может присоединять legacy dbo.vw_* / ped.a_* — богаче, чем v2.*_Snapshot.
  Grid list → snapshot; detail/export → этот view ([DetailSource] на detail-DTO).

Example:

use mdm
go

SELECT *
FROM v2.vw_TransportRate_Detail
WHERE TransportRateId = 34041206

*/
DROP VIEW IF EXISTS v2.vw_TransportRateDetail;
GO

DROP FUNCTION IF EXISTS v2.fn_GetTransportRateDetail;
GO

DROP VIEW IF EXISTS v2.vw_TransportRates_Detail;
GO

CREATE OR ALTER VIEW v2.vw_TransportRate_Detail
AS
    SELECT
     CAST(r.Id AS INT)              AS TransportRateId
    ,CAST(r.Code AS BIGINT)	        AS Code
    ,CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END 
                                    AS IsArchive
    ,ISNULL(r.IsDefRate, 0)         AS IsDefRate
    ,r.CreationDate 	            AS CreationDate
    ,r.LastChangeDate 	            AS LastChangeDate
    ,ar.Code		                AS AverageRateCode
    ,ar.RateLevel3 		            AS AverageRateLevel3TotalCostTon
    ,r.TotalCostTon 	            AS TotalCostTon
    ,r.TotalCostTransport 	        AS TotalCostTransport
    ,rct.Code 		                AS CalcType
    ,curSt.Code 		            AS CurrencyStandard
    ,CAST(r.CurrencyRateMonth AS DATE)	        AS CurrencyRateMonth
    ,r.EffectiveLoadOfTransportType AS EffectiveLoadOfTransportType
    ,CAST(r.StartDate AS DATE) 	    AS StartDate
    ,CAST(r.EndDate  AS DATE)	    AS EndDate
    ,rtp.Code		                AS TypeCode
    ,rtp.[Name]		                AS TypeName
    ,r.TotalCostTonRUB 	            AS TotalCostTonRUB
    ,r.TotalCostTonEUR 	            AS TotalCostTonEUR
    ,r.TotalCostTonCNY 	            AS TotalCostTonCNY
    ,r.TotalCostTonUSD 	            AS TotalCostTonUSD
    ,r.TotalCostTransportRUB 	    AS TotalCostTransportRUB
    ,r.TotalCostTransportEUR 	    AS TotalCostTransportEUR
    ,r.TotalCostTransportCNY 	    AS TotalCostTransportCNY
    ,r.TotalCostTransportUSD 	    AS TotalCostTransportUSD
    ,r.EmptyRFSize 	                AS EmptyRFSize
    ,curEmptyRF.Code 		        AS EmptyRFCurrency
    ,r.EmptyCISSize  	            AS EmptyCISSize
    ,curEmptyCIS.Code		        AS EmptyCISCurrency
    ,r.ProvisionTransportSize	    AS ProvisionTransportSize
    ,curProvisionTransport.Code	    AS ProvisionTransportCurrency
    ,r.FerryboatSize	            AS FerryboatSize
    ,curFerryboat.Code		        AS FerryboatCurrency
    ,r.TEFromSize	                AS TEFromSize
    ,curTEFrom.Code		            AS TEFromCurrency
    ,r.PNPFromSize	                AS RatePNPFromSize
    ,curPNPFrom.Code		        AS PNPFromCurrency
    ,r.TEToSize	                    AS TEToSize
    ,curTETo.Code		            AS TEToCurrency
    ,r.PNPToSize	                AS PNPToSize
    ,curPNPTo.Code		            AS PNPToCurrency
    ,r.DrainLoadingSize	            AS DrainLoadingSize
    ,curDrainLoading.Code	        AS DrainLoadingCurrency
    ,r.TransshipmentSize	        AS TransshipmentSize
    ,curTransshipment.Code		    AS TransshipmentCurrency
    ,r.FreightSize	                AS FreightSize
    ,curFreight.Code		        AS FreightCurrency
    ,r.AdditionalFeesCISSize	    AS AdditionalFeesCISSize
    ,curAdditionalFeesCIS.Code	    AS AdditionalFeesCISCurrency
    ,r.LoadedCISSize	            AS LoadedCISSize
    ,curLoadedCIS.Code		        AS LoadedCISCurrency
    ,r.LoadedRFSize	                AS LoadedRFSize
    ,curLoadedRF.Code		        AS LoadedRFCurrency
    ,r.TEFromSize_fix	            AS TEFromSize_fix
    ,curTEFrom_fix.Code		        AS TEFromCurrency_fix
    ,r.TEToSize_fix	                AS TEToSize_fix
    ,curTETo_fix.Code		        AS TEToCurrency_fix
    ,nf.Code		                AS NodeFromCode
    ,nf.Name_ru		                AS NodeFromNameRu
    ,nf.Name_en		                AS NodeFromNameEn
    ,rf.Code		                AS RegionFromCode
    ,rf.Name_ru		                AS RegionFromNameRu
    ,rf.Name_en		                AS RegionFromNameEn
    ,np.Code		                AS ProxyNodeCode
    ,np.Name_ru		                AS ProxyNodeNameRu
    ,np.Name_en		                AS ProxyNodeNameEn
    ,rp.Code		                AS ProxyRegionCode
    ,rp.Name_ru		                AS ProxyRegionNameRu
    ,rp.Name_en		                AS ProxyRegionNameEn
    ,nt.Code		                AS NodeToCode
    ,nt.Name_ru		                AS NodeToNameRu
    ,nt.Name_en		                AS NodeToNameEn
    ,rt.Code		                AS RegionToCode
    ,rt.Name_ru		                AS RegionToNameRu
    ,rt.Name_en		                AS RegionToNameEn
    ,bas.[Name]		                AS Basis
    ,nb.Code		                AS BasisNodeCode
    ,nb.Name_ru		                AS BasisNodeNameRu
    ,tk.Code		                AS TransportKindCode
    ,tk.[Name]		                AS TransportKindNameRu
    ,tk.NameEnRu		            AS TransportKindNameRuEn
    ,tt.Code		                AS TransportTypeCode
    ,tt.[Name]		                AS TransportTypeNameRu
    ,tt.NameEnRu		            AS TransportTypeNameRuEn
    ,pg.Code		                AS ProductGroupCode
    ,pg.NameEnRu 		            AS ProductGroupNameEnRu
    ,p.Code 		                AS ProductCode
    ,p.NameFull_ru		            AS ProductNameRu
    ,p.NameFull_en 		            AS ProductNameEn
    ,p.DPOCode 		                AS ProductDPOCOde
    ,con.Code		                AS ContractorCode
    ,con.NameSearch		            AS ContractorNameSearch
    ,con.ShortNameEGRUL		        AS ContractorEGRUL
    ,r.Nomination	                AS Nomination
    ,tsp.TenderServicePack	        AS TenderServicePack
    ,r.TenderNumber	                AS TenderNumber
    ,r.AdditionalAgreementNumber    AS AdditionalAgreementNumber
    ,r.Comment	                    AS Comment
    ,lg.Code		                AS LegCode
    ,lg.LastChangeDate 		        AS LegChangeDate
    ,lt.LastChangeDate 		        AS LeadTimeChangeDate
    ,lt.StartDate 		            AS LeadTimeStartDate
    ,lt.EndDate		                AS LeadTimeEndDate
    ,lt.Code 		                AS LeadTimeCode
    ,lt.SearchTime 		            AS LeadTimeSearchTime
    ,lt.LoadTime 		            AS LeadTimeLoadTime
    ,lt.TravelTime 		            AS LeadTimeTravelTime
    ,lt.DaysWaiting		            AS LeadTimeDaysWaiting
    ,lt.UnLoadTime 		            AS LeadTimeUnLoadTime
    ,lt.TransportationTime 		    AS LeadTimeTransportationTime
    ,lt.Distance 		            AS LeadTimeDistance
    ,tt1.Code		                AS Leg1_TransportTypeCode
    ,tt1.[Name]		                AS Leg1_TransportTypeNameRu
    ,tt1.NameEnRu		            AS Leg1_TransportTypeNameRuEn
    ,r.Leg1_EffectiveLoad	        AS Leg1_EffectiveLoad
    ,r.Leg1_TotalCostTon	        AS Leg1_TotalCostTon
    ,r.Leg1_TotalCostTransport	    AS Leg1_TotalCostTransport
    ,leg1_cur.Code		            AS Leg1_BaseCurrency
    ,r.Leg1_TotalCostTonRUB	        AS Leg1_TotalCostTonRUB
    ,r.Leg1_TotalCostTonUSD	        AS Leg1_TotalCostTonUSD
    ,r.Leg1_TotalCostTonEUR	        AS Leg1_TotalCostTonEUR
    ,r.Leg1_TotalCostTonCNY	        AS Leg1_TotalCostTonCNY
    ,r.Leg1_TotalCostTransportRUB	AS Leg1_TotalCostTransportRUB
    ,r.Leg1_TotalCostTransportUSD	AS Leg1_TotalCostTransportUSD
    ,r.Leg1_TotalCostTransportEUR	AS Leg1_TotalCostTransportEUR
    ,r.Leg1_TotalCostTransportCNY	AS Leg1_TotalCostTransportCNY
    ,lt.Leg1_SearchTime 		    AS LeadTimeLeg1_SearchTime
    ,lt.Leg1_LoadTime 		        AS LeadTimeLeg1_LoadTime
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg1_TravelTime
    ,lt.Leg1_DaysWaiting 		    AS LeadTimeLeg1_DaysWaiting
    ,lt.Leg1_TransportationTime     AS LeadTimeLeg1_TransportationTime
    ,lt.Leg1_Distance 		        AS LeadTimeLeg1_Distance
    ,tt2.Code		                AS Leg2_TransportTypeCode
    ,tt2.[Name]		                AS Leg2_TransportTypeNameRu
    ,tt2.NameEnRu		            AS Leg2_TransportTypeNameRuEn
    ,r.Leg2_EffectiveLoad	        AS Leg2_EffectiveLoad
    ,r.Leg2_TotalCostTon	        AS Leg2_TotalCostTon
    ,r.Leg2_TotalCostTransport	    AS Leg2_TotalCostTransport
    ,leg2_cur.Code		            AS Leg2_BaseCurrency
    ,r.Leg2_TotalCostTonRUB	        AS Leg2_TotalCostTonRUB
    ,r.Leg2_TotalCostTonUSD	        AS Leg2_TotalCostTonUSD
    ,r.Leg2_TotalCostTonEUR	        AS Leg2_TotalCostTonEUR
    ,r.Leg2_TotalCostTonCNY	        AS Leg2_TotalCostTonCNY
    ,r.Leg2_TotalCostTransportRUB	AS Leg2_TotalCostTransportRUB
    ,r.Leg2_TotalCostTransportUSD	AS Leg2_TotalCostTransportUSD
    ,r.Leg2_TotalCostTransportEUR	AS Leg2_TotalCostTransportEUR
    ,r.Leg2_TotalCostTransportCNY	AS Leg2_TotalCostTransportCNY
    ,lt.Leg1_TravelTime 		    AS LeadTimeLeg2_TravelTime
    ,lt.Leg2_DaysWaiting 		    AS LeadTimeLeg2_DaysWaiting
    ,lt.Leg2_UploadTime 		    AS LeadTimeLeg2_UploadTime
    ,lt.Leg2_TransportationTime 	AS LeadTimeLeg2_TransportationTime
    ,lt.Leg2_Distance 		        AS LeadTimeLeg2_Distance
    FROM vw_TransportRate AS r 
    CROSS APPLY (
        SELECT TOP 1
            rar.AverageRateLevel3Id
        FROM vw_RatesOfAverageRate rar
        WHERE rar.Rate = r.Id
        ORDER BY rar.Id DESC
    ) rar -- чтобы избежать дублирования 

    CROSS APPLY (
        SELECT TOP 1
            ar.Code,
            ar.RateLevel3
        FROM vw_AverageRateLevel3 ar
        WHERE ar.Id = rar.AverageRateLevel3Id
        ORDER BY ar.Id DESC
    ) ar -- чтобы избежать дублирования 
    
    JOIN vw_RateType rtp 
        ON r.RateType = rtp.Id			
			
    JOIN vw_RateCalcType rct 
        ON rct.Id = r.RateCalcType			
    JOIN vw_Currency curSt 
        ON curSt.Id = 	r.CurrencyStandard		
    LEFT JOIN vw_Currency curEmptyRF 
        ON curEmptyRF.Id = 	r.EmptyRFCurrency		
    LEFT JOIN vw_Currency curEmptyCIS 
        ON curEmptyCIS.Id =	r.EmptyCISCurrency		
    LEFT JOIN vw_Currency curProvisionTransport 
        ON curProvisionTransport.Id=	r.ProvisionTransportCurrency		
    LEFT JOIN vw_Currency curFerryboat 
        ON curFerryboat.Id=	r.FerryboatCurrency		
    LEFT JOIN vw_Currency curTEFrom 
        ON curTEFrom.Id=	r.TEFromCurrency		
    LEFT JOIN vw_Currency curPNPFrom 
        ON curPNPFrom.Id=	r.PNPFromCurrency		
    LEFT JOIN vw_Currency curTETo 
        ON curTETo.Id=	r.TEToCurrency		
    LEFT JOIN vw_Currency curPNPTo 
        ON curPNPTo.Id=	r.PNPToCurrency	
    LEFT JOIN vw_Currency curDrainLoading 
        ON curDrainLoading.Id=	r.DrainLoadingCurrency		
    LEFT JOIN vw_Currency curTransshipment 
        ON curTransshipment.Id=	r.TransshipmentCurrency		
    LEFT JOIN vw_Currency curFreight  
        ON curFreight.Id=	r.FreightCurrency		
    LEFT JOIN vw_Currency curAdditionalFeesCIS  
        ON curAdditionalFeesCIS.Id=	r.AdditionalFeesCISCurrency		
    LEFT JOIN vw_Currency curLoadedCIS 
        ON curLoadedCIS.Id=	r.LoadedCISCurrency		
    LEFT JOIN vw_Currency curLoadedRF  
        ON curLoadedRF.Id=	r.LoadedRFCurrency		
    LEFT JOIN vw_Currency curTEFrom_fix 
        ON curTEFrom_fix.Id=	r.TEFromCurrency_fix		
    LEFT JOIN vw_Currency curTETo_fix 
        ON curTETo_fix.Id=	r.TEToCurrency_fix		

 			
    JOIN vw_LocationsNodes nf 
        ON nf.Id = r.NodeFrom			
    LEFT JOIN vw_Region rf 
        ON rf.Id = nf.Region	

    LEFT JOIN vw_LocationsNodes np 
        ON np.Id = r.ProxyNode			
    LEFT JOIN vw_Region rp 
        ON rp.Id = np.Region			

    JOIN vw_LocationsNodes nt 
        ON nt.Id = r.NodeTo			
    LEFT JOIN vw_Region rt 
        ON rt.Id = nt.Region			
			
    LEFT JOIN vw_Basis bas 
        ON bas.Id=r.basis			
    LEFT JOIN vw_LocationsNodes nb 
        ON nb.Id = r.BasisNode			
			
    JOIN vw_TransportKind tk 
        ON tk.Id = r.TransportKind			
			
    JOIN vw_TransportType_level_3 tt 
        ON tt.Id = r.TransportType			
			
    LEFT JOIN vw_ProductGroup pg 
        ON pg.Id = r.ProductGroup			
    LEFT JOIN vw_MTR p  
        ON p.Id = r.Product			
			
			
    LEFT JOIN vw_Contractor con 
        ON con.Id=r.Counterparty			

    CROSS APPLY (
        SELECT TOP 1
            lr.TransportLegId
        FROM vw_LegRate lr
        WHERE lr.Rate = r.Id
        ORDER BY lr.Id DESC
    ) lr -- чтобы избежать дублирования 

    CROSS APPLY (
        SELECT TOP 1
            lg.Id,
            lg.Code,
            lg.LastChangeDate
        FROM vw_TransportLeg lg
        WHERE lg.Id = lr.TransportLegId
        ORDER BY lg.Id DESC
    ) lg -- чтобы избежать дублирования 
    
    OUTER APPLY (
        SELECT TOP (1)
            lit.*
        FROM vw_Leadtime AS lit
        INNER JOIN vw_ShipmentType AS st
            ON st.Id = lit.ShipmentType AND st.IsMain = 1
        WHERE lit.TransportLegId = lg.Id
          AND CAST(r.EndDate AS DATE) >= CAST(lit.StartDate AS DATE)
          AND CAST(lit.EndDate AS DATE) >= CAST(r.StartDate AS DATE)
        ORDER BY lit.PrimitiveEntityDataStateId ASC,
                 lit.StartDate ASC,
                 lit.LastChangeDate DESC
    ) AS lt
			
    LEFT JOIN vw_TransportType_level_3 tt1 
        ON tt1.Id = r.Leg1_TransportType			
    LEFT JOIN vw_TransportType_level_3 tt2 
        ON tt2.Id = r.Leg2_TransportType			
			
    LEFT JOIN vw_Currency leg1_cur 
        ON leg1_cur.Id=	r.Leg1_BaseCurrency		
    LEFT JOIN vw_Currency leg2_cur 
        ON leg2_cur.Id=	r.Leg2_BaseCurrency		

    LEFT JOIN vw_TransportRateTenderServicePack tsp 
        ON r.Id = tsp.TransportRateId
GO
GO

--------------------------------------------------------------------------------
-- [23/24] Modules\TransportRate :: vw_TransportRate_SnapshotSource.sql
-- Source: Modules\TransportRate\Programmability\vw_TransportRate_SnapshotSource.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Проекция snapshot TransportRate из legacy-вью — единый источник истины
    для read-модели. Используется:
      1) первичной полной заливкой (02.TransportRate_Insert.sql);
      2) инкрементальной синхронизацией (MERGE в SnapshotSyncExecutor).

    Один и тот же SELECT (включая WHERE-фильтр членства) для обоих путей
    гарантирует, что инкремент и полная пересборка дают идентичный результат.

    Набор и имена колонок ДОЛЖНЫ совпадать с v2.TransportRate_Snapshot
    (без суррогатного Id — он выдаётся SEQUENCE при вставке).

    Дополнительно к grid-колонкам проекция отдаёт скрытые FK-колонки
    (*_Id) — они нужны только синхронизации для каскадной инвалидации
    (изменение LocationsNodes / ProductGroup / MTR -> затронутые рейты).
    В грид и DTO не отдаются.

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE. Источник MTR (1015) обязателен в каскаде
    процедуры детекции — иначе вход/выход строки отследит только суточный reconcile.
    RateType (2048) — стабильный справочник, в Sources не входит;
    смена членства по RateType ловится только reconcile.
*/

CREATE OR ALTER VIEW v2.vw_TransportRate_SnapshotSource
AS
    SELECT
        CAST(r.Id AS INT)                                   AS TransportRateId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(r.IsDefRate, 0)                              AS IsDefRate,
        r.StartDate                                         AS StartDate,
        r.EndDate                                           AS EndDate,
        r.CreationDate                                      AS CreationDate,
        ISNULL(r.LastChangeDate, r.CreationDate)            AS LastChangeDate,

        r.TotalCostTon                                      AS TotalCostTon,
        r.TotalCostTransport                                AS TotalCostTransport,

        CAST(r.TransportKind AS INT)                        AS TransportKindId,
        CAST(r.TransportType AS INT)                        AS TransportTypeId,
        CAST(r.RateType AS INT)                             AS RateTypeId,
        CAST(r.CurrencyStandard AS INT)                     AS CurrencyId,

        TRY_CAST(LEFT(r.Code, 10) AS INT)                   AS Code,

        TRY_CAST(LEFT(rt.Code, 2) AS INT)                   AS RateTypeCode,
        TRY_CAST(LEFT(p.Code, 7) AS INT)                    AS ProductCode,
        LEFT(cur.Code, 3)                                   AS CurrencyCode,

        LEFT(nf.Code, 10)                                   AS NodeFromCode,
        LEFT(nf.Name_en, 30)                                AS NodeFromNameEn,
        LEFT(nf.Name_ru, 30)                                AS NodeFromNameRu,

        LEFT(np.Code, 10)                                   AS ProxyNodeCode,
        LEFT(np.Name_en, 30)                                AS ProxyNodeNameEn,
        LEFT(np.Name_ru, 30)                                AS ProxyNodeNameRu,

        LEFT(nt.Code, 10)                                   AS NodeToCode,
        LEFT(nt.Name_en, 30)                                AS NodeToNameEn,
        LEFT(nt.Name_ru, 30)                                AS NodeToNameRu,

        LEFT(tk.Code, 5)                                    AS TransportKindCode,
        LEFT(tt.Code, 20)                                   AS TransportTypeCode,

        LEFT(pg.Code, 5)                                    AS ProductGroupCode,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.ShortName, 100) AS ProductGroupNameRu,
        '(' + LEFT(pg.Code, 3) + ') ' + LEFT(pg.NameEn, 100)    AS ProductGroupNameEn,

        LEFT(p.NameShort_ru, 100)                           AS ProductNameRu,
        LEFT(p.NameShort_en, 100)                           AS ProductNameEn,

        -- Скрытые FK для каскадной инвалидации (не в гриде)
        CAST(nf.Id AS INT)                                  AS NodeFromId,
        CAST(nt.Id AS INT)                                  AS NodeToId,
        CAST(np.Id AS INT)                                  AS ProxyNodeId,
        CAST(pg.Id AS INT)                                  AS ProductGroupId,
        CAST(p.Id AS INT)                                   AS ProductId

    FROM vw_TransportRate r (NOLOCK)
    JOIN vw_LocationsNodes nf (NOLOCK) ON r.NodeFrom = nf.Id
    JOIN vw_LocationsNodes nt (NOLOCK) ON r.NodeTo = nt.Id
    LEFT JOIN vw_LocationsNodes np (NOLOCK) ON r.ProxyNode = np.Id
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON r.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id

    WHERE r.TotalCostTon IS NOT NULL
      AND r.TotalCostTransport IS NOT NULL
      AND TRY_CAST(LEFT(r.Code, 10) AS INT) IS NOT NULL
      AND TRY_CAST(LEFT(rt.Code, 2) AS INT) IS NOT NULL
      AND (LEFT(p.Code, 7) IS NULL OR TRY_CAST(LEFT(p.Code, 7) AS INT) IS NOT NULL)
GO
GO

--------------------------------------------------------------------------------
-- [24/24] Modules\TransportRate :: sp_TransportRate_PopulateAffectedKeys.sql
-- Source: Modules\TransportRate\Programmability\sp_TransportRate_PopulateAffectedKeys.sql
--------------------------------------------------------------------------------
GO
USE [mdm];
GO

/*
    Детекция изменившихся рейтов TransportRate по одному источнику за вызов.
    По границам курсора @Lo (эксклюзивно) и @Hi (инклюзивно) добавляет в
    #AffectedKeys бизнес-ключи (TransportRateId) затронутых рейтов.

    Требует, чтобы вызывающая сессия предварительно создала
    #AffectedKeys(EntityKey BIGINT). Идемпотентно в рамках цикла: уже
    добавленные ключи пропускаются (NOT EXISTS по #AffectedKeys).

    Источники (@Source = имя базовой legacy-таблицы, оно же ключ v2.SyncState):
      dbo.PrimitiveEntityData_2012 — TransportRate (основная): сами изменившиеся Id.
      dbo.PrimitiveEntityData_1014 — LocationsNodes (каскад: NodeFrom/To/Proxy).
      dbo.PrimitiveEntityData_1013 — ProductGroup   (каскад: ProductGroupId).
      dbo.PrimitiveEntityData_1015 — MTR/Product     (каскад: ProductId).

    ВНИМАНИЕ: членство строки в snapshot зависит от rt.Code (RateType) и
    p.Code (MTR) в WHERE проекции. Источник 1015 обязателен — иначе
    вход/выход строки из snapshot отследит только суточный reconcile.

    RateType (2048), TransportKind (2008), TransportType (2023) и Currency (2016)
    стабильны — не отслеживаются.

    Каскад — эквиджойн по одной FK-колонке snapshot (для 1014 — UNION по трём):
    позволяет использовать NC-индексы по этим колонкам вместо OR-скана.

    Вызывается из Platform/Sync/SnapshotSyncExecutor (per-source).
*/
CREATE OR ALTER PROCEDURE v2.TransportRate_PopulateAffectedKeys
    @Source NVARCHAR(200),
    @Lo     BINARY(8),
    @Hi     BINARY(8)
AS
BEGIN
    SET NOCOUNT ON;

    -- TransportRate (основная): изменившиеся PrimitiveEntityItemId и есть бизнес-ключи рейтов.
    IF @Source = N'dbo.PrimitiveEntityData_2012' -- TransportRate (основная)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT d.PrimitiveEntityItemId
        FROM dbo.PrimitiveEntityData_2012 d WITH (NOLOCK) -- TransportRate (основная)
        WHERE d.RowVer > @Lo AND d.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = d.PrimitiveEntityItemId);
        RETURN;
    END

    -- LocationsNodes: затронутые рейты через NodeFromId / NodeToId / ProxyNodeId.
    IF @Source = N'dbo.PrimitiveEntityData_1014' -- LocationsNodes
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT x.EntityKey
        FROM (
            SELECT s.TransportRateId AS EntityKey
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.NodeFromId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportRateId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.NodeToId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
            UNION
            SELECT s.TransportRateId
            FROM dbo.PrimitiveEntityData_1014 r WITH (NOLOCK) -- LocationsNodes
            JOIN v2.TransportRate_Snapshot s ON s.ProxyNodeId = r.PrimitiveEntityItemId
            WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
        ) AS x
        WHERE NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = x.EntityKey);
        RETURN;
    END

    -- ProductGroup: затронутые рейты через ProductGroupId.
    IF @Source = N'dbo.PrimitiveEntityData_1013' -- ProductGroup
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.TransportRateId
        FROM dbo.PrimitiveEntityData_1013 r WITH (NOLOCK) -- ProductGroup
        JOIN v2.TransportRate_Snapshot s ON s.ProductGroupId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.TransportRateId);
        RETURN;
    END

    -- MTR/Product: затронутые рейты через ProductId (важно для WHERE-членства).
    IF @Source = N'dbo.PrimitiveEntityData_1015' -- MTR (Product)
    BEGIN
        INSERT INTO #AffectedKeys (EntityKey)
        SELECT DISTINCT s.TransportRateId
        FROM dbo.PrimitiveEntityData_1015 r WITH (NOLOCK) -- MTR (Product)
        JOIN v2.TransportRate_Snapshot s ON s.ProductId = r.PrimitiveEntityItemId
        WHERE r.RowVer > @Lo AND r.RowVer <= @Hi
          AND NOT EXISTS (SELECT 1 FROM #AffectedKeys a WHERE a.EntityKey = s.TransportRateId);
        RETURN;
    END

    ;THROW 50000, 'Unknown TransportRate sync source.', 1;
END
GO
GO

