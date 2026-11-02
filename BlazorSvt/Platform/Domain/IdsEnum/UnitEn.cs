using System.ComponentModel.DataAnnotations;

namespace BlazorSvt.Platform.Domain.IdsEnum;

/// <summary>
/// Unit of measurement (en). Values = Id from dbo.vw_Unit. Short dictionary: not part of the sync cascade.
/// </summary>
public enum UnitEn
{
    [Display(Name = "piece")]
    Piece = 32515,

    [Display(Name = "ton")]
    Ton = 32516,

    [Display(Name = "kilogram")]
    Kilogram = 32517,

    [Display(Name = "meter")]
    Meter = 32518,

    [Display(Name = "kilometer")]
    Kilometer = 32519,

    [Display(Name = "liter")]
    Liter = 32520,

    [Display(Name = "pack")]
    Pack = 32521,

    [Display(Name = "sheet")]
    Sheet = 32522,

    [Display(Name = "wagon")]
    Wagon = 32523,

    [Display(Name = "set")]
    Set = 32524,

    [Display(Name = "ruble")]
    Ruble = 32525,

    [Display(Name = "cubic meter")]
    CubicMeter = 32526,

    [Display(Name = "square meter")]
    SquareMeter = 32527,

    [Display(Name = "kilowatt")]
    Kilowatt = 32528,

    [Display(Name = "couple")]
    Pair = 32529,

    [Display(Name = "chunk")]
    Chunk = 32530,

    [Display(Name = "pre-packing")]
    PrePacking = 32531,

    [Display(Name = "kit")]
    Kit = 32532,

    [Display(Name = "shell")]
    Carton = 32533,

    [Display(Name = "flask")]
    Flask = 32534,

    [Display(Name = "conventional unit")]
    ConventionalUnit = 32535,

    [Display(Name = "box")]
    Box = 32536,

    [Display(Name = "bank")]
    Jar = 32537,

    [Display(Name = "container")]
    Container = 32538,

    [Display(Name = "linear meter")]
    LinearMeter = 32539,

    [Display(Name = "bottle")]
    Bottle = 32540,

    [Display(Name = "package")]
    Package = 32541,

    [Display(Name = "roll")]
    Roll = 32542,

    [Display(Name = "hour")]
    Hour = 32543,

    [Display(Name = "bight")]
    Coil = 32544,

    [Display(Name = "thousand pieces")]
    ThousandPieces = 32545,

    [Display(Name = "bag")]
    Bag = 32546,

    [Display(Name = "pan")]
    Pallet = 32547,

    [Display(Name = "thousand square meters")]
    ThousandSquareMeters = 32548,

    [Display(Name = "packing")]
    Packing = 32549,

    [Display(Name = "ampoule")]
    Ampoule = 32550,

    [Display(Name = "thousand linear meters")]
    ThousandLinearMeters = 32551,

    [Display(Name = "balloon")]
    Cylinder = 32552,

    [Display(Name = "jerrycan")]
    Jerrycan = 32553,

    [Display(Name = "one sheets")]
    HundredSheets = 32554,

    [Display(Name = "tube")]
    Tube = 32555,

    [Display(Name = "barrel")]
    Barrel = 32556,

    [Display(Name = "bucket")]
    Bucket = 32557,

    [Display(Name = "milliliter")]
    Milliliter = 32558,

    [Display(Name = "gram")]
    Gram = 32559,

    [Display(Name = "thousand rubles")]
    ThousandRubles = 32560,

    [Display(Name = "thousand meters")]
    ThousandMeters = 32561,

    [Display(Name = "thousand cubic meters")]
    ThousandCubicMeters = 32562,

    [Display(Name = "half-set")]
    HalfSet = 32563,

    [Display(Name = "tons of equivalent fuel")]
    TonsOfEquivalentFuel = 32564,

    [Display(Name = "kilogram of fuel equivalent")]
    KilogramOfFuelEquivalent = 32565,

    [Display(Name = "millimeter")]
    Millimeter = 32566,

    [Display(Name = "soft container")]
    SoftContainer = 32567,

    [Display(Name = "Giga-calorie")]
    GigaCalorie = 32568,

    [Display(Name = "megacalorie")]
    Megacalorie = 32569,

    [Display(Name = "kilowatt - hour")]
    KilowattHour = 32570,

    [Display(Name = "Megawatt")]
    Megawatt = 32571,

    [Display(Name = "decaliter")]
    Decaliter = 32572,

    [Display(Name = "Milligram")]
    Milligram = 32573,

    [Display(Name = "Gigacalorie per hour")]
    GigacaloriePerHour = 32574,

    [Display(Name = "lb")]
    Pound = 32575,

    [Display(Name = "Megawatt-hour")]
    MegawattHour = 32576,
}
