USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.TransportLeg_ExportFull
    @PageSize = 100,
    @TableName = N'mdm.v2.TransportLeg_Snapshot',
    @AllowedColumnsJson = N'[{"ColumnName":"IsArchive","SqlColumnName":"IsArchive","ColumnType":"BIT"}]',
    @SelectList = N'
        SELECT
            TransportLegId
  ',
    @SortKey = N'CreationDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
    ]'

*/
DROP PROCEDURE IF EXISTS v2.ExportTransportLegsFull;
GO

DROP PROCEDURE IF EXISTS v2.TransportLegs_ExportFull;
GO

CREATE OR ALTER PROCEDURE v2.TransportLeg_ExportFull
    @PageSize               INT,
    @TableName              NVARCHAR(300),
    @AllowedColumnsJson     NVARCHAR(MAX),
    @SelectList             NVARCHAR(MAX),
    @SortKey                NVARCHAR(50) = NULL,
    @SortDirection          NVARCHAR(5) = NULL,
    @FilterJson             NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Filtered (
        RowNum  INT IDENTITY(1,1) NOT NULL,
        TransportLegId  INT NOT NULL,
        PRIMARY KEY (TransportLegId)
    );

    INSERT INTO #Filtered (TransportLegId)
    EXEC v2.GetBlazorGridData
        @PageNumber         = 1,
        @PageSize           = @PageSize,
        @TableName          = @TableName,
        @AllowedColumnsJson = @AllowedColumnsJson,
        @SelectList         = @SelectList,
        @SortKey            = @SortKey,
        @SortDirection      = @SortDirection,
        @FilterJson         = @FilterJson;

    SELECT d.*
    FROM v2.vw_TransportLeg_Detail d
    WHERE d.TransportLegId IN (SELECT TransportLegId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.TransportLegId = d.TransportLegId);
END
GO
