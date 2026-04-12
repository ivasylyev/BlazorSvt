USE [mdm]
GO

DROP TABLE IF EXISTS dbo.LocationNodesSnapshot
GO

CREATE TABLE dbo.LocationNodesSnapshot (
    Id              BIGINT IDENTITY(1,1) NOT NULL,
    IsArchive       BIT NOT NULL,
    NodeTypeId      BIGINT NULL,
    LocationTypeId  BIGINT NULL,
    RegionId        BIGINT NULL,
    CountryId       BIGINT NULL,
    CreationDate    DATETIME NOT NULL,
    LastChangeDate  DATETIME NULL,

    Code            NVARCHAR(10) NOT NULL,
    NameEn          NVARCHAR(100) NULL,
    NameRu          NVARCHAR(100) NULL,
    LocationTypeNameRu NVARCHAR(20) NULL,
    LocationTypeNameEn NVARCHAR(20) NULL,
    NodeTypeNameRu     NVARCHAR(20) NULL,
    NodeTypeNameEn     NVARCHAR(20) NULL,
    RegionNameRu     NVARCHAR(100) NULL,
    RegionNameEn     NVARCHAR(100) NULL,
    CountryNameRu    NVARCHAR(100) NULL,
    CountryNameEn    NVARCHAR(100) NULL,
);

ALTER TABLE dbo.LocationNodesSnapshot ADD  CONSTRAINT [PK_LocationNodesSnapshot] PRIMARY KEY CLUSTERED 
(
	Id ASC
)
GO

ALTER TABLE dbo.LocationNodesSnapshot ADD  CONSTRAINT [DF_LocationNodesSnapshot_CreationDate]  DEFAULT (getdate()) FOR [CreationDate]
GO


