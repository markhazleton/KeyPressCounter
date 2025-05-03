using System.Threading;

namespace MWH.KeyPressCounter;

/// <summary>
/// Provides counting functionality with thread-safe operations for tracking key presses and mouse clicks.
/// </summary>
public class Counter
{
    private int currentCount;
    private int totalCount;
    private int maxPerInterval;
    private int intervalsWithoutIncrement;
    private int longestIntervalWithoutIncrement;
    private readonly object lockObject = new();

    /// <summary>
    /// Gets the current count within the active interval.
    /// </summary>
    public int CurrentCount => currentCount;

    /// <summary>
    /// Gets the total count since the counter was last reset.
    /// </summary>
    public int TotalCount => totalCount;

    /// <summary>
    /// Gets the maximum count recorded in any interval.
    /// </summary>
    public int MaxPerInterval => maxPerInterval;

    /// <summary>
    /// Gets the longest period (in intervals) during which no increments were recorded.
    /// </summary>
    public int LongestIntervalWithoutIncrement => longestIntervalWithoutIncrement;

    /// <summary>
    /// Increments both the current and total counters atomically.
    /// </summary>
    public void Increment()
    {
        lock (lockObject)
        {
            currentCount++;
            totalCount++;
            intervalsWithoutIncrement = 0; // Reset the interval counter on each increment
        }
    }

    /// <summary>
    /// Updates the metrics at the end of an interval, including max values and tracking periods of inactivity.
    /// </summary>
    public void UpdateIntervalMetrics()
    {
        lock (lockObject)
        {
            if (currentCount > maxPerInterval)
            {
                maxPerInterval = currentCount;
            }

            if (currentCount == 0)
            {
                intervalsWithoutIncrement++;
            }

            if (intervalsWithoutIncrement > longestIntervalWithoutIncrement)
            {
                longestIntervalWithoutIncrement = intervalsWithoutIncrement;
            }

            // Reset the counter for the next interval
            currentCount = 0;
        }
    }

    /// <summary>
    /// Resets all total and historical metrics to zero.
    /// </summary>
    public void ResetTotalMetrics()
    {
        lock (lockObject)
        {
            totalCount = 0;
            maxPerInterval = 0;
            longestIntervalWithoutIncrement = 0;
        }
    }

    /// <summary>
    /// Returns a string representation of the counter's current state.
    /// </summary>
    /// <returns>A formatted string showing counter statistics</returns>
    public override string ToString()
    {
        return $"Total Count: {TotalCount}, Current Count: {CurrentCount}, Max Per Interval: {MaxPerInterval}, Longest Interval Without Increment: {LongestIntervalWithoutIncrement}";
    }
}