USE [mdm]
GO

IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name = N'v2_ftCatalog')
BEGIN
    CREATE FULLTEXT CATALOG v2_ftCatalog AS DEFAULT;
END
GO
