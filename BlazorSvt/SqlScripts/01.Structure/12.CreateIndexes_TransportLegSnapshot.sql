USE [mdm]
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('dbo.TransportLegSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON dbo.TransportLegSnapshot;
END
GO



-- Удаляем уникальный индекс для полнтотекстового индекса
DROP INDEX IF EXISTS UX_TransportLegSnapshot_Id ON [dbo].[TransportLegSnapshot];
GO

-- Создаем уникальный индекс для полнтотекстового индекса
CREATE UNIQUE NONCLUSTERED INDEX UX_TransportLegSnapshot_Id
    ON dbo.TransportLegSnapshot (Id)
    ON [PRIMARY];  -- важно: НЕ на partition scheme
GO

-- Создаем полнотекстовый индекс
CREATE FULLTEXT INDEX ON dbo.TransportLegSnapshot 
( 
    Code               LANGUAGE 1033,      -- English,
    NodeFromCode       LANGUAGE 1033,      -- English,
    NodeFromNameEn     LANGUAGE 1033,      -- English,
    NodeFromNameRu     LANGUAGE 1049,      -- Russian,
    RegionFromCode     LANGUAGE 1033,      -- English,
    RegionFromNameEn   LANGUAGE 1033,      -- English,
    RegionFromNameRu   LANGUAGE 1049,      -- Russian,
    ProxyNodeCode      LANGUAGE 1033,      -- English,
    ProxyNodeNameEn    LANGUAGE 1033,      -- English,
    ProxyNodeNameRu    LANGUAGE 1049,      -- Russian,
    ProxyRegionCode    LANGUAGE 1033,      -- English,
    ProxyRegionNameEn  LANGUAGE 1033,      -- English,
    ProxyRegionNameRu  LANGUAGE 1049,      -- Russian,
    NodeToCode         LANGUAGE 1033,      -- English,
    NodeToNameEn       LANGUAGE 1033,      -- English,
    NodeToNameRu       LANGUAGE 1049,      -- Russian,
    RegionToCode       LANGUAGE 1033,      -- English,
    RegionToNameEn     LANGUAGE 1033,      -- English,
    RegionToNameRu     LANGUAGE 1049      -- Russian,
)
KEY INDEX UX_TransportLegSnapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO

-- Отключаем стоп-лист для полнотекстового индекса. Если не сделать - слова типа "прочее" не попадут в индекс
ALTER FULLTEXT INDEX ON mdm.dbo.TransportLegSnapshot
SET STOPLIST = OFF;
GO


--  индекс для Кода для активного партишена
DROP INDEX IF EXISTS ix_TransportLegSnapshot_Active_Code ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportLegSnapshot_Active_Code 
ON dbo.TransportLegSnapshot (Code)
WHERE IsArchive = 0;
GO

--  индекс для Кода для архивного партишена
DROP INDEX IF EXISTS ix_TransportLegSnapshot_Archive_Code ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportLegSnapshot_Archive_Code 
ON dbo.TransportLegSnapshot (Code)
WHERE IsArchive = 1;
GO





--  индекс на Транспорт

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Active_TransportKindId] ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLegSnapshot_Active_TransportKindId] ON [dbo].TransportLegSnapshot
(
	[TransportKindId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Archive_TransportKindId] ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLegSnapshot_Archive_TransportKindId] ON [dbo].TransportLegSnapshot
(
	[TransportKindId]
)
WHERE IsArchive = 1;
GO


-- индекс на Тип отгрузки 

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Active_ShipmentTypeId] ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLegSnapshot_Active_ShipmentTypeId] ON [dbo].TransportLegSnapshot
(
	ShipmentTypeId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Archive_ShipmentTypeId] ON [dbo].[TransportLegSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLegSnapshot_Archive_ShipmentTypeId] ON [dbo].TransportLegSnapshot
(
	ShipmentTypeId
)
WHERE IsArchive = 1;
GO


UPDATE STATISTICS dbo.TransportLegSnapshot WITH FULLSCAN;
GO


