using System;
using System.Diagnostics;
using Prism.Navigation;
using Xunit;

namespace Prism.Maui.Tests.Fixtures.Navigation;

/// <summary>
/// Tests for the minimum-interval delay <see cref="PageNavigationService"/> enforces between
/// navigations. Reproduces issue #3405 where a backward device clock/timezone change caused the
/// delay to balloon and navigation to appear frozen.
/// </summary>
public class PageNavigationDelayFixture
{
    // A fixed, arbitrary baseline timestamp so the tests are deterministic and independent of
    // the machine's current Stopwatch value.
    private const long Baseline = 1_000_000_000_000;

    // Converts a TimeSpan offset into Stopwatch ticks so the tests are independent of Stopwatch.Frequency.
    private static long OffsetTimestamp(long baseline, TimeSpan offset) =>
        baseline + (long)(offset.TotalSeconds * Stopwatch.Frequency);

    [Fact]
    public void GetRemainingNavigationDelay_WhenLessThanMinimumElapsed_ReturnsRemainingTime()
    {
        var min = PageNavigationService.MinTimeBetweenNavigations;
        var elapsed = TimeSpan.FromMilliseconds(50);
        var now = OffsetTimestamp(Baseline, elapsed);

        var delay = PageNavigationService.GetRemainingNavigationDelay(Baseline, now);

        // Some, but not all, of the minimum interval remains.
        Assert.True(delay > TimeSpan.Zero);
        Assert.InRange(delay, TimeSpan.Zero, min);
    }

    [Fact]
    public void GetRemainingNavigationDelay_WhenMinimumAlreadyElapsed_ReturnsZero()
    {
        var min = PageNavigationService.MinTimeBetweenNavigations;
        var now = OffsetTimestamp(Baseline, min + TimeSpan.FromMilliseconds(50));

        var delay = PageNavigationService.GetRemainingNavigationDelay(Baseline, now);

        Assert.Equal(TimeSpan.Zero, delay);
    }

    // Issue #3405: when the device clock/timezone moves backward, the "current" timestamp is earlier
    // than the timestamp recorded after the previous navigation. The delay must never exceed the
    // configured minimum. Previously the delay grew by the full size of the backward jump (e.g. an
    // hour), so the next navigation's Task.Delay never returned and the app appeared to freeze.
    [Theory]
    [InlineData(1)]         // one minute earlier
    [InlineData(60)]        // one hour earlier (the exact scenario reported in the issue)
    [InlineData(60 * 24)]   // one day earlier
    public void GetRemainingNavigationDelay_WhenClockMovesBackward_DoesNotExceedMinimum(int minutesBackward)
    {
        var min = PageNavigationService.MinTimeBetweenNavigations;
        var now = OffsetTimestamp(Baseline, TimeSpan.FromMinutes(-minutesBackward));

        var delay = PageNavigationService.GetRemainingNavigationDelay(Baseline, now);

        Assert.InRange(delay, TimeSpan.Zero, min);
    }
}
