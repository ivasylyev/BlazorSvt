namespace BlazorSvt.Models.Dto
{
    public class RateDto
    {
        public string? Code { get; set; }
        public bool IsDefRate { get; set; }
        public string? RateTypeCode { get; set; }
        public string? RateTypeName { get; set; }

        public string? NodeFromCode { get; set; }
        public string? NodeFromNameEn { get; set; }
        public string? NodeFromNameRu { get; set; }

        public string? ProxyNodeCode { get; set; }
        public string? ProxyNodeNameEn { get; set; }
        public string? ProxyNodeNameRu { get; set; }

        public string? NodeToCode { get; set; }
        public string? NodeToNameEn { get; set; }
        public string? NodeToNameRu { get; set; }

        public string? TransportKindNameRu { get; set; }
        public string? TransportTypeNameRu { get; set; }

        public DateOnly StartDate { get; set; }
        public DateOnly EndDate { get; set; }
        public DateTime CreationDate { get; set; }
        public DateTime? LastChangeDate { get; set; }

        public string? ProductGroupCode { get; set; }
        public string? ProductGroupName { get; set; }

        public string ShortNameEGRUL { get; set; }

        public decimal TotalCostTon { get; set; }
        public decimal TotalCostTransport { get; set; }

        public string? CurrencyCode { get; set; }
        public string? CurrencyName { get; set; }

        public bool IsArchive { get; set; }
    }

}
