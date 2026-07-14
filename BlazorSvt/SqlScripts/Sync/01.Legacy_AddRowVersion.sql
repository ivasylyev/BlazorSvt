USE [mdm];
GO

/*
    Инкрементальная синхронизация legacy -> v2 snapshot.

    Добавляет служебный столбец RowVer (rowversion) в базовые таблицы legacy,
    на которых стоят вью справочников. rowversion:
      - поддерживается движком SQL Server (не триггер, не логика в БД);
      - монотонно инкрементится при любом INSERT/UPDATE строки;
      - не может быть задан вручную (legacy-код и генератор метамодели его не трогают).

    Изменение аддитивное и не ломает пайплайн загрузки: все процедуры
    stg.Validate/Preload/Load/PostLoad пишут в эти таблицы только с явным
    списком колонок, а все операции легаси идут через вью (SELECT * из
    базовых таблиц отсутствует).

    ВНИМАНИЕ: ALTER TABLE ... ADD rowversion на таблице с данными — это
    data-size операция под Sch-M локом. Выполнять в maintenance-окне.

    Соответствие "справочник -> базовая таблица" получено из
    stg.LoadTransportLeg / stg.LoadLocationsNodes / stg.LoadTransportRate
    (подзапросы к dbo.PrimitiveEntityData_*).

    Стабильные справочники (TypePlace, TypeNode, TransportKind, ShipmentType,
    RateType, TransportType, Currency) не включены — их изменения не отслеживаются.
*/

DECLARE @tables TABLE (TableName SYSNAME, Comment NVARCHAR(100));

INSERT INTO @tables (TableName, Comment)
VALUES
    -- TransportLeg
    (N'dbo.PrimitiveEntityData_2007', N'TransportLeg (основная)'),
    (N'dbo.PrimitiveEntityData_1014', N'LocationsNodes (Node*/TransportRate/TransportLeg)'),
    (N'dbo.PrimitiveEntityData_1008', N'Region (RegionFrom/To/Proxy)'),
    -- LocationsNodes
    (N'dbo.PrimitiveEntityData_1009', N'Country'),
    -- TransportRate
    (N'dbo.PrimitiveEntityData_2012', N'TransportRate (основная)'),
    (N'dbo.PrimitiveEntityData_1013', N'ProductGroup'),
    (N'dbo.PrimitiveEntityData_1015', N'MTR (Product)'),
    -- AverageRateLevel3
    (N'dbo.PrimitiveEntityData_2057', N'AverageRateLevel3 (основная)');

DECLARE @tableName SYSNAME, @comment NVARCHAR(100), @sql NVARCHAR(MAX);

DECLARE cur CURSOR LOCAL FAST_FORWARD FOR
    SELECT TableName, Comment FROM @tables;

OPEN cur;
FETCH NEXT FROM cur INTO @tableName, @comment;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(@tableName, N'U') IS NULL
    BEGIN
        RAISERROR(N'Таблица %s не найдена — пропуск (%s).', 10, 1, @tableName, @comment);
    END
    ELSE IF NOT EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(@tableName) AND name = N'RowVer')
    BEGIN
        SET @sql = N'ALTER TABLE ' + @tableName + N' ADD RowVer rowversion NOT NULL;';
        PRINT N'Добавляю RowVer в ' + @tableName + N' (' + @comment + N')';
        EXEC sys.sp_executesql @sql;
    END
    ELSE
    BEGIN
        PRINT N'RowVer уже есть в ' + @tableName + N' — пропуск.';
    END

    FETCH NEXT FROM cur INTO @tableName, @comment;
END

CLOSE cur;
DEALLOCATE cur;
GO

/*
    NC-индексы по RowVer только на «горячие» крупные базовые таблицы, чтобы
    детекция изменений (RowVer > @Lo AND RowVer <= @Hi) шла range-seek'ом,
    а не сканом. На остальные источники индекс не ставим — там объём мал.

    Идемпотентно: индекс создаётся только если таблица и колонка RowVer есть,
    а индекса ещё нет.
*/

DECLARE @idxTables TABLE (TableName SYSNAME, IndexName SYSNAME);

INSERT INTO @idxTables (TableName, IndexName)
VALUES
    (N'dbo.PrimitiveEntityData_2012', N'ix_PrimitiveEntityData_2012_RowVer'),  -- TransportRate
    (N'dbo.PrimitiveEntityData_2057', N'ix_PrimitiveEntityData_2057_RowVer'),  -- AverageRateLevel3
    (N'dbo.PrimitiveEntityData_1015', N'ix_PrimitiveEntityData_1015_RowVer'),  -- MTR (Product)
    (N'dbo.PrimitiveEntityData_1014', N'ix_PrimitiveEntityData_1014_RowVer');  -- LocationsNodes

DECLARE @idxTable SYSNAME, @idxName SYSNAME, @idxSql NVARCHAR(MAX);

DECLARE idxCur CURSOR LOCAL FAST_FORWARD FOR
    SELECT TableName, IndexName FROM @idxTables;

OPEN idxCur;
FETCH NEXT FROM idxCur INTO @idxTable, @idxName;

WHILE @@FETCH_STATUS = 0
BEGIN
    IF OBJECT_ID(@idxTable, N'U') IS NULL
    BEGIN
        RAISERROR(N'Таблица %s не найдена — пропуск NC-индекса по RowVer.', 10, 1, @idxTable);
    END
    ELSE IF NOT EXISTS (
        SELECT 1 FROM sys.columns
        WHERE object_id = OBJECT_ID(@idxTable) AND name = N'RowVer')
    BEGIN
        RAISERROR(N'В таблице %s нет колонки RowVer — пропуск NC-индекса.', 10, 1, @idxTable);
    END
    ELSE IF NOT EXISTS (
        SELECT 1 FROM sys.indexes
        WHERE object_id = OBJECT_ID(@idxTable) AND name = @idxName)
    BEGIN
        SET @idxSql = N'CREATE NONCLUSTERED INDEX ' + QUOTENAME(@idxName)
            + N' ON ' + @idxTable + N' (RowVer);';
        PRINT N'Создаю NC-индекс ' + @idxName + N' на ' + @idxTable;
        EXEC sys.sp_executesql @idxSql;
    END
    ELSE
    BEGIN
        PRINT N'NC-индекс ' + @idxName + N' уже есть — пропуск.';
    END

    FETCH NEXT FROM idxCur INTO @idxTable, @idxName;
END

CLOSE idxCur;
DEALLOCATE idxCur;
GO
