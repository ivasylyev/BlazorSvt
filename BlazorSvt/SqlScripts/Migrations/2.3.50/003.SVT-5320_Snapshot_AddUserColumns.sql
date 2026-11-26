USE [mdm];
GO

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DECLARE @Targets TABLE
(
    Entity      SYSNAME NOT NULL,
    KeyColumn   SYSNAME NOT NULL,
    LegacyView  SYSNAME NOT NULL
);

INSERT INTO @Targets (Entity, KeyColumn, LegacyView)
VALUES
    (N'AverageRateLevel3',        N'AverageRateLevel3Id',        N'vw_AverageRateLevel3'),
    (N'Coefficient',              N'CoefficientId',              N'vw_Coefficient'),
    (N'Contractor',               N'ContractorId',               N'vw_LegalType'),
    (N'Country',                  N'CountryId',                  N'vw_Country'),
    (N'Currency',                 N'CurrencyId',                 N'vw_Currency'),
    (N'LocationsNodes',           N'LocationsNodesId',           N'vw_LocationsNodes'),
    (N'MTR',                      N'MTRId',                      N'vw_MTR'),
    (N'ParityRates',              N'ParityRatesId',              N'vw_ParityRates'),
    (N'ProductGroup',             N'ProductGroupId',             N'vw_ProductGroup'),
    (N'RateAutoSelectionSetting', N'RateAutoSelectionSettingId', N'vw_RateAutoSelectionSetting'),
    (N'RateCalcType',             N'RateCalcTypeId',             N'vw_RateCalcType'),
    (N'RateSyncClass',            N'RateSyncClassId',            N'vw_RateSyncClass'),
    (N'RateType',                 N'RateTypeId',                 N'vw_RateType'),
    (N'Region',                   N'RegionId',                   N'vw_Region'),
    (N'Relevance',                N'RelevanceId',                N'vw_Relevance'),
    (N'ReportConstructor',        N'ReportConstructorId',        N'vw_ReportConstructor'),
    (N'TransportKind',            N'TransportKindId',            N'vw_TransportKind'),
    (N'TransportLeg',             N'TransportLegId',             N'vw_TransportLeg'),
    (N'TransportRate',            N'TransportRateId',            N'vw_TransportRate'),
    (N'TransportRoute',           N'TransportRouteId',           N'vw_TransportRoute'),
    (N'TransportTypeLevel3',      N'TransportTypeLevel3Id',      N'vw_TransportType_level_3');

DECLARE
    @BatchSize  BIGINT = 50000,
    @Entity     SYSNAME,
    @KeyColumn  SYSNAME,
    @LegacyView SYSNAME,
    @Table      NVARCHAR(300),
    @ObjectId   INT,
    @Sql        NVARCHAR(MAX),
    @Rows       BIGINT;

DECLARE TargetCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Entity, KeyColumn, LegacyView FROM @Targets;

OPEN TargetCursor;
FETCH NEXT FROM TargetCursor INTO @Entity, @KeyColumn, @LegacyView;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Table = N'v2.' + QUOTENAME(@Entity + N'_Snapshot');
    SET @ObjectId = OBJECT_ID(@Table, N'U');

    IF @ObjectId IS NOT NULL
    BEGIN
        SET @Sql = N'';
        IF COL_LENGTH(@Table, N'CreationUserName') IS NULL
            SET @Sql += N'ALTER TABLE ' + @Table + N' ADD CreationUserName NVARCHAR(255) NULL; ';
        IF COL_LENGTH(@Table, N'LastChangeUserName') IS NULL
            SET @Sql += N'ALTER TABLE ' + @Table + N' ADD LastChangeUserName NVARCHAR(255) NULL; ';
        IF @Sql <> N''
            EXEC sp_executesql @Sql;

        SET @Sql = N'
DECLARE @Archive BIT = 0, @From BIGINT, @Max BIGINT, @Total BIGINT = 0;
WHILE @Archive IS NOT NULL
BEGIN
    SELECT @From = MIN(Id), @Max = MAX(Id) FROM ' + @Table + N' WHERE IsArchive = @Archive;
    WHILE @From <= @Max
    BEGIN
        UPDATE s
        SET s.CreationUserName   = cu.DisplayName,
            s.LastChangeUserName = lu.DisplayName
        FROM ' + @Table + N' s
        JOIN dbo.' + QUOTENAME(@LegacyView) + N' r (NOLOCK) ON r.Id = s.' + QUOTENAME(@KeyColumn) + N'
        LEFT JOIN v2.vw_UserDisplayName cu ON cu.UserId = r.CreationUserId
        LEFT JOIN v2.vw_UserDisplayName lu ON lu.UserId = ISNULL(r.LastChangeUserId, r.CreationUserId)
        WHERE s.IsArchive = @Archive
          AND s.Id >= @From AND s.Id < @From + @BatchSize
          AND (s.CreationUserName IS NULL OR s.LastChangeUserName IS NULL);
        SET @Total += @@ROWCOUNT;
        SET @From += @BatchSize;
    END
    SET @Archive = CASE WHEN @Archive = 0 THEN 1 END;
END
SET @RowsOut = @Total;';

        EXEC sp_executesql @Sql, N'@BatchSize BIGINT, @RowsOut BIGINT OUTPUT', @BatchSize = @BatchSize, @RowsOut = @Rows OUTPUT;

        PRINT @Table + N': CreationUserName / LastChangeUserName, updated ' + CAST(@Rows AS NVARCHAR(20));
    END

    FETCH NEXT FROM TargetCursor INTO @Entity, @KeyColumn, @LegacyView;
END

CLOSE TargetCursor;
DEALLOCATE TargetCursor;

DECLARE @Missing NVARCHAR(MAX);

SELECT @Missing = STRING_AGG(t.Entity, N', ')
FROM @Targets t
WHERE OBJECT_ID(N'v2.' + QUOTENAME(t.Entity + N'_Snapshot'), N'U') IS NOT NULL
  AND (COL_LENGTH(N'v2.' + QUOTENAME(t.Entity + N'_Snapshot'), N'CreationUserName') IS NULL
       OR COL_LENGTH(N'v2.' + QUOTENAME(t.Entity + N'_Snapshot'), N'LastChangeUserName') IS NULL);

IF @Missing IS NOT NULL
BEGIN
    DECLARE @Message NVARCHAR(2048) = N'SVT-5320: no user columns in snapshot: ' + @Missing;
    ;THROW 50000, @Message, 1;
END
GO
