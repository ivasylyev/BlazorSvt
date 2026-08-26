USE [mdm];
GO

/*
    Разовый data-fix архивных ParityRates (PrimitiveEntityDataStateId = 2):
    заполнение NULL в обязательных полях, чтобы строки входили в snapshot/грид
    и в легаси можно было повесить NOT NULL constraints.

    Идемпотентен. Не трогает Actual (1) и Deleted (3).
    Не меняет LastChangeDate / LastChangeUserId.
    Не трогает v2.ParityRates_Snapshot — инкрементальный sync подхватит RowVer.

    Константы:
      Relevance          11610185  — «Месяц»
      LocationsNodes     2999951   — «г. Братск»
      TransportType L3   543816    — «Авто фура 20 т»
      ProductGroup       32470     — «Каучуки»
      Currency           32694     — RUB
      StartDate/EndDate  2022-01-01 / 2022-01-31
*/

SET QUOTED_IDENTIFIER ON;
SET ANSI_NULLS ON;
SET XACT_ABORT ON;
SET NOCOUNT ON;

BEGIN TRY
    BEGIN TRANSACTION;

    DECLARE @Missing NVARCHAR(1000) = N'';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Relevance WITH (NOLOCK) WHERE Id = 11610185)
        SET @Missing = @Missing + N'Relevance 11610185 (Месяц); ';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_LocationsNodes WITH (NOLOCK) WHERE Id = 2999951)
        SET @Missing = @Missing + N'LocationsNodes 2999951 (г. Братск); ';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_TransportType_level_3 WITH (NOLOCK) WHERE Id = 543816)
        SET @Missing = @Missing + N'TransportType 543816 (Авто фура 20 т); ';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_ProductGroup WITH (NOLOCK) WHERE Id = 32470)
        SET @Missing = @Missing + N'ProductGroup 32470 (Каучуки); ';

    IF NOT EXISTS (SELECT 1 FROM dbo.vw_Currency WITH (NOLOCK) WHERE Id = 32694)
        SET @Missing = @Missing + N'Currency 32694 (RUB); ';

    IF @Missing <> N''
    BEGIN
        DECLARE @GuardMsg NVARCHAR(1200) = N'Константа не найдена во view: ' + @Missing;
        ;THROW 50000, @GuardMsg, 1;
    END

    UPDATE dbo.vw_ParityRates
    SET
        TotalCostTon = ISNULL(TotalCostTon, 0),
        LoadOfTransport = ISNULL(LoadOfTransport, 0),
        TotalCostTransport = CASE
            WHEN TotalCostTransport IS NOT NULL THEN TotalCostTransport
            WHEN TotalCostTon IS NOT NULL AND LoadOfTransport IS NOT NULL
                THEN ROUND(TotalCostTon * LoadOfTransport, 2)
            ELSE CAST(0 AS DECIMAL(11, 2))
        END,
        Relevance = ISNULL(Relevance, 11610185),
        NodeFromCode = CASE
            WHEN NodeFromCode IS NOT NULL THEN NodeFromCode
            WHEN NodeToCode IS NOT NULL THEN NodeToCode
            ELSE 2999951
        END,
        NodeToCode = CASE
            WHEN NodeToCode IS NOT NULL THEN NodeToCode
            WHEN NodeFromCode IS NOT NULL THEN NodeFromCode
            ELSE 2999951
        END,
        TransportTypeCode = ISNULL(TransportTypeCode, 543816),
        ProductGroupCode = ISNULL(ProductGroupCode, 32470),
        CurrencyStandard = ISNULL(CurrencyStandard, 32694),
        StartDate = ISNULL(StartDate, CONVERT(DATETIME2(7), '20220101', 112)),
        EndDate = ISNULL(EndDate, CONVERT(DATETIME2(7), '20220131', 112))
    WHERE PrimitiveEntityDataStateId = 2
      AND (
            TotalCostTon IS NULL
         OR TotalCostTransport IS NULL
         OR LoadOfTransport IS NULL
         OR Relevance IS NULL
         OR NodeFromCode IS NULL
         OR NodeToCode IS NULL
         OR TransportTypeCode IS NULL
         OR ProductGroupCode IS NULL
         OR CurrencyStandard IS NULL
         OR StartDate IS NULL
         OR EndDate IS NULL
          );

    PRINT CONCAT(N'Updated archival ParityRates rows: ', @@ROWCOUNT);

    COMMIT TRANSACTION;

    -- Контроль: у архивных эти поля должны стать NOT NULL
    SELECT
        SUM(CASE WHEN TotalCostTon IS NULL THEN 1 ELSE 0 END)         AS NullTotalCostTon,
        SUM(CASE WHEN TotalCostTransport IS NULL THEN 1 ELSE 0 END)  AS NullTotalCostTransport,
        SUM(CASE WHEN LoadOfTransport IS NULL THEN 1 ELSE 0 END)     AS NullLoadOfTransport,
        SUM(CASE WHEN Relevance IS NULL THEN 1 ELSE 0 END)           AS NullRelevance,
        SUM(CASE WHEN NodeFromCode IS NULL THEN 1 ELSE 0 END)        AS NullNodeFrom,
        SUM(CASE WHEN NodeToCode IS NULL THEN 1 ELSE 0 END)          AS NullNodeTo,
        SUM(CASE WHEN TransportTypeCode IS NULL THEN 1 ELSE 0 END)   AS NullTransportType,
        SUM(CASE WHEN ProductGroupCode IS NULL THEN 1 ELSE 0 END)    AS NullProductGroup,
        SUM(CASE WHEN CurrencyStandard IS NULL THEN 1 ELSE 0 END)    AS NullCurrency,
        SUM(CASE WHEN StartDate IS NULL THEN 1 ELSE 0 END)           AS NullStartDate,
        SUM(CASE WHEN EndDate IS NULL THEN 1 ELSE 0 END)             AS NullEndDate
    FROM dbo.vw_ParityRates WITH (NOLOCK)
    WHERE PrimitiveEntityDataStateId = 2;
END TRY
BEGIN CATCH
    IF XACT_STATE() <> 0
        ROLLBACK TRANSACTION;

    THROW;
END CATCH
GO
