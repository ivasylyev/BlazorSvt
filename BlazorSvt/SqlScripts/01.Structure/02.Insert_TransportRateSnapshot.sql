
INSERT INTO dbo.TransportRateSnapshot (
    RateId,
    IsArchive,
    IsDefRate,
    StartDate,
    EndDate,
    CreationDate,
    LastChangeDate,

    TotalCostTon,
    TotalCostTransport,

    NodeFromId,          
    ProxyNodeId,         
    NodeToId,            
    TransportKindId,     
    TransportTypeId,     
    ProductGroupId,      
    ProductId, 
    RateTypeId,
    CurrencyId,

    Code,                  

    CurrencyCode,
    RateTypeCode,
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
    TransportTypeCode,
    ProductGroupCode,
    ProductGroupNameRu,
    ProductGroupNameEn,
    ProductCode,
    ProductNameRu,
    ProductNameEn,
    ContractorCode,
    ContractorEGRUL
)
SELECT 
        r.Id        AS RateId,
        CASE WHEN ISNULL(r.PrimitiveEntityDataStateId, 2) = 2 THEN 1 ELSE 0 END AS IsArchive,
        ISNULL(r.IsDefRate, 0) AS IsDefRate,
        r.StartDate,
        r.EndDate,
        r.CreationDate,
        r.LastChangeDate,

        r.TotalCostTon,
        r.TotalCostTransport,

        r.NodeFrom,          
        r.ProxyNode,         
        r.NodeTo,            
        r.TransportKind,     
        r.TransportType,     
        r.ProductGroup,      
        r.Product,     
        r.RateType,
        r.CurrencyStandard,

        LEFT(r.Code, 10),              -- Code (NVARCHAR(10))

        LEFT(cur.Code, 3),             -- CurrencyCode (NVARCHAR(3))
        LEFT(rt.Code, 2),              -- RateTypeCode (NVARCHAR(2)) 
        LEFT(nf.Code, 10),             -- NodeFromCode (NVARCHAR(10))
        LEFT(nf.a_2123, 30),           -- NodeFromNameEn (NVARCHAR(30))
        LEFT(nf.a_1020, 30),           -- NodeFromNameRu (NVARCHAR(30))
        LEFT(np.Code, 10),             -- ProxyNodeCode (NVARCHAR(10))
        LEFT(np.a_2123, 30),           -- ProxyNodeNameEn (NVARCHAR(30))
        LEFT(np.a_1020, 30),           -- ProxyNodeNameRu (NVARCHAR(30))
        LEFT(nt.Code, 10),             -- NodeToCode (NVARCHAR(10))
        LEFT(nt.a_2123, 30),           -- NodeToNameEn (NVARCHAR(30))
        LEFT(nt.a_1020, 30),           -- NodeToNameRu (NVARCHAR(30))
        LEFT(tk.Code, 5),              -- TransportKindCode (NVARCHAR(5))
        LEFT(tt.Code, 20),             -- TransportTypeCode (NVARCHAR(20))
        LEFT(pg.Code, 5),              -- ProductGroupCode (NVARCHAR(5))
        LEFT(pg.[Name], 100),          -- ProductGroupNameRu (NVARCHAR(100))
        LEFT(pg.NameEn, 100),          -- ProductGroupNameEn (NVARCHAR(100))
        LEFT(p.Code, 7),               -- ProductCode (NVARCHAR(7))
        LEFT(p.NameShort_ru, 100),     -- ProductNameRu (NVARCHAR(100))
        LEFT(p.NameShort_en, 100),     -- ProductNameEn (NVARCHAR(100))
        LEFT(cn.Code, 10),             -- ContractorCode (NVARCHAR(10))
        LEFT(cn.ShortNameEGRUL, 20)    -- ContractorEGRUL (NVARCHAR(20))
       
    
    FROM vw_TransportRate r (NOLOCK)
    JOIN PrimitiveEntityData_1014 nf (NOLOCK) ON r.NodeFrom = nf.PrimitiveEntityItemId
    JOIN PrimitiveEntityData_1014 nt (NOLOCK) ON r.NodeTo = nt.PrimitiveEntityItemId
    LEFT JOIN PrimitiveEntityData_1014 np (NOLOCK) ON r.ProxyNode = np.PrimitiveEntityItemId
    LEFT JOIN vw_ProductGroup pg (NOLOCK) ON r.ProductGroup = pg.Id
    LEFT JOIN vw_MTR p (NOLOCK) ON r.Product = p.Id
    JOIN vw_RateType rt (NOLOCK) ON r.RateType = rt.Id
    JOIN vw_TransportKind tk (NOLOCK) ON r.TransportKind = tk.Id
    JOIN vw_TransportType_level_3 tt (NOLOCK) ON r.TransportType = tt.Id
    LEFT JOIN vw_Contractor cn (NOLOCK) ON r.Counterparty = cn.Id
    JOIN vw_Currency cur (NOLOCK) ON r.CurrencyStandard = cur.Id

    where r.TotalCostTon is not null
    and r.TotalCostTransport is not null

