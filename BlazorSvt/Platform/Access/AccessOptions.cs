namespace BlazorSvt.Platform.Access;

/// <summary>Секция <c>Auth</c> в appsettings.</summary>
public sealed class AccessOptions
{
    public const string SectionName = "Auth";

    /// <summary>
    /// Только Development: пропуск проверки ролей (локальная отладка и тесты без IIS).
    /// Вне Development игнорируется, даже если в конфиге true.
    /// </summary>
    public bool IgnoreAccessControl { get; set; }

    /// <summary>
    /// Сверка <c>UserRole</c> с прямыми AD-группами при открытии circuit.
    /// false или отсутствие ключа — только чтение уже сохранённых ролей, без обращения в AD.
    /// </summary>
    public bool SynchronizeUserRoles { get; set; }
}
