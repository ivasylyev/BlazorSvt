USE [mdm]
GO

DROP TABLE IF EXISTS dbo.TransportRateSnapshot
GO

CREATE TABLE dbo.TransportRateSnapshot (
    Id              BIGINT IDENTITY(1,1) NOT NULL,
    IsArchive        BIT NOT NULL,
    IsDefRate       BIT NOT NULL,
    StartDate       DATETIME NOT NULL,
    EndDate         DATETIME NOT NULL,
    CreationDate    DATETIME NOT NULL,
    LastChangeDate  DATETIME NULL,
    TotalCostTon    DECIMAL(15,2) NULL,
    TotalCostTransport DECIMAL(15,2) NULL,

    Code                NVARCHAR(10) NOT NULL,
    RateTypeCode        NVARCHAR(2) NULL,
    RateTypeName        NVARCHAR(20) NULL,
    NodeFromCode        NVARCHAR(10) NULL,
    NodeFromNameEn      NVARCHAR(100) NULL,
    NodeFromNameRu      NVARCHAR(100) NULL,
    ProxyNodeCode       NVARCHAR(10) NULL,
    ProxyNodeNameEn     NVARCHAR(100) NULL,
    ProxyNodeNameRu     NVARCHAR(100) NULL,
    NodeToCode          NVARCHAR(10) NULL,
    NodeToNameEn        NVARCHAR(100) NULL,
    NodeToNameRu        NVARCHAR(100) NULL,
    TransportKindCode   NVARCHAR(20) NULL,
    TransportKindNameRu NVARCHAR(100) NULL,
    TransportTypeCode   NVARCHAR(20) NULL,
    TransportTypeNameRu NVARCHAR(100) NULL,
    ProductGroupCode    NVARCHAR(5) NULL,
    ProductGroupName    NVARCHAR(100) NULL,
    ContractorCode      NVARCHAR(10) NULL,
    ContractorEGRUL     NVARCHAR(1000) NULL,
    CurrencyCode        NVARCHAR(3) NULL,
    CurrencyName        NVARCHAR(50) NULL
);

ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [PK_TransportRateSnapshot] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
GO

ALTER TABLE dbo.TransportRateSnapshot ADD  CONSTRAINT [DF_TransportRateSnapshot_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO

