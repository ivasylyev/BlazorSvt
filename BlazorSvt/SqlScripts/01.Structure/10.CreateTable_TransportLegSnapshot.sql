USE [mdm];
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportLegSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportLegSnapshot;
END
GO

-- Удаляем индексы (если существуют)
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'DROP INDEX IF EXISTS [' + name + '] ON v2.TransportLegSnapshot; '
FROM sys.indexes 
WHERE object_id = OBJECT_ID('v2.TransportLegSnapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';  -- Не удаляем PRIMARY KEY
EXEC sp_executesql @sql;
GO

-- 2. Удаляем таблицу
DROP TABLE IF EXISTS v2.TransportLegSnapshot;
GO

-- 3. Удаляем SEQUENCE
DROP SEQUENCE IF EXISTS v2.seq_TransportLegId;
GO

-- 4. Удаляем схему партиционирования
IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_TransportLeg')
BEGIN
    DROP PARTITION SCHEME v2_ps_TransportLeg;
END
GO

-- 5. Удаляем функцию партиционирования
IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_TransportLeg_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_TransportLeg_IsArchive;
END
GO

-- 6. Создаем функцию партиционирования
CREATE PARTITION FUNCTION v2_pf_TransportLeg_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
-- Partition 1: IsArchive < 0 (пусто)
-- Partition 2: IsArchive >= 0 AND < 1 (IsArchive = 0)  
-- Partition 3: IsArchive >= 1 (IsArchive = 1)
GO

-- 7. Создаем схему партиционирования
CREATE PARTITION SCHEME v2_ps_TransportLeg
AS PARTITION v2_pf_TransportLeg_IsArchive
ALL TO ([PRIMARY]);
GO

-- 8. Создаем SEQUENCE для Id
CREATE SEQUENCE v2.seq_TransportLegId AS INT 
START WITH 1 
INCREMENT BY 1;
GO

-- 9. Создаем таблицу с партиционированием
CREATE TABLE v2.TransportLegSnapshot (
    Id                  INT NOT NULL,
    LegId               INT NOT NULL,
    Code                NVARCHAR(50) NOT NULL,

    IsArchive           BIT NOT NULL,
    CanBeUsed           BIT NOT NULL, -- активное плечо (то есть используется) или нет. Не то же самое что IsArchive!

    ShipmentTypeId      INT NOT NULL,

    TransportKindId     INT NOT NULL,
    TransportKindCode   NVARCHAR(5) NOT NULL,
    
    SearchTimeT         NVARCHAR(20) NULL,
    LoadTimeT           NVARCHAR(20) NULL,
    TravelTimeT         NVARCHAR(20) NOT NULL,
    DaysWaitingT        NVARCHAR(20) NULL,
    UnLoadTimeT         NVARCHAR(20) NULL,
    TransportationTimeT NVARCHAR(20) NOT NULL,

    NodeFromCode        NVARCHAR(10) NOT NULL,
    NodeFromNameEn      NVARCHAR(30) NOT NULL,
    NodeFromNameRu      NVARCHAR(30) NOT NULL,
    RegionFromCode      NVARCHAR(10) NOT NULL,
    RegionFromNameEn    NVARCHAR(60) NOT NULL,
    RegionFromNameRu    NVARCHAR(60) NOT NULL,

    ProxyNodeCode       NVARCHAR(10) NULL,
    ProxyNodeNameEn     NVARCHAR(30) NULL,
    ProxyNodeNameRu     NVARCHAR(30) NULL,
    ProxyRegionCode     NVARCHAR(10) NULL,
    ProxyRegionNameEn   NVARCHAR(60) NULL,
    ProxyRegionNameRu   NVARCHAR(60) NULL,

    NodeToCode          NVARCHAR(10) NOT NULL,
    NodeToNameEn        NVARCHAR(30) NOT NULL,
    NodeToNameRu        NVARCHAR(30) NOT NULL,
    RegionToCode        NVARCHAR(10) NOT NULL,
    RegionToNameEn      NVARCHAR(60) NOT NULL,
    RegionToNameRu      NVARCHAR(60) NOT NULL,
    
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL
    
    CONSTRAINT PK_TransportLegSnapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_TransportLeg(IsArchive);
GO

-- 10. Добавляем DEFAULT constraints
ALTER TABLE v2.TransportLegSnapshot 
ADD CONSTRAINT DF_TransportLegSnapshot_Id 
DEFAULT (NEXT VALUE FOR v2.seq_TransportLegId) FOR Id;
GO

ALTER TABLE v2.TransportLegSnapshot 
ADD CONSTRAINT DF_TransportLegSnapshot_CreationDate 
DEFAULT (GETDATE()) FOR CreationDate;
GO