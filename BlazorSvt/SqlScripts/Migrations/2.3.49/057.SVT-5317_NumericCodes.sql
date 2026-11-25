USE [mdm];
GO

/*
    SVT-5317. Числовые коды в snapshot: NVARCHAR / DECIMAL -> INT.

    Code -> INT NOT NULL, где все коды легаси — целые и помещаются в INT:
    RateSyncClass, ReportConstructor, Coefficient, RateAutoSelectionSetting,
    TransportRoute, RateType, Country, MTR, Contractor.
    Country.ISOCode, Currency.Digital, Currency.Code_NSI -> INT NULL.

    Идемпотентно: колонка, которая уже INT, и отсутствующая таблица пропускаются.
    Колонка выводится из full-text индекса (индекс удаляется, если колонка в нём единственная).
    Фильтрованные индексы ix_{Entity}_Snapshot_Active_Code / _Archive_Code пересоздаются.
    Нецелое значение в колонке остановит скрипт ошибкой конвертации.
    Последний блок проверяет результат: колонка существующей таблицы не INT — THROW.
*/

SET NOCOUNT ON;
SET ANSI_NULLS ON;
SET QUOTED_IDENTIFIER ON;
GO

DECLARE @Targets TABLE
(
    Entity      SYSNAME NOT NULL,
    ColumnName  SYSNAME NOT NULL,
    IsNullable  BIT     NOT NULL
);

INSERT INTO @Targets (Entity, ColumnName, IsNullable)
VALUES
    (N'RateSyncClass',            N'Code',     0),
    (N'ReportConstructor',        N'Code',     0),
    (N'Coefficient',              N'Code',     0),
    (N'RateAutoSelectionSetting', N'Code',     0),
    (N'TransportRoute',           N'Code',     0),
    (N'RateType',                 N'Code',     0),
    (N'Country',                  N'Code',     0),
    (N'MTR',                      N'Code',     0),
    (N'Contractor',               N'Code',     0),
    (N'Country',                  N'ISOCode',  1),
    (N'Currency',                 N'Digital',  1),
    (N'Currency',                 N'Code_NSI', 1);

DECLARE
    @Entity     SYSNAME,
    @ColumnName SYSNAME,
    @IsNullable BIT,
    @Table      NVARCHAR(300),
    @ObjectId   INT,
    @Sql        NVARCHAR(MAX);

DECLARE TargetCursor CURSOR LOCAL FAST_FORWARD FOR
    SELECT Entity, ColumnName, IsNullable FROM @Targets;

OPEN TargetCursor;
FETCH NEXT FROM TargetCursor INTO @Entity, @ColumnName, @IsNullable;

WHILE @@FETCH_STATUS = 0
BEGIN
    SET @Table = N'v2.' + QUOTENAME(@Entity + N'_Snapshot');
    SET @ObjectId = OBJECT_ID(@Table, N'U');

    IF @ObjectId IS NOT NULL
       AND EXISTS (
            SELECT 1 FROM sys.columns
            WHERE object_id = @ObjectId
              AND name = @ColumnName
              AND system_type_id <> TYPE_ID(N'int'))
    BEGIN
        IF COLUMNPROPERTY(@ObjectId, @ColumnName, 'IsFulltextIndexed') = 1
        BEGIN
            IF (SELECT COUNT(*) FROM sys.fulltext_index_columns WHERE object_id = @ObjectId) = 1
                SET @Sql = N'DROP FULLTEXT INDEX ON ' + @Table + N';';
            ELSE
                SET @Sql = N'ALTER FULLTEXT INDEX ON ' + @Table + N' DROP (' + QUOTENAME(@ColumnName) + N');';
            EXEC sp_executesql @Sql;
        END

        SET @Sql = N'';
        SELECT @Sql += N'DROP INDEX ' + QUOTENAME(i.name) + N' ON ' + @Table + N'; '
        FROM sys.indexes i
        WHERE i.object_id = @ObjectId
          AND i.is_primary_key = 0
          AND EXISTS (
                SELECT 1
                FROM sys.index_columns ic
                JOIN sys.columns c ON c.object_id = ic.object_id AND c.column_id = ic.column_id
                WHERE ic.object_id = i.object_id AND ic.index_id = i.index_id AND c.name = @ColumnName);

        SELECT @Sql += N'DROP STATISTICS ' + @Table + N'.' + QUOTENAME(s.name) + N'; '
        FROM sys.stats s
        WHERE s.object_id = @ObjectId
          AND NOT EXISTS (SELECT 1 FROM sys.indexes i WHERE i.object_id = s.object_id AND i.index_id = s.stats_id)
          AND EXISTS (
                SELECT 1
                FROM sys.stats_columns sc
                JOIN sys.columns c ON c.object_id = sc.object_id AND c.column_id = sc.column_id
                WHERE sc.object_id = s.object_id AND sc.stats_id = s.stats_id AND c.name = @ColumnName);

        SET @Sql += N'ALTER TABLE ' + @Table + N' ALTER COLUMN ' + QUOTENAME(@ColumnName)
            + N' INT ' + IIF(@IsNullable = 1, N'NULL', N'NOT NULL') + N';';

        IF @ColumnName = N'Code'
            SET @Sql += N'
CREATE NONCLUSTERED INDEX ' + QUOTENAME(N'ix_' + @Entity + N'_Snapshot_Active_Code') + N'
ON ' + @Table + N' (Code)
WHERE IsArchive = 0;
CREATE NONCLUSTERED INDEX ' + QUOTENAME(N'ix_' + @Entity + N'_Snapshot_Archive_Code') + N'
ON ' + @Table + N' (Code)
WHERE IsArchive = 1;
UPDATE STATISTICS ' + @Table + N' WITH FULLSCAN;';

        EXEC sp_executesql @Sql;

        PRINT @Table + N'.' + @ColumnName + N' -> INT';
    END

    FETCH NEXT FROM TargetCursor INTO @Entity, @ColumnName, @IsNullable;
END

CLOSE TargetCursor;
DEALLOCATE TargetCursor;
GO

DECLARE @NotConverted NVARCHAR(MAX);

SELECT @NotConverted = STRING_AGG(t.name + N'.' + c.name + N' (' + TYPE_NAME(c.system_type_id) + N')', N', ')
FROM sys.tables t
JOIN sys.columns c ON c.object_id = t.object_id
WHERE t.schema_id = SCHEMA_ID(N'v2')
  AND c.system_type_id <> TYPE_ID(N'int')
  AND (
        (c.name = N'Code' AND t.name IN (
            N'RateSyncClass_Snapshot', N'ReportConstructor_Snapshot', N'Coefficient_Snapshot',
            N'RateAutoSelectionSetting_Snapshot', N'TransportRoute_Snapshot', N'RateType_Snapshot',
            N'Country_Snapshot', N'MTR_Snapshot', N'Contractor_Snapshot'))
     OR (c.name = N'ISOCode' AND t.name = N'Country_Snapshot')
     OR (c.name IN (N'Digital', N'Code_NSI') AND t.name = N'Currency_Snapshot')
  );

IF @NotConverted IS NOT NULL
BEGIN
    DECLARE @Message NVARCHAR(2048) = N'SVT-5317: колонки не переведены в INT: ' + @NotConverted;
    ;THROW 50000, @Message, 1;
END

PRINT N'SVT-5317: проверка типов пройдена.';
GO
