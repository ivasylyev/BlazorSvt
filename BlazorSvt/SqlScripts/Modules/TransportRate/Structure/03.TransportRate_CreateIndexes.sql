USE [mdm]
GO

SET ANSI_NULLS ON
GO
SET QUOTED_IDENTIFIER ON
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportRateSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportRateSnapshot;
END
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportRate_Snapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportRate_Snapshot;
END
GO



-- Удаляем уникальный индекс для полнтотекстового индекса
DROP INDEX IF EXISTS UX_TransportRateSnapshot_Id ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS UX_TransportRate_Snapshot_Id ON [v2].[TransportRate_Snapshot];
GO

-- Создаем уникальный индекс для полнтотекстового индекса
CREATE UNIQUE NONCLUSTERED INDEX UX_TransportRate_Snapshot_Id
    ON v2.TransportRate_Snapshot (Id)
    ON [PRIMARY];  -- важно: НЕ на partition scheme
GO

-- Создаем полнотекстовый индекс
CREATE FULLTEXT INDEX ON v2.TransportRate_Snapshot
( 
    NodeFromCode       LANGUAGE 1033,      -- English,
    NodeFromNameEn     LANGUAGE 1033,      -- English,
    NodeFromNameRu     LANGUAGE 1049,      -- Russian,
    ProxyNodeCode      LANGUAGE 1033,      -- English,
    ProxyNodeNameEn    LANGUAGE 1033,      -- English,
    ProxyNodeNameRu    LANGUAGE 1049,      -- Russian,
    NodeToCode         LANGUAGE 1033,      -- English,
    NodeToNameEn       LANGUAGE 1033,      -- English,
    NodeToNameRu       LANGUAGE 1049,      -- Russian,
    TransportKindCode  LANGUAGE 1033,      -- English,
    TransportTypeCode  LANGUAGE 1033,      -- English,
    ProductGroupCode   LANGUAGE 1033,      -- English,
    ProductGroupNameRu LANGUAGE 1049,      -- Russian,
    ProductGroupNameEn LANGUAGE 1033,      -- English,
    ProductNameRu      LANGUAGE 1049,      -- Russian,
    ProductNameEn      LANGUAGE 1033      -- English
)
KEY INDEX UX_TransportRate_Snapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO

-- Отключаем стоп-лист для полнотекстового индекса. Если не сделать - слова типа "прочее" не попадут в индекс
ALTER FULLTEXT INDEX ON mdm.v2.TransportRate_Snapshot
SET STOPLIST = OFF;
GO


--  индекс для Кода для активного партишена
DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Code ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS ix_TransportRate_Snapshot_Active_Code ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRate_Snapshot_Active_Code
ON v2.TransportRate_Snapshot (Code)
WHERE IsArchive = 0;
GO

--  индекс для Кода для архивного партишена
DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Code ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS ix_TransportRate_Snapshot_Archive_Code ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRate_Snapshot_Archive_Code
ON v2.TransportRate_Snapshot (Code)
WHERE IsArchive = 1;
GO








--  индекс на Даты 

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Date ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS ix_TransportRate_Snapshot_Active_Date ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRate_Snapshot_Active_Date
ON v2.TransportRate_Snapshot (StartDate, EndDate)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Date ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS ix_TransportRate_Snapshot_Archive_Date ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRate_Snapshot_Archive_Date
ON v2.TransportRate_Snapshot (StartDate, EndDate)
WHERE IsArchive = 1;
GO



--  индекс на Транспорт

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_TransportKindId_TransportTypeId] ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_Active_TransportKindId_TransportTypeId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_Active_TransportKindId_TransportTypeId] ON [v2].TransportRate_Snapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_TransportKindId_TransportTypeId] ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_Archive_TransportKindId_TransportTypeId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_Archive_TransportKindId_TransportTypeId] ON [v2].TransportRate_Snapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 1;
GO


-- индекс на Тип ставки 

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_RateTypeId] ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_Active_RateTypeId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_Active_RateTypeId] ON [v2].TransportRate_Snapshot
(
	[RateTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_RateTypeId] ON [v2].[TransportRateSnapshot];
DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_Archive_RateTypeId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_Archive_RateTypeId] ON [v2].TransportRate_Snapshot
(
	[RateTypeId]
)
WHERE IsArchive = 1;
GO


-- Индекс на бизнес-ключ для синхронизации: upsert (MERGE ON TransportRateId),
-- pre-delete смены партиции и reconciliation ищут строки по ключу без скана таблицы.
DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_TransportRateId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_TransportRateId]
ON v2.TransportRate_Snapshot (TransportRateId);
GO

-- Индексы на скрытые FK-колонки для каскадной синхронизации.
-- По ним SnapshotSyncExecutor находит рейты, затронутые изменением
-- LocationsNodes / ProductGroup / MTR / Currency.
-- TransportKindId / TransportTypeId / RateTypeId уже проиндексированы выше.

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_NodeFromId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_NodeFromId]
ON v2.TransportRate_Snapshot (NodeFromId);
GO

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_NodeToId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_NodeToId]
ON v2.TransportRate_Snapshot (NodeToId);
GO

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_ProxyNodeId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_ProxyNodeId]
ON v2.TransportRate_Snapshot (ProxyNodeId)
WHERE ProxyNodeId IS NOT NULL;
GO

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_ProductGroupId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_ProductGroupId]
ON v2.TransportRate_Snapshot (ProductGroupId)
WHERE ProductGroupId IS NOT NULL;
GO

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_ProductId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_ProductId]
ON v2.TransportRate_Snapshot (ProductId)
WHERE ProductId IS NOT NULL;
GO

DROP INDEX IF EXISTS [ix_TransportRate_Snapshot_CurrencyId] ON [v2].[TransportRate_Snapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRate_Snapshot_CurrencyId]
ON v2.TransportRate_Snapshot (CurrencyId);
GO

UPDATE STATISTICS v2.TransportRate_Snapshot WITH FULLSCAN;
GO
