using MWH.KeyPressCounter;

namespace KeyPressCounter.SmokeTests;

[TestClass]
public sealed class SmokeTests
{
    [TestMethod]
    public void CounterIncrementAndUpdateMetricsTracksTotalsAndPeak()
    {
        var counter = new Counter();

        counter.Increment();
        counter.Increment();
        counter.UpdateIntervalMetrics();

        Assert.AreEqual(2, counter.TotalCount);
        Assert.AreEqual(2, counter.MaxPerInterval);
        Assert.AreEqual(0, counter.CurrentCount);
    }

    [TestMethod]
    public void CounterTwoIdleIntervalsTracksLongestIdleInterval()
    {
        var counter = new Counter();

        counter.UpdateIntervalMetrics();
        counter.UpdateIntervalMetrics();

        Assert.AreEqual(2, counter.LongestIntervalWithoutIncrement);
    }

    [TestMethod]
    public void AppConfigDefaultPathsAreComposedFromDirectoryAndFileNames()
    {
        var config = new AppConfig();

        string expectedActivityPath = Path.Combine(config.LogDirectory, config.ActivityLogFileName);
        string expectedSummaryPath = Path.Combine(config.LogDirectory, config.DailySummaryLogFileName);

        Assert.AreEqual(expectedActivityPath, config.GetActivityLogPath());
        Assert.AreEqual(expectedSummaryPath, config.GetDailySummaryLogPath());
    }
}
