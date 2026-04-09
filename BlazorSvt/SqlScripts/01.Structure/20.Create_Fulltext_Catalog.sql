USE [mdm]
GO

IF EXISTS (
    SELECT 1
    FROM sys.fulltext_catalogs
    WHERE name = N'ftCatalog'
)
BEGIN
    DROP FULLTEXT CATALOG ftCatalog;
END
GO
GO
CREATE FULLTEXT CATALOG ftCatalog AS DEFAULT;

GO
