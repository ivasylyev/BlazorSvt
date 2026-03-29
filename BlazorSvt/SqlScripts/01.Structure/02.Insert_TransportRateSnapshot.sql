SET IDENTITY_INSERT dbo.TransportRateSnapshot ON

INSERT INTO dbo.TransportRateSnapshot (
    Id,
    Code,
    IsDefRate,
    StartDate,
    EndDate,
    CreationDate,
    LastChangeDate,
    TotalCostTon,
    TotalCostTransport,
    IsArchive,
    RateTypeCode,
    RateTypeName,
    NodeFromCode,
    NodeFromNameEn,
    NodeFromNameRu,
    ProxyNodeCode,
    ProxyNodeNameEn,
    ProxyNodeNameRu,
    NodeToCode,
    NodeToNameEn,
    NodeToNameRu,
    TransportKindCode,
    TransportKindNameRu,
    TransportTypeCode,
    TransportTypeNameRu,
    ProductGroupCode,
    ProductGroupName,
    ContractorCode,
    ContractorEGRUL,
    CurrencyCode,
    CurrencyName
)
SELECT 
        r.Id        AS RateId,
        r.Code      AS RateCode,
        ISNULL(r.IsDefRate, 0) AS IsDefRate,
        r.StartDate,
        r.EndDate,
        r.CreationDate,
        r.LastChangeDate,
        r.TotalCostTon,
        r.TotalCostTransport,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END,
        rt.Code     AS RateTypeCode,
        rt.[Name]   AS RateTypeName,
        nf.Code     AS NodeFromCode,
        nf.a_2123   AS NodeFromNameEn,
        nf.a_1020   AS NodeFromNameRu,
        np.Code     AS ProxyNodeCode,   
        np.a_2123   AS ProxyNodeNameEn,
        np.a_1020   AS ProxyNodeNameRu,
        nt.Code     AS NodeToCode,
        nt.a_2123   AS NodeToNameEn,
        nt.a_1020   AS NodeToNameRu,
        tk.Code     AS TransportKindCode,
        tk.[Name]   AS TransportKindNameRu,
        tt.Code     AS TransportTypeCode,
        tt.[Name]   AS TransportTypeNameRu,
        pg.Code     AS ProductGroupCode,
        pg.[Name]   AS ProductGroupName,
        cn.Code     AS ContractorCode,
        cn.ShortNameEGRUL AS ContractorEGRUL,
        cur.Code AS CurrencyCode,
        cur.[Name] AS CurrencyName
    
    FROM vw_TransportRate r (NOLOCK)
    --JOIN FilteredNF nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId
    JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId
    JOIN PrimitiveEntityData_1014 nt (NOLOCK) ON r.NodeTo = nt.PrimitiveEntityItemId
    LEFT JOIN PrimitiveEntityData_1014 np (NOLOCK) ON r.ProxyNode = np.PrimitiveEntityItemId
    --LEFT JOIN FilteredNP np ON r.ProxyNode = np.PrimitiveEntityItemId
    JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    LEFT JOIN vw_Contractor cn (NOLOCK) ON r.Counterparty = cn.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id

    where r.CreationDate is not null 
    and r.StartDate is not null
    and r.EndDate is not null
    and r.TransportKind is not null
    and r.TransportType is not null
    and r.CurrencyStandard is not null
    and r.ProductGroup is not null
    and r.NodeFrom is not null
    and r.NodeTo is not null

SET IDENTITY_INSERT dbo.TransportRateSnapshot OFF