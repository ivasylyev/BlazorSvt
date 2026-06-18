USE [mdm]
GO

-- Удаляем полнотекстовый индекс
IF EXISTS (SELECT * FROM sys.fulltext_indexes WHERE object_id = OBJECT_ID('v2.TransportRateSnapshot'))
BEGIN
    DROP FULLTEXT INDEX ON v2.TransportRateSnapshot;
END
GO



-- Удаляем уникальный индекс для полнтотекстового индекса
DROP INDEX IF EXISTS UX_TransportRateSnapshot_Id ON [v2].[TransportRateSnapshot];
GO

-- Создаем уникальный индекс для полнтотекстового индекса
CREATE UNIQUE NONCLUSTERED INDEX UX_TransportRateSnapshot_Id
    ON v2.TransportRateSnapshot (Id)
    ON [PRIMARY];  -- важно: НЕ на partition scheme
GO

-- Создаем полнотекстовый индекс
CREATE FULLTEXT INDEX ON v2.TransportRateSnapshot 
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
    ProductGroupCode   LANGUAGE 1033,      -- English,
    ProductGroupNameRu LANGUAGE 1049,      -- Russian,
    ProductGroupNameEn LANGUAGE 1033,      -- English,
    ProductNameRu      LANGUAGE 1049,      -- Russian,
    ProductNameEn      LANGUAGE 1033      -- English
)
KEY INDEX UX_TransportRateSnapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO

-- Отключаем стоп-лист для полнотекстового индекса. Если не сделать - слова типа "прочее" не попадут в индекс
ALTER FULLTEXT INDEX ON mdm.v2.TransportRateSnapshot
SET STOPLIST = OFF;
GO


--  индекс для Кода для активного партишена
DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Code ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Active_Code 
ON v2.TransportRateSnapshot (Code)
WHERE IsArchive = 0;
GO

--  индекс для Кода для архивного партишена
DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Code ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Archive_Code 
ON v2.TransportRateSnapshot (Code)
WHERE IsArchive = 1;
GO








--  индекс на Даты 

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Date ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Active_Date 
ON v2.TransportRateSnapshot (StartDate, EndDate)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Date ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Archive_Date 
ON v2.TransportRateSnapshot (StartDate, EndDate)
WHERE IsArchive = 1;
GO



--  индекс на Транспорт

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_TransportKindId_TransportTypeId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_TransportKindId_TransportTypeId] ON [v2].TransportRateSnapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_TransportKindId_TransportTypeId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_TransportKindId_TransportTypeId] ON [v2].TransportRateSnapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 1;
GO


-- индекс на Тип ставки 

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_RateTypeId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_RateTypeId] ON [v2].TransportRateSnapshot
(
	[RateTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_RateTypeId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_RateTypeId] ON [v2].TransportRateSnapshot
(
	[RateTypeId]
)
WHERE IsArchive = 1;
GO


UPDATE STATISTICS v2.TransportRateSnapshot WITH FULLSCAN;
GO

--  НЕ НАДО создавать индекс на Валюты  - он не селективный и портит планы запроса при использовании полнотекстового индекса 
/*

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_CurrencyId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_CurrencyId] ON [v2].TransportRateSnapshot
(
	[CurrencyId]
)
WHERE IsArchive = 0;
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_CurrencyId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_CurrencyId] ON [v2].TransportRateSnapshot
(
	[CurrencyId]
)
WHERE IsArchive = 1;
GO



--  НЕ НАДО создавать индекс на Группа и продукт  - он ортит планы запроса при использовании полнотекстового индекса 

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_ProductGroupId_ProductId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_ProductGroupId_ProductId] ON [v2].TransportRateSnapshot
(
	[ProductGroupId],
    [ProductId]
)
WHERE IsArchive = 0;
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_ProductGroupId_ProductId] ON [v2].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_ProductGroupId_ProductId] ON [v2].TransportRateSnapshot
(
	[ProductGroupId],
    [ProductId]
)
WHERE IsArchive = 1;
GO

*/

