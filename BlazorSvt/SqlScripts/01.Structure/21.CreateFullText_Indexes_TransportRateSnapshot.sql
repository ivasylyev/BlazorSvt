USE [mdm]
GO



-- Создать новый сразу с несколькими столбцами
CREATE FULLTEXT INDEX ON dbo.TransportRateSnapshot 
(
 
    Code               ,
    RateTypeCode       ,
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
    TransportTypeCode  ,
    TransportTypeNameRu,
    ProductGroupCode   ,
    ProductGroupName   ,
    ContractorCode     ,
    ContractorEGRUL    ,
    CurrencyCode       ,
    CurrencyName       


)
KEY INDEX PK_TransportRateSnapshot;
GO