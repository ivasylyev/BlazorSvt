using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Валюта. Значения = ItemId из legacy <c>dbo.vw_Currency (dbo.PrimitiveEntityData_2016)</c>.
/// Стабильный справочник: не входит в sync-cascade (см. svt-development-patterns).
/// </summary>
public enum CurrencyRuEn
{
    [Display(Name = "CNY")]
    CNY = 32652,

    [Display(Name = "EUR")]
    EUR = 32628,

    [Display(Name = "RUB")]
    RUB = 32694,

    [Display(Name = "USD")]
    USD = 32623,

    [Display(Name = "AED")]
    AED = 32618,

    [Display(Name = "AFN")]
    AFN = 32586,

    [Display(Name = "ALL")]
    ALL = 32580,

    [Display(Name = "AMD")]
    AMD = 32585,

    [Display(Name = "AOA")]
    AOA = 32583,

    [Display(Name = "ARS")]
    ARS = 32584,

    [Display(Name = "ATS")]
    ATS = 32578,

    [Display(Name = "AUD")]
    AUD = 32577,

    [Display(Name = "AZN")]
    AZN = 32579,

    [Display(Name = "BBD")]
    BBD = 32589,

    [Display(Name = "BDT")]
    BDT = 32588,

    [Display(Name = "BEF")]
    BEF = 32593,

    [Display(Name = "BGN")]
    BGN = 32596,

    [Display(Name = "BHD")]
    BHD = 32590,

    [Display(Name = "BIF")]
    BIF = 32601,

    [Display(Name = "BMD")]
    BMD = 32594,

    [Display(Name = "BND")]
    BND = 32600,

    [Display(Name = "BOB")]
    BOB = 32597,

    [Display(Name = "BRL")]
    BRL = 32599,

    [Display(Name = "BSD")]
    BSD = 32587,

    [Display(Name = "BTN")]
    BTN = 32602,

    [Display(Name = "BWP")]
    BWP = 32598,

    [Display(Name = "BYN")]
    BYN = 32741,

    [Display(Name = "BYR")]
    BYR = 32592,

    [Display(Name = "BZD")]
    BZD = 32591,

    [Display(Name = "CAD")]
    CAD = 32644,

    [Display(Name = "CDF")]
    CDF = 32654,

    [Display(Name = "CHF")]
    CHF = 32729,

    [Display(Name = "CLP")]
    CLP = 32727,

    [Display(Name = "COP")]
    COP = 32653,

    [Display(Name = "CRC")]
    CRC = 32655,

    [Display(Name = "CUP")]
    CUP = 32656,

    [Display(Name = "CVE")]
    CVE = 32732,

    [Display(Name = "CYP")]
    CYP = 32650,

    [Display(Name = "CZK")]
    CZK = 32726,

    [Display(Name = "DEM")]
    DEM = 32677,

    [Display(Name = "DJF")]
    DJF = 32720,

    [Display(Name = "DKK")]
    DKK = 32617,

    [Display(Name = "DOP")]
    DOP = 32627,

    [Display(Name = "DZD")]
    DZD = 32581,

    [Display(Name = "ECS")]
    ECS = 32731,

    [Display(Name = "EEK")]
    EEK = 32733,

    [Display(Name = "EGP")]
    EGP = 32629,

    [Display(Name = "ESP")]
    ESP = 32639,

    [Display(Name = "ETB")]
    ETB = 32734,

    [Display(Name = "FIM")]
    FIM = 32719,

    [Display(Name = "FJD")]
    FJD = 32626,

    [Display(Name = "FRF")]
    FRF = 32724,

    [Display(Name = "GBP")]
    GBP = 32582,

    [Display(Name = "GEL")]
    GEL = 32616,

    [Display(Name = "GHS")]
    GHS = 32611,

    [Display(Name = "GIP")]
    GIP = 32613,

    [Display(Name = "GMD")]
    GMD = 32610,

    [Display(Name = "GNF")]
    GNF = 32612,

    [Display(Name = "GRD")]
    GRD = 32615,

    [Display(Name = "GTQ")]
    GTQ = 32647,

    [Display(Name = "GYD")]
    GYD = 32609,

    [Display(Name = "HKD")]
    HKD = 32614,

    [Display(Name = "HNL")]
    HNL = 32659,

    [Display(Name = "HRK")]
    HRK = 32725,

    [Display(Name = "HTG")]
    HTG = 32608,

    [Display(Name = "HUF")]
    HUF = 32604,

    [Display(Name = "IDR")]
    IDR = 32633,

    [Display(Name = "IEP")]
    IEP = 32637,

    [Display(Name = "ILS")]
    ILS = 32631,

    [Display(Name = "INR")]
    INR = 32632,

    [Display(Name = "IQD")]
    IQD = 32635,

    [Display(Name = "IRR")]
    IRR = 32636,

    [Display(Name = "ISK")]
    ISK = 32638,

    [Display(Name = "ITL")]
    ITL = 32640,

    [Display(Name = "JMD")]
    JMD = 32737,

    [Display(Name = "JOD")]
    JOD = 32634,

    [Display(Name = "JPY")]
    JPY = 32738,

    [Display(Name = "KES")]
    KES = 32646,

    [Display(Name = "KGS")]
    KGS = 32651,

    [Display(Name = "KHR")]
    KHR = 32643,

    [Display(Name = "KMF")]
    KMF = 32722,

    [Display(Name = "KPW")]
    KPW = 32697,

    [Display(Name = "KRW")]
    KRW = 32736,

    [Display(Name = "KWD")]
    KWD = 32657,

    [Display(Name = "KYD")]
    KYD = 32621,

    [Display(Name = "KZT")]
    KZT = 32642,

    [Display(Name = "LAK")]
    LAK = 32649,

    [Display(Name = "LBP")]
    LBP = 32660,

    [Display(Name = "LKR")]
    LKR = 32730,

    [Display(Name = "LTL")]
    LTL = 32662,

    [Display(Name = "LUF")]
    LUF = 32663,

    [Display(Name = "LVL")]
    LVL = 32658,

    [Display(Name = "LYD")]
    LYD = 32661,

    [Display(Name = "MAD")]
    MAD = 32672,

    [Display(Name = "MDL")]
    MDL = 32675,

    [Display(Name = "MGA")]
    MGA = 32668,

    [Display(Name = "MKD")]
    MKD = 32666,

    [Display(Name = "MMK")]
    MMK = 32595,

    [Display(Name = "MNT")]
    MNT = 32676,

    [Display(Name = "MRO")]
    MRO = 32665,

    [Display(Name = "MTL")]
    MTL = 32671,

    [Display(Name = "MUR")]
    MUR = 32664,

    [Display(Name = "MVR")]
    MVR = 32670,

    [Display(Name = "MWK")]
    MWK = 32667,

    [Display(Name = "MXN")]
    MXN = 32673,

    [Display(Name = "MYR")]
    MYR = 32669,

    [Display(Name = "MZN")]
    MZN = 32674,

    [Display(Name = "NAD")]
    NAD = 32622,

    [Display(Name = "NGN")]
    NGN = 32679,

    [Display(Name = "NIO")]
    NIO = 32681,

    [Display(Name = "NLG")]
    NLG = 32680,

    [Display(Name = "NOK")]
    NOK = 32686,

    [Display(Name = "NPR")]
    NPR = 32678,

    [Display(Name = "NZD")]
    NZD = 32683,

    [Display(Name = "OMR")]
    OMR = 32687,

    [Display(Name = "PEN")]
    PEN = 32691,

    [Display(Name = "PGK")]
    PGK = 32648,

    [Display(Name = "PHP")]
    PHP = 32718,

    [Display(Name = "PKR")]
    PKR = 32689,

    [Display(Name = "PLN")]
    PLN = 32692,

    [Display(Name = "PTE")]
    PTE = 32693,

    [Display(Name = "PYG")]
    PYG = 32690,

    [Display(Name = "QAR")]
    QAR = 32645,

    [Display(Name = "RON")]
    RON = 32684,

    [Display(Name = "RSD")]
    RSD = 32699,

    [Display(Name = "RWF")]
    RWF = 32723,

    [Display(Name = "SAR")]
    SAR = 32695,

    [Display(Name = "SBD")]
    SBD = 32624,

    [Display(Name = "SCR")]
    SCR = 32698,

    [Display(Name = "SDG")]
    SDG = 32705,

    [Display(Name = "SEK")]
    SEK = 32728,

    [Display(Name = "SGD")]
    SGD = 32700,

    [Display(Name = "SIT")]
    SIT = 32703,

    [Display(Name = "SKK")]
    SKK = 32702,

    [Display(Name = "SLL")]
    SLL = 32707,

    [Display(Name = "SOS")]
    SOS = 32704,

    [Display(Name = "SRD")]
    SRD = 32706,

    [Display(Name = "STD")]
    STD = 32619,

    [Display(Name = "SYP")]
    SYP = 32701,

    [Display(Name = "SZL")]
    SZL = 32696,

    [Display(Name = "THB")]
    THB = 32709,

    [Display(Name = "TJS")]
    TJS = 32708,

    [Display(Name = "TMM")]
    TMM = 32713,

    [Display(Name = "TMT")]
    TMT = 32739,

    [Display(Name = "TND")]
    TND = 32712,

    [Display(Name = "TOP")]
    TOP = 32688,

    [Display(Name = "TRY")]
    TRY = 32682,

    [Display(Name = "TTD")]
    TTD = 32625,

    [Display(Name = "TWD")]
    TWD = 32685,

    [Display(Name = "TZS")]
    TZS = 32711,

    [Display(Name = "UAH")]
    UAH = 32716,

    [Display(Name = "UGX")]
    UGX = 32714,

    [Display(Name = "UYU")]
    UYU = 32717,

    [Display(Name = "UZS")]
    UZS = 32715,

    [Display(Name = "VEF")]
    VEF = 32605,

    [Display(Name = "VND")]
    VND = 32607,

    [Display(Name = "VUV")]
    VUV = 32603,

    [Display(Name = "WST")]
    WST = 32710,

    [Display(Name = "XAF")]
    XAF = 32721,

    [Display(Name = "XCD")]
    XCD = 32606,

    [Display(Name = "XDR")]
    XDR = 32740,

    [Display(Name = "YER")]
    YER = 32641,

    [Display(Name = "ZAR")]
    ZAR = 32735,

    [Display(Name = "ZMK")]
    ZMK = 32630,

    [Display(Name = "ZWD")]
    ZWD = 32620
}
