using BlazorSvt.Platform.Sync;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Sync;

[Trait("Category", "Unit")]
public class SnapshotSyncSchedulerTests
{
    private static readonly TimeSpan ReconcileAt = new(2, 0, 0);
    private static readonly DateTime Day = new(2026, 7, 7, 0, 0, 0, DateTimeKind.Unspecified);

    [Fact]
    public void CrossedReconcileBoundary_WhenTickCrossesScheduledTime_ReturnsTrue()
    {
        var previous = Day.AddHours(1).AddMinutes(50);
        var now = Day.AddHours(2).AddMinutes(10);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeTrue();
    }

    [Fact]
    public void CrossedReconcileBoundary_WhenTickDoesNotReachScheduledTime_ReturnsFalse()
    {
        var previous = Day.AddHours(1);
        var now = Day.AddHours(1).AddMinutes(30);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeFalse();
    }

    [Fact]
    public void CrossedReconcileBoundary_WhenPreviousIsAlreadyAfterScheduledTime_ReturnsFalse()
    {
        var previous = Day.AddHours(2).AddMinutes(5);
        var now = Day.AddHours(2).AddMinutes(10);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeFalse();
    }

    [Fact]
    public void CrossedReconcileBoundary_WhenScheduledTimeIsOnTickBoundary_ReturnsTrue()
    {
        var previous = Day.AddHours(1).AddMinutes(59).AddSeconds(59);
        var now = Day.AddHours(2).AddSeconds(1);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeTrue();
    }

    [Fact]
    public void CrossedReconcileBoundary_WhenAppStartedAfterScheduledTime_NoCatchUp()
    {
        var previous = Day.AddHours(10);
        var now = Day.AddHours(10).AddMinutes(30);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeFalse();
    }

    [Fact]
    public void CrossedReconcileBoundary_WhenTickSpansMidnightAndCrossesScheduledTime_ReturnsTrue()
    {
        var previous = Day.AddHours(23);
        var now = Day.AddDays(1).AddHours(3);

        SnapshotSyncScheduler.CrossedReconcileBoundary(previous, now, ReconcileAt)
            .Should().BeTrue();
    }
}
