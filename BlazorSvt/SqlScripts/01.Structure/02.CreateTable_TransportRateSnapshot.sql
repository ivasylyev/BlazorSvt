USE [mdm];
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('dbo.TransportRateSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON dbo.TransportRateSnapshot;
END
GO

-- Удаляем индексы (если существуют)
DECLARE @sql NVARCHAR(MAX) = '';
SELECT @sql = @sql + 'DROP INDEX IF EXISTS [' + name + '] ON dbo.TransportRateSnapshot; '
FROM sys.indexes 
WHERE object_id = OBJECT_ID('dbo.TransportRateSnapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';  -- Не удаляем PRIMARY KEY
EXEC sp_executesql @sql;
GO

-- 2. Удаляем таблицу
DROP TABLE IF EXISTS dbo.TransportRateSnapshot;
GO

-- 3. Удаляем SEQUENCE
DROP SEQUENCE IF EXISTS dbo.seq_TransportRateId;
GO

-- 4. Удаляем схему партиционирования
IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'ps_TransportRate')
BEGIN
    DROP PARTITION SCHEME ps_TransportRate;
END
GO

-- 5. Удаляем функцию партиционирования
IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'pf_TransportRate_IsArchive')
BEGIN
    DROP PARTITION FUNCTION pf_TransportRate_IsArchive;
END
GO

-- 6. Создаем функцию партиционирования
CREATE PARTITION FUNCTION pf_TransportRate_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
-- Partition 1: IsArchive < 0 (пусто)
-- Partition 2: IsArchive >= 0 AND < 1 (IsArchive = 0)  
-- Partition 3: IsArchive >= 1 (IsArchive = 1)
GO

-- 7. Создаем схему партиционирования
CREATE PARTITION SCHEME ps_TransportRate
AS PARTITION pf_TransportRate_IsArchive
ALL TO ([PRIMARY]);
GO

-- 8. Создаем SEQUENCE для Id
CREATE SEQUENCE dbo.seq_TransportRateId AS BIGINT 
START WITH 1 
INCREMENT BY 1;
GO

-- 9. Создаем таблицу с партиционированием
CREATE TABLE dbo.TransportRateSnapshot (
    Id                  INT NOT NULL,
    RateId              INT NOT NULL,
    IsArchive           BIT NOT NULL,
    IsDefRate           BIT NOT NULL,
    StartDate           DATETIME NOT NULL,
    EndDate             DATETIME NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL,
    TotalCostTon        DECIMAL(15,2) NOT NULL,
    TotalCostTransport  DECIMAL(15,2) NOT NULL,
    TransportKindId     INT NOT NULL,
    TransportTypeId     INT NOT NULL,
    ProductGroupId      INT NULL,
    ProductId           INT NULL,
    RateTypeId          INT NOT NULL,
    CurrencyId          INT NOT NULL,
    Code                INT NOT NULL,
    RateTypeCode        INT NOT NULL,
    ProductCode         INT NULL,
    CurrencyCode        NVARCHAR(3) NOT NULL,
    NodeFromCode        NVARCHAR(10) NOT NULL,
    NodeFromNameEn      NVARCHAR(30) NOT NULL,
    NodeFromNameRu      NVARCHAR(30) NOT NULL,
    ProxyNodeCode       NVARCHAR(10) NULL,
    ProxyNodeNameEn     NVARCHAR(30) NULL,
    ProxyNodeNameRu     NVARCHAR(30) NULL,
    NodeToCode          NVARCHAR(10) NOT NULL,
    NodeToNameEn        NVARCHAR(30) NOT NULL,
    NodeToNameRu        NVARCHAR(30) NOT NULL,
    TransportKindCode   NVARCHAR(5) NOT NULL,
    TransportTypeCode   NVARCHAR(20) NOT NULL,
    ProductGroupCode    NVARCHAR(5) NULL,
    ProductGroupNameRu  NVARCHAR(110) NULL,
    ProductGroupNameEn  NVARCHAR(110) NULL,
    ProductNameRu       NVARCHAR(100) NULL,
    ProductNameEn       NVARCHAR(100) NULL
    
    CONSTRAINT PK_TransportRateSnapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON ps_TransportRate(IsArchive);
GO

-- =====================================================
-- 10. Добавляем DEFAULT constraints
-- =====================================================
ALTER TABLE dbo.TransportRateSnapshot 
ADD CONSTRAINT DF_TransportRateSnapshot_Id 
DEFAULT (NEXT VALUE FOR dbo.seq_TransportRateId) FOR Id;
GO

ALTER TABLE dbo.TransportRateSnapshot 
ADD CONSTRAINT DF_TransportRateSnapshot_CreationDate 
DEFAULT (GETDATE()) FOR CreationDate;
GO