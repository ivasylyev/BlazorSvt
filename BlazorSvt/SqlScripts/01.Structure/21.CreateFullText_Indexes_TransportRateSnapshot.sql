USE [mdm]
GO
IF EXISTS (
    SELECT 1
    FROM sys.fulltext_indexes fti
    INNER JOIN sys.objects o ON fti.object_id = o.object_id
    WHERE o.object_id = OBJECT_ID(N'dbo.TransportRateSnapshot')
)
BEGIN
    DROP FULLTEXT INDEX ON dbo.TransportRateSnapshot;
END

GO
CREATE FULLTEXT INDEX ON dbo.TransportRateSnapshot 
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
    ProductNameEn      LANGUAGE 1033,      -- English,
    ContractorCode     LANGUAGE 1033,      -- English,
    ContractorEGRUL    
)
KEY INDEX UX_TransportRateSnapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO


ALTER FULLTEXT INDEX ON mdm.dbo.TransportRateSnapshot
SET STOPLIST = OFF;

GO