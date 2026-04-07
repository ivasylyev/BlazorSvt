USE [mdm]
GO



-- Создать новый сразу с несколькими столбцами
CREATE FULLTEXT INDEX ON dbo.TransportRateSnapshot 
( 
    Code               ,

    CurrencyCode       ,
    RateTypeName       ,
    NodeFromCode       ,
    NodeFromNameEn     ,
    NodeFromNameRu     ,
    ProxyNodeCode      ,
    ProxyNodeNameEn    ,
    ProxyNodeNameRu    ,
    NodeToCode         ,
    NodeToNameEn       ,
    NodeToNameRu       ,
    TransportKindCode  ,
    TransportKindNameRu,
    TransportKindNameEn,
    TransportTypeCode  ,
    TransportTypeNameRu,
    TransportTypeNameEn,
    ProductGroupCode   ,
    ProductGroupNameRu ,
    ProductGroupNameEn ,
    ProductCode   ,
    ProductNameRu ,
    ProductNameEn ,
    ContractorCode     ,
    ContractorEGRUL    
)
KEY INDEX PK_TransportRateSnapshot;
GO