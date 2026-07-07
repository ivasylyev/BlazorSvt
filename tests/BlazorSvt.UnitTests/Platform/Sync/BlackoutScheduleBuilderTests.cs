using BlazorSvt.Platform.Sync;
using FluentAssertions;
using Microsoft.Extensions.Logging.Abstractions;

namespace BlazorSvt.UnitTests.Platform.Sync;

[Trait("Category", "Unit")]
public class BlackoutScheduleBuilderTests
{
    private static readonly IReadOnlyList<BlackoutInterval> DefaultSchedule =
    [
        new()
        {
            StartTime = "01:00:00",
            EndTime = "02:30:00",
            DaysOfWeek =
            [
                "Monday",
                "Tuesday",
                "Wednesday",
                "Thursday",
                "Friday",
                "Saturday",
                "Sunday"
            ]
        },
        new()
        {
            StartTime = "04:00:00",
            EndTime = "06:00:00",
            DaysOfWeek = ["Saturday"]
        }
    ];

    [Fact]
    public void Build_AppliesIntervalOnlyToSpecifiedDays()
    {
        var schedule = BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "10:00",
                EndTime = "11:00",
                DaysOfWeek = ["Wednesday"]
            }
        ]);

        schedule.Should().ContainKey(DayOfWeek.Wednesday);
        schedule.Should().NotContainKey(DayOfWeek.Tuesday);
        schedule[DayOfWeek.Wednesday].Should().ContainSingle()
            .Which.Should().Be((TimeSpan.FromHours(10), TimeSpan.FromHours(11)));
    }

    [Fact]
    public void Build_SaturdayHasTwoWindows_SundayHasOne()
    {
        var schedule = BlackoutScheduleBuilder.Build(DefaultSchedule);

        schedule[DayOfWeek.Saturday].Should().HaveCount(2);
        schedule[DayOfWeek.Sunday].Should().ContainSingle()
            .Which.Should().Be((new TimeSpan(1, 0, 0), new TimeSpan(2, 30, 0)));
    }

    [Fact]
    public void IsInBlackout_UsesHalfOpenInterval()
    {
        var schedule = BlackoutScheduleBuilder.Build(DefaultSchedule);
        var saturday = new DateTime(2026, 7, 4, 0, 0, 0, DateTimeKind.Unspecified);

        BlackoutScheduleBuilder.IsInBlackout(saturday.AddHours(1), schedule).Should().BeTrue();
        BlackoutScheduleBuilder.IsInBlackout(saturday.AddHours(2).AddMinutes(30), schedule).Should().BeFalse();
        BlackoutScheduleBuilder.IsInBlackout(saturday.AddHours(4), schedule).Should().BeTrue();
        BlackoutScheduleBuilder.IsInBlackout(saturday.AddHours(6), schedule).Should().BeFalse();
    }

    [Fact]
    public void Build_EmptyDaysOfWeek_Throws()
    {
        var act = () => BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "01:00",
                EndTime = "02:00",
                DaysOfWeek = []
            }
        ]);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*DaysOfWeek*");
    }

    [Fact]
    public void Build_NullDaysOfWeek_Throws()
    {
        var act = () => BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "01:00",
                EndTime = "02:00",
                DaysOfWeek = null!
            }
        ]);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*DaysOfWeek*");
    }

    [Fact]
    public void Build_InvalidWindow_IsSkipped()
    {
        var schedule = BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "03:00",
                EndTime = "03:00",
                DaysOfWeek = ["Monday"]
            },
            new BlackoutInterval
            {
                StartTime = "04:00",
                EndTime = "05:00",
                DaysOfWeek = ["Monday"]
            }
        ],
        NullLogger.Instance);

        schedule[DayOfWeek.Monday].Should().ContainSingle()
            .Which.Should().Be((TimeSpan.FromHours(4), TimeSpan.FromHours(5)));
    }

    [Fact]
    public void Build_EmptyList_ReturnsEmptySchedule()
    {
        var schedule = BlackoutScheduleBuilder.Build([]);

        schedule.Should().BeEmpty();
        BlackoutScheduleBuilder.IsInBlackout(DateTime.Now, schedule).Should().BeFalse();
    }

    [Fact]
    public void IsInBlackout_OverlappingIntervalsOnSameDay_ReturnsTrue()
    {
        var schedule = BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "01:00",
                EndTime = "03:00",
                DaysOfWeek = ["Friday"]
            },
            new BlackoutInterval
            {
                StartTime = "02:00",
                EndTime = "04:00",
                DaysOfWeek = ["Friday"]
            }
        ]);

        var friday = new DateTime(2026, 7, 3, 2, 30, 0, DateTimeKind.Unspecified);
        BlackoutScheduleBuilder.IsInBlackout(friday, schedule).Should().BeTrue();
    }

    [Fact]
    public void Build_ParsesDayNamesCaseInsensitively()
    {
        var schedule = BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "08:00",
                EndTime = "09:00",
                DaysOfWeek = ["saturday"]
            }
        ]);

        schedule.Should().ContainKey(DayOfWeek.Saturday);
    }

    [Fact]
    public void Build_UnknownDayName_Throws()
    {
        var act = () => BlackoutScheduleBuilder.Build(
        [
            new BlackoutInterval
            {
                StartTime = "01:00",
                EndTime = "02:00",
                DaysOfWeek = ["Funday"]
            }
        ]);

        act.Should().Throw<InvalidOperationException>()
            .WithMessage("*Funday*");
    }
}
