WITH FilteredNP AS (
    SELECT Id
    FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
    WHERE CONTAINS(ProxyNodeNameRu, N'"до*"')
    AND StateId = 1
),
FilteredNF AS (
    SELECT Id
    FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
    WHERE CONTAINS(NodeFromNameRu, N'"тобо*"')
    AND StateId = 1
),
FilteredNT AS (
    SELECT Id
    FROM [mdm].[dbo].[TransportRateSnapshot] (NOLOCK)
    WHERE CONTAINS(NodeToNameRu, N'"ки*"')
    AND StateId = 1
)
SELECT tr.[Id]
      ,[StateId]
      ,[Code]
      ,[IsDefRate]
      ,[StartDate]
      ,[EndDate]
      ,[CreationDate]
      ,[LastChangeDate]
      ,[TotalCostTon]
      ,[TotalCostTransport]
      ,[RateTypeCode]
      ,[RateTypeName]
      ,[NodeFromCode]
      ,[NodeFromNameEn]
      ,[NodeFromNameRu]
      ,[ProxyNodeCode]
      ,[ProxyNodeNameEn]
      ,[ProxyNodeNameRu]
      ,[NodeToCode]
      ,[NodeToNameEn]
      ,[NodeToNameRu]
      ,[TransportKindCode]
      ,[TransportKindNameRu]
      ,[TransportTypeCode]
      ,[TransportTypeNameRu]
      ,[ProductGroupCode]
      ,[ProductGroupName]
      ,[ContractorCode]
      ,[ContractorEGRUL]
      ,[CurrencyCode]
      ,[CurrencyName]
  FROM [mdm].[dbo].[TransportRateSnapshot] tr
  INNER JOIN FilteredNP ON FilteredNP.Id = tr.Id
  INNER JOIN FilteredNF ON FilteredNF.Id = tr.Id
  INNER JOIN FilteredNT ON FilteredNT.Id = tr.Id
  -- WHERE CONTAINS(ProxyNodeNameRu, N'"но*"')
  -- AND CONTAINS([NodeFromNameRu], N'"тобол*"')
 -- where StateId = 1

    ORDER BY ProxyNodeNameRu, Id DESC 
    OFFSET 10 ROWS
    FETCH NEXT 10 ROWS ONLY;