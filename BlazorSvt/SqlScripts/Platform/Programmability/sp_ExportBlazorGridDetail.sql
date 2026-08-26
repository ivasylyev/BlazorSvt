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
