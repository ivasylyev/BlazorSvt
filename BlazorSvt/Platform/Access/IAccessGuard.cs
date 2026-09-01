namespace BlazorSvt.Platform.Access;

/// <summary>Проверка права в сервисах данных (не только в UI). Write по домену — отдельный <see cref="AccessAction"/> в MVP 0.4+.</summary>
public interface IAccessGuard
{
    void Ensure(AccessAction action);
}
