USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.LocationsNodes_ExportFull
    @PageSize = 100,
    @Lang = N'ru',
    @SortKey = N'CreationDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
    ]'

*/
DROP PROCEDURE IF EXISTS v2.ExportLocationsNodesFull;
GO

DROP PROCEDURE IF EXISTS v2.LocationNodes_ExportFull;
GO

CREATE OR ALTER PROCEDURE v2.LocationsNodes_ExportFull
    @PageSize       INT,
    @Lang           NVARCHAR(2),
    @SortKey        NVARCHAR(50) = NULL,
    @SortDirection  NVARCHAR(5) = NULL,
    @FilterJson     NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Filtered (
        RowNum INT IDENTITY(1,1) NOT NULL,
        LocationsNodesId BIGINT NOT NULL,
        PRIMARY KEY (LocationsNodesId)
    );

    INSERT INTO #Filtered (LocationsNodesId)
    EXEC v2.LocationsNodes_Get
        @PageNumber    = 1,
        @PageSize      = @PageSize,
        @Lang          = @Lang,
        @SortKey       = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson    = @FilterJson,
        @KeysOnly      = 1;

    SELECT d.*
    FROM v2.vw_LocationsNodes_Detail d
    WHERE d.LocationsNodesId IN (SELECT LocationsNodesId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.LocationsNodesId = d.LocationsNodesId);
END
GO
