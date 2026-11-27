namespace BlazorSvt.Modules.Contractor.Detail;

[DetailSource("v2.vw_Contractor_Detail", "ContractorId")]
public class ContractorDetailDto
{
    public long ContractorId { get; set; }
    public bool IsArchive { get; set; }
    public string? CreationUserName { get; set; }
    public DateTime CreationDate { get; set; }
    public string? LastChangeUserName { get; set; }
    public DateTime LastChangeDate { get; set; }
    public required long Code { get; set; }
    public string? NameSearch { get; set; }
    public bool Consignee { get; set; }
    public bool IsTrader { get; set; }
    public bool IsProcessing { get; set; }
    public bool IsSender { get; set; }
    public bool IsDebtor { get; set; }
    public bool IsCreditor { get; set; }
    public string? INN { get; set; }
    public string? ShortNameEGRUL { get; set; }
    public string? FullNameEGRUL { get; set; }
    public string? Customs { get; set; }
    public string? CustomCode { get; set; }
    public string? OrgAffiliation { get; set; }
}
