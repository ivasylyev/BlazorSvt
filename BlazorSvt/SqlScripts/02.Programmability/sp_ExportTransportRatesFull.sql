USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.ExportTransportRatesFull
    @PageSize = 100,
    @Lang = N'ru',
    @SortKey = N'StartDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertType":null,"PropertyName":"IsArchive","Value":"False","Operator":1,"StringComparison":5}
    ]'

*/
CREATE OR ALTER PROCEDURE v2.ExportTransportRatesFull
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
        RateId  INT NOT NULL,
        PRIMARY KEY (RateId)
    );

    INSERT INTO #Filtered (RateId)
    EXEC v2.GetTransportRates
        @PageNumber    = 1,
        @PageSize      = @PageSize,
        @Lang          = @Lang,
        @SortKey       = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson    = @FilterJson,
        @KeysOnly      = 1;

    SELECT d.*
    FROM v2.fn_GetTransportRateDetail() d
    WHERE d.RateId IN (SELECT RateId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.RateId = d.RateId);
END
GO
