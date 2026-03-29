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
	[Code] ASC
)
--INCLUDE([IsArchive])
GO


DROP INDEX IF EXISTS [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].[TransportRateSnapshot];
GO

CREATE NONCLUSTERED  INDEX [ix_TransportRateSnapshot_NodeFromNameRu] ON [dbo].TransportRateSnapshot
(
	[NodeFromNameRu] ASC
)
--INCLUDE([IsArchive])
GO

--drop  INDEX [ix_TransportRateSnapshot_NodeToNameRu] ON [dbo].TransportRateSnapshot
DROP INDEX IF EXISTS ix_TransportRateSnapshot_NodeToNameRu ON [dbo].[TransportRateSnapshot];
GO


CREATE NONCLUSTERED  INDEX ix_TransportRateSnapshot_NodeToNameRu ON [dbo].TransportRateSnapshot
(
	[NodeToNameRu] ASC
)
--INCLUDE([IsArchive])
GO
