using Newtonsoft.Json;

namespace BlazorSvt.Platform.Grid.Models;

public sealed record GridFilter(
    string PropertyName,
    string? Value,
    string Operator,
    [property: JsonProperty(NullValueHandling = NullValueHandling.Ignore)] string? Precision = null);
