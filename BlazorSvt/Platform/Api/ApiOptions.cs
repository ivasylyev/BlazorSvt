namespace BlazorSvt.Platform.Api;

/// <summary>Секция <c>Api</c> в appsettings.</summary>
public sealed class ApiOptions
{
    public const string SectionName = "Api";

    /// <summary>
    /// Windows-учётки (<c>DOMAIN\user</c>), которым разрешён вызов <c>/api</c>:
    /// служебная учётка Атласа, на dev — ещё и учётки разработчиков.
    /// Пусто — API закрыт для всех (fail-closed).
    /// </summary>
    public string[] TrustedClients { get; set; } = [];

    /// <summary>Swagger UI по адресу <c>/api/swagger</c>. На проде — по решению при релизе.</summary>
    public bool SwaggerUiEnabled { get; set; }
}
