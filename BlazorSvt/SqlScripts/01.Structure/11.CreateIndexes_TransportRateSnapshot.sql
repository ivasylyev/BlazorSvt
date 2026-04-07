USE [mdm]
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_IsArchive_IsDefRate] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_IsArchive_IsDefRate] ON [dbo].TransportRateSnapshot
(
	[IsArchive],
    [IsDefRate]
)
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_code_include_id] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_code_include_id] ON [dbo].TransportRateSnapshot
(
	[Code] ASC,
    [IsArchive]
)
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].[TransportRateSnapshot];
GO

CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeFromNameRu] ASC,
    [IsArchive]
)
GO

--drop  INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot
DROP INDEX IF EXISTS ix_TransportRateSnapshot_NodeToNameRu ON [dbo].[TransportRateSnapshot];
GO


CREATE NONCLUSTERED  INDEX ix_TransportRateSnapshot_NodeToNameRu ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC,
    [IsArchive]
)
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_IsArchive_IsDefRate] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_IsArchive_IsDefRate] ON [dbo].TransportRateSnapshot
(
	[IsArchive],
    [IsDefRate]
)
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_RateTypeCode_IsArchive] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_RateTypeCode_IsArchive] ON [dbo].TransportRateSnapshot
(
	[RateTypeId],
    [IsArchive]
)
GO



DROP INDEX IF EXISTS [ix_TransportRateSnapshot_TransportKindId_TransportTypeId_IsArchive] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_TransportKindId_TransportTypeId_IsArchive] ON [dbo].TransportRateSnapshot
(
	[TransportKindId],
    [TransportTypeId],
    [IsArchive]
)
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_ProductGroupId_ProductId_IsArchive] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_ProductGroupId_ProductId_IsArchive] ON [dbo].TransportRateSnapshot
(
	[ProductGroupId],
    [ProductId],
    [IsArchive]
)
GO

DROP INDEX IF EXISTS [ix_TransportRateSnapshot_CurrencyId_IsArchive] ON [dbo].[TransportRateSnapshot];
GO
CREATE NONCLUSTERED INDEX [ix_TransportRateSnapshot_CurrencyId_IsArchive] ON [dbo].TransportRateSnapshot
(
	[CurrencyId],
    [IsArchive]
)
GO