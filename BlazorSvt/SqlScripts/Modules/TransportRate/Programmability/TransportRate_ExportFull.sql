USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.TransportRate_ExportFull
    @PageSize = 100,
    @Lang = N'ru',
    @SortKey = N'StartDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertyName":"IsArchive","Value":"False","Operator":"Equals"}
    ]'

*/
DROP PROCEDURE IF EXISTS v2.ExportTransportRatesFull;
GO

DROP PROCEDURE IF EXISTS v2.TransportRates_ExportFull;
GO

CREATE OR ALTER PROCEDURE v2.TransportRate_ExportFull
    @PageSize       INT,
    @Lang           NVARCHAR(2),
    @SortKey        NVARCHAR(50) = NULL,
    @SortDirection  NVARCHAR(5) = NULL,
    @FilterJson     NVARCHAR(MAX) = NULL
AS
BEGIN
    SET NOCOUNT ON;

    CREATE TABLE #Filtered (
        RowNum  INT IDENTITY(1,1) NOT NULL,
        TransportRateId  INT NOT NULL,
        PRIMARY KEY (TransportRateId)
    );

    INSERT INTO #Filtered (TransportRateId)
    EXEC v2.TransportRate_Get
        @PageNumber    = 1,
        @PageSize      = @PageSize,
        @Lang          = @Lang,
        @SortKey       = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson    = @FilterJson,
        @KeysOnly      = 1;

    SELECT d.*
    FROM v2.vw_TransportRate_Detail d
    WHERE d.TransportRateId IN (SELECT TransportRateId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.TransportRateId = d.TransportRateId);
END
GO
