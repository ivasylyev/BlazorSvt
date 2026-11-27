namespace BlazorSvt.Platform.Access;

/// <summary>Проверка права в сервисах данных (не только в UI). Write по домену — отдельный <see cref="AccessAction"/> в вехе 6 (пакет 6.1).</summary>
public interface IAccessGuard
{
    void Ensure(AccessAction action);
}
