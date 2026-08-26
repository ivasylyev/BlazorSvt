namespace BlazorSvt.Modules.HomeRatePivot;

/// <summary>
/// Хардкод whitelist направлений для прототипа виджета на Home.
/// Порядок = порядок строк в таблице.
/// </summary>
public static class HomeRatePivotDirections
{
    public static IReadOnlyList<(string FromCode, string ToCode)> Pairs { get; } =
    [
        ("A56543", "REG74"), // Нижнекамск - Респ. Татарстан
        ("A56543", "REG47"), // Нижнекамск - Московская обл.
        ("A56522", "REG74"), // Казань - Респ. Татарстан
        ("A56522", "REG47"), // Казань - Московская обл.
        ("A56395", "REG47"), // Тобольск - Московская обл.
        ("A56395", "REG81"), // Тобольск - Свердловская обл.
        ("A56395", "REG74"), // Тобольск - Респ. Татарстан
        ("A56395", "REG29"), // Тобольск - Калужская обл.
        ("A56364", "REG47"), // Томск - Московская обл.
    ];
}
