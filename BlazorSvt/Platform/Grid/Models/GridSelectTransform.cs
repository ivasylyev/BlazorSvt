namespace BlazorSvt.Platform.Grid.Models;

/// <summary>Как собрать выражение SELECT для колонки snapshot.</summary>
public enum GridSelectTransform
{
    /// <summary>DateOnly → <see cref="CastAsDate"/>, иначе <see cref="None"/>.</summary>
    Auto,

    /// <summary>Колонка как есть (с алиасом, если SqlColumn ≠ имя свойства).</summary>
    None,

    /// <summary><c>CAST(col AS DATE) AS Prop</c> — для DateOnly / date-колонок.</summary>
    CastAsDate
}
