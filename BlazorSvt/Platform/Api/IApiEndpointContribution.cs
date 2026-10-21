namespace BlazorSvt.Platform.Api;

/// <summary>
/// Вклад модуля в API: модуль регистрирует реализацию в DI, <c>MapSvtApi</c> вызывает
/// <see cref="Map"/> на группе <c>/api/v1</c> (Host не знает о модулях).
/// </summary>
public interface IApiEndpointContribution
{
    void Map(RouteGroupBuilder api);
}
