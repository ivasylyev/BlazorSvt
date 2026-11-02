using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Единица измерения (ru). Значения = Id из dbo.vw_Unit. Короткий словарь: в каскад синхронизации не входит.
/// </summary>
public enum UnitRu
{
    [Display(Name = "штука")]
    Piece = 32515,

    [Display(Name = "тонна")]
    Ton = 32516,

    [Display(Name = "килограмм")]
    Kilogram = 32517,

    [Display(Name = "метр")]
    Meter = 32518,

    [Display(Name = "километр")]
    Kilometer = 32519,

    [Display(Name = "литр")]
    Liter = 32520,

    [Display(Name = "пачка")]
    Pack = 32521,

    [Display(Name = "лист")]
    Sheet = 32522,

    [Display(Name = "вагон")]
    Wagon = 32523,

    [Display(Name = "комплект")]
    Set = 32524,

    [Display(Name = "рубль")]
    Ruble = 32525,

    [Display(Name = "метр кубический")]
    CubicMeter = 32526,

    [Display(Name = "метр квадратный")]
    SquareMeter = 32527,

    [Display(Name = "киловатт")]
    Kilowatt = 32528,

    [Display(Name = "пара")]
    Couple = 32529,

    [Display(Name = "кусок")]
    Chunk = 32530,

    [Display(Name = "фасовка")]
    PrePacking = 32531,

    [Display(Name = "набор")]
    Kit = 32532,

    [Display(Name = "коробка")]
    Shell = 32533,

    [Display(Name = "флакон")]
    Flask = 32534,

    [Display(Name = "условная единица")]
    ConventionalUnit = 32535,

    [Display(Name = "ящик")]
    Box = 32536,

    [Display(Name = "банка")]
    Bank = 32537,

    [Display(Name = "контейнер")]
    Container = 32538,

    [Display(Name = "метр погонный")]
    LinearMeter = 32539,

    [Display(Name = "бутылка")]
    Bottle = 32540,

    [Display(Name = "пакет")]
    Package = 32541,

    [Display(Name = "рулон")]
    Roll = 32542,

    [Display(Name = "час")]
    Hour = 32543,

    [Display(Name = "бухта")]
    Bight = 32544,

    [Display(Name = "тысяча штук")]
    ThousandPieces = 32545,

    [Display(Name = "мешок")]
    Bag = 32546,

    [Display(Name = "поддон")]
    Pan = 32547,

    [Display(Name = "тысяча квадратных метров")]
    ThousandSquareMeters = 32548,

    [Display(Name = "упаковка")]
    Packing = 32549,

    [Display(Name = "ампула")]
    Ampoule = 32550,

    [Display(Name = "тысяча метров погонных")]
    ThousandLinearMeters = 32551,

    [Display(Name = "баллон")]
    Balloon = 32552,

    [Display(Name = "канистра")]
    Jerrycan = 32553,

    [Display(Name = "сто листов")]
    OneSheets = 32554,

    [Display(Name = "тюбик")]
    Tube = 32555,

    [Display(Name = "бочка")]
    Barrel = 32556,

    [Display(Name = "ведро")]
    Bucket = 32557,

    [Display(Name = "миллилитр")]
    Milliliter = 32558,

    [Display(Name = "грамм")]
    Gram = 32559,

    [Display(Name = "тысяча рублей")]
    ThousandRubles = 32560,

    [Display(Name = "тысяча метров")]
    ThousandMeters = 32561,

    [Display(Name = "тысяча кубических метров")]
    ThousandCubicMeters = 32562,

    [Display(Name = "полукомплект")]
    HalfSet = 32563,

    [Display(Name = "тонна условного топлива")]
    TonsOfEquivalentFuel = 32564,

    [Display(Name = "килограмм  условного топлива")]
    KilogramOfFuelEquivalent = 32565,

    [Display(Name = "миллиметр")]
    Millimeter = 32566,

    [Display(Name = "контейнер мягкий")]
    SoftContainer = 32567,

    [Display(Name = "гигакалория")]
    GigaCalorie = 32568,

    [Display(Name = "мегакалория")]
    Megacalorie = 32569,

    [Display(Name = "киловатт - час")]
    KilowattHour = 32570,

    [Display(Name = "Мегаватт")]
    Megawatt = 32571,

    [Display(Name = "Декалитр")]
    Decaliter = 32572,

    [Display(Name = "Миллиграмм")]
    Milligram = 32573,

    [Display(Name = "Гигакалория в час")]
    GigacaloriePerHour = 32574,

    [Display(Name = "фунт")]
    Pound = 32575,

    [Display(Name = "Мегаватт-час")]
    MegawattHour = 32576,

}
