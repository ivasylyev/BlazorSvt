namespace BlazorSvt.Platform.Access;

/// <summary>
/// Действие для <see cref="IAccessGuard"/>. Write по домену справочника — отдельный action в вехи 6,
/// не булевы флаги на <see cref="ICurrentUser"/>.
/// </summary>
public enum AccessAction
{
    Read = 0,

    /// <summary>Прототип загрузчика (/load). На проде не выдаётся; в 9+ заменяется доменным write.</summary>
    Import = 1
}
