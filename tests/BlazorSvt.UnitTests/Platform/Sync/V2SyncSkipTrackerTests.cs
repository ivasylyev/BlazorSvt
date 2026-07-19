using BlazorSvt.Platform.Sync;
using FluentAssertions;

namespace BlazorSvt.UnitTests.Platform.Sync;

[Trait("Category", "Unit")]
public class V2SyncSkipTrackerTests
{
    [Fact]
    public void Observe_WhenSeededDisabled_FirstTicksDoNotLogTransitionOrHeartbeat()
    {
        var tracker = new V2SyncSkipTracker(initialEnabled: false);

        var d1 = tracker.Observe(false);
        d1.ShouldRun.Should().BeFalse();
        d1.LogTransition.Should().BeFalse();
        d1.LogHeartbeat.Should().BeFalse();
        d1.SkippedTicks.Should().Be(1);

        var d9 = default(V2SyncSkipTracker.Decision);
        for (var i = 0; i < 8; i++)
        {
            d9 = tracker.Observe(false);
        }

        d9.SkippedTicks.Should().Be(9);
        d9.LogHeartbeat.Should().BeFalse();
    }

    [Fact]
    public void Observe_WhenDisabled_LogsHeartbeatEveryTenthSkippedTick()
    {
        var tracker = new V2SyncSkipTracker(initialEnabled: false);

        V2SyncSkipTracker.Decision? tenth = null;
        for (var i = 0; i < V2SyncSkipTracker.HeartbeatEveryTicks; i++)
        {
            tenth = tracker.Observe(false);
        }

        tenth.Should().NotBeNull();
        tenth!.Value.LogHeartbeat.Should().BeTrue();
        tenth.Value.SkippedTicks.Should().Be(10);
        tenth.Value.ShouldRun.Should().BeFalse();
    }

    [Fact]
    public void Observe_WhenTransitionOffToOn_LogsTransitionAndResetsSkipCounter()
    {
        var tracker = new V2SyncSkipTracker(initialEnabled: false);
        tracker.Observe(false);

        var on = tracker.Observe(true);

        on.ShouldRun.Should().BeTrue();
        on.LogTransition.Should().BeTrue();
        on.LogHeartbeat.Should().BeFalse();
        on.SkippedTicks.Should().Be(0);
        on.Enabled.Should().BeTrue();
    }

    [Fact]
    public void Observe_WhenTransitionOnToOff_LogsTransition()
    {
        var tracker = new V2SyncSkipTracker(initialEnabled: true);

        var off = tracker.Observe(false);

        off.ShouldRun.Should().BeFalse();
        off.LogTransition.Should().BeTrue();
        off.LogHeartbeat.Should().BeFalse();
        off.SkippedTicks.Should().Be(1);
        off.Enabled.Should().BeFalse();
    }

    [Fact]
    public void Observe_WhenUnseeded_FirstValueDoesNotLogTransition()
    {
        var tracker = new V2SyncSkipTracker();

        var first = tracker.Observe(true);

        first.ShouldRun.Should().BeTrue();
        first.LogTransition.Should().BeFalse();
    }
}
