USE [mdm];
GO

IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.Currency_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.Currency_Snapshot;
END
GO

DECLARE @sql_new NVARCHAR(MAX) = N'';
SELECT @sql_new = @sql_new + N'DROP INDEX IF EXISTS [' + name + N'] ON v2.Currency_Snapshot; '
FROM sys.indexes
WHERE object_id = OBJECT_ID('v2.Currency_Snapshot')
  AND name IS NOT NULL
  AND name NOT LIKE 'PK_%';
EXEC sp_executesql @sql_new;
GO

DROP TABLE IF EXISTS v2.Currency_Snapshot;
GO

DROP SEQUENCE IF EXISTS v2.seq_CurrencyId;
GO

IF EXISTS (SELECT * FROM sys.partition_schemes WHERE name = 'v2_ps_Currency')
BEGIN
    DROP PARTITION SCHEME v2_ps_Currency;
END
GO

IF EXISTS (SELECT * FROM sys.partition_functions WHERE name = 'v2_pf_Currency_IsArchive')
BEGIN
    DROP PARTITION FUNCTION v2_pf_Currency_IsArchive;
END
GO

CREATE PARTITION FUNCTION v2_pf_Currency_IsArchive (BIT)
AS RANGE RIGHT FOR VALUES (0);
GO

CREATE PARTITION SCHEME v2_ps_Currency
AS PARTITION v2_pf_Currency_IsArchive
ALL TO ([PRIMARY]);
GO

CREATE SEQUENCE v2.seq_CurrencyId AS INT
START WITH 1
INCREMENT BY 1;
GO

CREATE TABLE v2.Currency_Snapshot (
    Id                  INT NOT NULL,
    CurrencyId          BIGINT NOT NULL,
    IsArchive           BIT NOT NULL,
    Code                NVARCHAR(50) NOT NULL,
    Name                NVARCHAR(4000) NULL,
    Symbolic            NVARCHAR(4000) NULL,
    Digital             DECIMAL(19, 4) NULL,
    Code_NSI            NVARCHAR(50) NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL,

    CONSTRAINT PK_Currency_Snapshot PRIMARY KEY CLUSTERED (IsArchive, Id)
) ON v2_ps_Currency(IsArchive);
GO

ALTER TABLE v2.Currency_Snapshot
ADD CONSTRAINT DF_Currency_Snapshot_Id
DEFAULT (NEXT VALUE FOR v2.seq_CurrencyId) FOR Id;
GO

ALTER TABLE v2.Currency_Snapshot
ADD CONSTRAINT DF_Currency_Snapshot_CreationDate
DEFAULT (GETDATE()) FOR CreationDate;
GO
