namespace MWH.KeyPressCounter;
public class Counter
{
    public int CurrentCount { get; private set; }
    public int TotalCount { get; private set; }
    public int MaxPerInterval { get; private set; }
    private int intervalsWithoutIncrement;
    public int LongestIntervalWithoutIncrement { get; private set; }

    public void Increment()
    {
        CurrentCount++;
        TotalCount++;
        intervalsWithoutIncrement = 0; // Reset the interval counter on each increment
    }

    public void UpdateIntervalMetrics()
    {
        if (CurrentCount > MaxPerInterval)
        {
            MaxPerInterval = CurrentCount;
        }

        if (CurrentCount == 0)
        {
            intervalsWithoutIncrement++;
        }

        if (intervalsWithoutIncrement > LongestIntervalWithoutIncrement)
        {
            LongestIntervalWithoutIncrement = intervalsWithoutIncrement;
        }

        // Reset the counter for the next interval
        CurrentCount = 0;
    }

    public void ResetTotalMetrics()
    {
        TotalCount = 0;
        MaxPerInterval = 0;
        LongestIntervalWithoutIncrement = 0;
    }
    public override string ToString()
    {
        return $"Total Count: {TotalCount}, Current Count: {CurrentCount}, Max Per Interval: {MaxPerInterval}, Longest Interval Without Increment: {LongestIntervalWithoutIncrement}";
    }
}