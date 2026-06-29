USE [mdm]
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.LocationNodesSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.LocationNodesSnapshot;
END
GO

-- Создаем полнотекстовый индекс
CREATE FULLTEXT INDEX ON v2.LocationNodesSnapshot 
( 
    Code               LANGUAGE 1033,      -- English,
    NameEn             LANGUAGE 1033,      -- English,
    NameRu             LANGUAGE 1049,      -- Russian,
    LocationTypeNameRu LANGUAGE 1049,      -- Russian,
    LocationTypeNameEn LANGUAGE 1033,      -- English,
    NodeTypeNameRu     LANGUAGE 1049,      -- Russian,
    NodeTypeNameEn     LANGUAGE 1033,      -- English,
    RegionNameRu       LANGUAGE 1049,      -- Russian,
    RegionNameEn       LANGUAGE 1033,      -- English,
    CountryNameRu      LANGUAGE 1049,      -- Russian,
    CountryNameEn      LANGUAGE 1033       -- English
)
KEY INDEX PK_LocationNodesSnapshot
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO

-- Отключаем стоп-лист для полнотекстового индекса. Если не сделать - слова типа "прочее" не попадут в индекс
ALTER FULLTEXT INDEX ON mdm.v2.LocationNodesSnapshot
SET STOPLIST = OFF;
GO


--  индекс для Кода для активного партишена
DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Active_Code ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationNodesSnapshot_Active_Code 
ON v2.LocationNodesSnapshot (Code)
WHERE IsArchive = 0;
GO

--  индекс для Кода для архивного партишена
DROP INDEX IF EXISTS ix_LocationNodesSnapshot_Archive_Code ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_LocationNodesSnapshot_Archive_Code 
ON v2.LocationNodesSnapshot (Code)
WHERE IsArchive = 1;
GO


--  индекс на Тип узла

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Active_NodeTypeId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Active_NodeTypeId] ON [v2].LocationNodesSnapshot
(
	[NodeTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Archive_NodeTypeId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Archive_NodeTypeId] ON [v2].LocationNodesSnapshot
(
	[NodeTypeId]
)
WHERE IsArchive = 1;
GO


-- индекс на Тип местоположения 

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Active_LocationTypeId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Active_LocationTypeId] ON [v2].LocationNodesSnapshot
(
	LocationTypeId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Archive_LocationTypeId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Archive_LocationTypeId] ON [v2].LocationNodesSnapshot
(
	LocationTypeId
)
WHERE IsArchive = 1;
GO


-- индекс на Регион

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Active_RegionId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Active_RegionId] ON [v2].LocationNodesSnapshot
(
	RegionId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Archive_RegionId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Archive_RegionId] ON [v2].LocationNodesSnapshot
(
	RegionId
)
WHERE IsArchive = 1;
GO


-- индекс на Страну

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Active_CountryId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Active_CountryId] ON [v2].LocationNodesSnapshot
(
	CountryId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_LocationNodesSnapshot_Archive_CountryId] ON [v2].[LocationNodesSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_LocationNodesSnapshot_Archive_CountryId] ON [v2].LocationNodesSnapshot
(
	CountryId
)
WHERE IsArchive = 1;
GO


UPDATE STATISTICS v2.LocationNodesSnapshot WITH FULLSCAN;
GO

