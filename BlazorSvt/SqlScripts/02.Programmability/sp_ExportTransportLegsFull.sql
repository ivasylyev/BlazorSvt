USE mdm
GO

/*
Example:

USE mdm
GO

EXEC v2.ExportTransportLegsFull
    @PageSize = 100,
    @Lang = N'ru',
    @SortKey = N'CreationDate',
    @SortDirection = N'ASC',
    @FilterJson = N'[
        {"PropertType":null,"PropertyName":"IsArchive","Value":"False","Operator":1,"StringComparison":5}
    ]'

*/
CREATE OR ALTER PROCEDURE v2.ExportTransportLegsFull
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
        LegId  INT NOT NULL,
        PRIMARY KEY (LegId)
    );

    INSERT INTO #Filtered (LegId)
    EXEC v2.GetTransportLegs
        @PageNumber    = 1,
        @PageSize      = @PageSize,
        @Lang          = @Lang,
        @SortKey       = @SortKey,
        @SortDirection = @SortDirection,
        @FilterJson    = @FilterJson,
        @KeysOnly      = 1;

    SELECT d.*
    FROM v2.fn_GetTransportLegDetail() d
    WHERE d.LegId IN (SELECT LegId FROM #Filtered)
    ORDER BY (SELECT f.RowNum FROM #Filtered f WHERE f.LegId = d.LegId);
END
GO
