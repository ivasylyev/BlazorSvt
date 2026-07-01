USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.TransportLeg_ExportFull
    @PageSize = 100,
    @Lang = N'ru',
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
        TransportLegId  INT NOT NULL,
        PRIMARY KEY (TransportLegId)
    );

    INSERT INTO #Filtered (TransportLegId)
    EXEC v2.TransportLeg_Get
        @PageNumber    = 1,
        @PageSize      = @PageSize,
        @Lang          = @Lang,
        @SortKey       = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson    = @FilterJson,
        @KeysOnly      = 1;

    SELECT d.*
    FROM v2.vw_TransportLeg_Detail d
    WHERE d.TransportLegId IN (SELECT TransportLegId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.TransportLegId = d.TransportLegId);
END
GO
