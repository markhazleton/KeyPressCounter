using System.Runtime.InteropServices;

namespace MWH.KeyPressCounter;

/// <summary>
/// Provides utilities to detect when the user is idle based on the lack of keyboard or mouse input.
/// </summary>
public static class UserIdleDetector
{
    [DllImport("user32.dll")]
    private static extern bool GetLastInputInfo(ref LASTINPUTINFO plii);

    [StructLayout(LayoutKind.Sequential)]
    private struct LASTINPUTINFO
    {
        public uint cbSize;
        public uint dwTime;
    }

    /// <summary>
    /// Determines whether the user has been idle for the specified number of seconds.
    /// </summary>
    /// <param name="idleThresholdSeconds">The threshold in seconds after which the user is considered idle.</param>
    /// <returns>True if the user has been idle for the specified period, false otherwise.</returns>
    public static bool IsUserIdle(int idleThresholdSeconds)
    {
        return GetIdleTimeSeconds() >= idleThresholdSeconds;
    }

    /// <summary>
    /// Gets the number of seconds the user has been idle.
    /// </summary>
    /// <returns>The number of seconds since the last user input.</returns>
    public static int GetIdleTimeSeconds()
    {
        LASTINPUTINFO lastInputInfo = new();
        lastInputInfo.cbSize = (uint)Marshal.SizeOf(lastInputInfo);
        
        if (!GetLastInputInfo(ref lastInputInfo))
        {
            return 0; // Failed to get idle time, assume not idle
        }
        
        // Calculate idle time in seconds
        uint idleTime = ((uint)Environment.TickCount - lastInputInfo.dwTime) / 1000;
        return (int)idleTime;
    }
}