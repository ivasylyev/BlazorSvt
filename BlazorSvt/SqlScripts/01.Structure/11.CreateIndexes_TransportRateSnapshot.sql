USE [mdm]
GO


-- Для полнтотекстового индекса
DROP INDEX IF EXISTS UX_TransportRateSnapshot_Id ON [dbo].[TransportRateSnapshot];
GO

CREATE UNIQUE NONCLUSTERED INDEX UX_TransportRateSnapshot_Id
ON dbo.TransportRateSnapshot (Id)
ON [PRIMARY];  -- важно: НЕ на partition scheme



-- Даты
DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Date ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Active_Date 
ON dbo.TransportRateSnapshot (StartDate, EndDate)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Date ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Archive_Date 
ON dbo.TransportRateSnapshot (StartDate, EndDate)
WHERE IsArchive = 1;
GO


-- Код

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Active_Code ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Active_Code 
ON dbo.TransportRateSnapshot (Code)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS ix_TransportRateSnapshot_Archive_Code ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX ix_TransportRateSnapshot_Archive_Code 
ON dbo.TransportRateSnapshot (Code)
WHERE IsArchive = 1;
GO




-- Транспорт

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_TransportKindId_TransportTypeId] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_TransportKindId_TransportTypeId] ON [dbo].TransportRateSnapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 0;
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_TransportKindId_TransportTypeId] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_TransportKindId_TransportTypeId] ON [dbo].TransportRateSnapshot
(
	[TransportKindId],
    [TransportTypeId]
)
WHERE IsArchive = 1;
GO


--  тип ставки
/*
DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_RateTypeCode] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_RateTypeCode] ON [dbo].TransportRateSnapshot
(
	[RateTypeId]
)
WHERE IsArchive = 0;
GO



DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_RateTypeCode] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_RateTypeCode] ON [dbo].TransportRateSnapshot
(
	[RateTypeId]
)
WHERE IsArchive = 1;
GO

*/

/*

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_CurrencyId_IsArchive] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_CurrencyId_IsArchive] ON [dbo].TransportRateSnapshot
(
	[CurrencyId],
    [IsArchive]
)
GO



-- Группа и продукт

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Active_ProductGroupId_ProductId] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Active_ProductGroupId_ProductId] ON [dbo].TransportRateSnapshot
(
	[ProductGroupId],
    [ProductId]
)
WHERE IsArchive = 0;
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_Archive_ProductGroupId_ProductId] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_Archive_ProductGroupId_ProductId] ON [dbo].TransportRateSnapshot
(
	[ProductGroupId],
    [ProductId]
)
WHERE IsArchive = 1;
GO

*/

-- =====================================================
UPDATE STATISTICS dbo.TransportRateSnapshot WITH FULLSCAN;
GO

