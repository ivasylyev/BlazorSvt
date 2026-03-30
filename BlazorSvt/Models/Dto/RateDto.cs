namespace BlazorSvt.Models.Dto
{
    public class RateDto
    {
        public string? Code { get; set; }
        public bool IsDefRate { get; set; }
        public string? RateTypeCode { get; set; }
        public string? RateTypeName { get; set; }

        public string? NodeFromCode { get; set; }
        public string? NodeFromName { get; set; }

        public string? ProxyNodeCode { get; set; }
        public string? ProxyNodeName { get; set; }

        public string? NodeToCode { get; set; }
        public string? NodeToName { get; set; }

        public string? TransportKindName { get; set; }
        public string? TransportTypeName { get; set; }

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
