USE [mdm]
GO

IF NOT EXISTS (SELECT 1 FROM sys.fulltext_catalogs WHERE name = N'ftCatalog')
BEGIN
    CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;
END
GO
