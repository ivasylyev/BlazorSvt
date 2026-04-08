USE [mdm]
GO
DROP FULLTEXT INDEX ON dbo.TransportRateSnapshot
GO
CREATE FULLTEXT INDEX ON dbo.TransportRateSnapshot 
( 
    Code               ,
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
    ProductCode        LANGUAGE 1033,      -- English,
    ProductNameRu      LANGUAGE 1049,      -- Russian,
    ProductNameEn      LANGUAGE 1033,      -- English,
    ContractorCode     LANGUAGE 1033,      -- English,
    ContractorEGRUL    
)
KEY INDEX UX_TransportRateSnapshot_Id
WITH STOPLIST = SYSTEM, 
     CHANGE_TRACKING = AUTO;
GO
