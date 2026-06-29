namespace BlazorSvt.Platform.Grid.Services;

public class PageTimingService
{
    private TimeSpan loadDuration = TimeSpan.Zero;

    public TimeSpan LoadDuration
    {
        get => loadDuration;
        set
        {
            loadDuration = value;
            OnChange?.Invoke();
        }
    }

    public event Action? OnChange;
}