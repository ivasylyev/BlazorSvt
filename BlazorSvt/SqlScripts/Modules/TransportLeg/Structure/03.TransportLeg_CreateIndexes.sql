USE [mdm]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportLegSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportLegSnapshot;
END
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportLeg_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportLeg_Snapshot;
END
GO



-- Удаляем уникальный индекс для полнтотекстового индекса
DROP INDEX IF EXISTS UX_TransportLegSnapshot_Id ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS UX_TransportLeg_Snapshot_Id ON [v2].[TransportLeg_Snapshot];
GO

-- Создаем уникальный индекс для полнтотекстового индекса
CREATE UNIQUE NONCLUSTERED INDEX UX_TransportLeg_Snapshot_Id
    ON v2.TransportLeg_Snapshot (Id)
    ON [PRIMARY];  -- важно: НЕ на partition scheme
GO

-- Создаем полнотекстовый индекс
CREATE FULLTEXT INDEX ON v2.TransportLeg_Snapshot
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
KEY INDEX UX_TransportLeg_Snapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO

-- Отключаем стоп-лист для полнотекстового индекса. Если не сделать - слова типа "прочее" не попадут в индекс
ALTER FULLTEXT INDEX ON mdm.v2.TransportLeg_Snapshot
SET STOPLIST = OFF;
GO


--  индекс для Кода для активного партишена
DROP INDEX IF EXISTS ix_TransportLegSnapshot_Active_Code ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS ix_TransportLeg_Snapshot_Active_Code ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportLeg_Snapshot_Active_Code
ON v2.TransportLeg_Snapshot (Code)
WHERE IsArchive = 0;
GO

--  индекс для Кода для архивного партишена
DROP INDEX IF EXISTS ix_TransportLegSnapshot_Archive_Code ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS ix_TransportLeg_Snapshot_Archive_Code ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportLeg_Snapshot_Archive_Code
ON v2.TransportLeg_Snapshot (Code)
WHERE IsArchive = 1;
GO





--  индекс на Транспорт

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Active_TransportKindId] ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS [ix_TransportLeg_Snapshot_Active_TransportKindId] ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLeg_Snapshot_Active_TransportKindId] ON [v2].TransportLeg_Snapshot
(
	[TransportKindId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Archive_TransportKindId] ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS [ix_TransportLeg_Snapshot_Archive_TransportKindId] ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLeg_Snapshot_Archive_TransportKindId] ON [v2].TransportLeg_Snapshot
(
	[TransportKindId]
)
WHERE IsArchive = 1;
GO


-- индекс на Тип отгрузки 

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Active_ShipmentTypeId] ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS [ix_TransportLeg_Snapshot_Active_ShipmentTypeId] ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLeg_Snapshot_Active_ShipmentTypeId] ON [v2].TransportLeg_Snapshot
(
	ShipmentTypeId
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportLegSnapshot_Archive_ShipmentTypeId] ON [v2].[TransportLegSnapshot];
DROP INDEX IF EXISTS [ix_TransportLeg_Snapshot_Archive_ShipmentTypeId] ON [v2].[TransportLeg_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportLeg_Snapshot_Archive_ShipmentTypeId] ON [v2].TransportLeg_Snapshot
(
	ShipmentTypeId
)
WHERE IsArchive = 1;
GO


UPDATE STATISTICS v2.TransportLeg_Snapshot WITH FULLSCAN;
GO


