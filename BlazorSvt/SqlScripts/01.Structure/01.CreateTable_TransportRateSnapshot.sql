USE [mdm]
GO

DROP TABLE IF EXISTS dbo.TransportRateSnapshot
GO

CREATE TABLE dbo.TransportRateSnapshot (
    Id                  BIGINT IDENTITY(1,1) NOT NULL,
    IsArchive           BIT NOT NULL,
    IsDefRate           BIT NOT NULL,
    StartDate           DATETIME NOT NULL,
    EndDate             DATETIME NOT NULL,
    CreationDate        DATETIME NOT NULL,
    LastChangeDate      DATETIME NOT NULL,
    TotalCostTon        DECIMAL(15,2) NOT NULL,
    TotalCostTransport  DECIMAL(15,2) NOT NULL,

    NodeFromId          BIGINT NOT NULL,
    ProxyNodeId         BIGINT NULL,
    NodeToId            BIGINT NOT NULL,
    TransportKindId     BIGINT NOT NULL,
    TransportTypeId     BIGINT NOT NULL,
    ProductGroupId      BIGINT NULL,
    ProductId           BIGINT NULL,
    RateTypeId          BIGINT NOT NULL,
    CurrencyId          BIGINT NOT NULL,

    Code                NVARCHAR(10) NOT NULL,

    CurrencyCode        NVARCHAR(3) NOT NULL,
    RateTypeCode        NVARCHAR(2) NOT NULL,
    --RateTypeName        NVARCHAR(20) NOT NULL,
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
    --TransportKindNameRu NVARCHAR(50) NOT NULL,
    --TransportKindNameEn NVARCHAR(50) NOT NULL,
    TransportTypeCode   NVARCHAR(20) NOT NULL,
    --TransportTypeNameRu NVARCHAR(100) NOT NULL,
    --TransportTypeNameEn NVARCHAR(100) NOT NULL,
    ProductGroupCode    NVARCHAR(5) NULL,
    ProductGroupNameRu  NVARCHAR(100) NULL,
    ProductGroupNameEn  NVARCHAR(100) NULL,
    ProductCode         NVARCHAR(7) NULL,
    ProductNameRu       NVARCHAR(100) NULL,
    ProductNameEn       NVARCHAR(100) NULL,
    ContractorCode      NVARCHAR(10) NULL,
    ContractorEGRUL     NVARCHAR(20) NULL
);

ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [PK_TransportRateSnapshot] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
GO

ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [DF_TransportRateSnapshot_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO

