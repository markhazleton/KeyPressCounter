using Gma.System.MouseKeyHook;
using System.Timers;

namespace MWH.KeyPressCounter;

public class CustomApplicationContext : ApplicationContext
{
    private readonly string dailyLogFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "DailySummaryLog.txt");
    private System.Timers.Timer dailyLogTimer;
    private IKeyboardMouseEvents globalHook;
    private readonly Counter keyPressCounter = new();
    private readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ActivityLog.txt");
    private System.Timers.Timer logTimer;
    private readonly Counter mouseClickCounter = new();
    private NotifyIcon trayIcon;

    public CustomApplicationContext()
    {

        InitializeContext();
        StartGlobalHooks();
        SetupLogTimer();
        SetupDailyLogTimer();
    }

    private ContextMenuStrip CreateContextMenu()
    {
        ContextMenuStrip menu = new ContextMenuStrip();
        ToolStripMenuItem exitItem = new ToolStripMenuItem("Exit", null, Exit_Click);

        menu.Items.Add(exitItem);
        return menu;
    }


    private void Exit_Click(object sender, EventArgs e)
    {
        Application.Exit();
    }

    private TimeSpan GetRemainingTimeUntilEndOfDay()
    {
        DateTime now = DateTime.Now;
        DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
        return endOfDay - now;
    }


    private void InitializeContext()
    {
        trayIcon = new NotifyIcon()
        {
            Icon = new Icon("Icon1.ico"), // Set your icon here
            ContextMenuStrip = CreateContextMenu(), // Optional: Set if you want a right-click menu
            Visible = true, // Make the icon visible in the tray
            Text = "Your application tooltip text here" // Hover text
        };
        // Optional: Handle double-click event
        trayIcon.DoubleClick += TrayIcon_DoubleClick;
    }

    private void LogActivity(object sender, ElapsedEventArgs e)
    {
        keyPressCounter.UpdateIntervalMetrics();
        mouseClickCounter.UpdateIntervalMetrics();

        string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: Keystrokes: {keyPressCounter.TotalCount}, Mouse Clicks: {mouseClickCounter.TotalCount}, Max Keystrokes/Min: {keyPressCounter.MaxPerInterval}, Max Clicks/Min: {mouseClickCounter.MaxPerInterval}";
        File.AppendAllText(logFilePath, log + Environment.NewLine);
    }

    private void LogDailySummary(object sender, ElapsedEventArgs e)
    {
        double averageClicksPerMinute = mouseClickCounter.TotalCount / 1440.0; // 1440 minutes in a day
        string log = $"{DateTime.Now:yyyy-MM-dd}: Total Keystrokes: {keyPressCounter.TotalCount}, Total Mouse Clicks: {mouseClickCounter.TotalCount}, Avg Clicks/Min: {averageClicksPerMinute:F2}, Longest No Click Period: {mouseClickCounter.LongestIntervalWithoutIncrement} minutes";
        File.AppendAllText(dailyLogFilePath, log + Environment.NewLine);

        keyPressCounter.ResetTotalMetrics();
        mouseClickCounter.ResetTotalMetrics();

        // Set up the timer for the next day
        SetupDailyLogTimer();
    }

    private void SetupDailyLogTimer()
    {
        dailyLogTimer = new System.Timers.Timer(GetRemainingTimeUntilEndOfDay().TotalMilliseconds);
        dailyLogTimer.Elapsed += LogDailySummary;
        dailyLogTimer.AutoReset = false; // Only trigger once
        dailyLogTimer.Enabled = true;
    }

    private void SetupLogTimer()
    {
        logTimer = new System.Timers.Timer(60000); // 60 seconds
        logTimer.Elapsed += LogActivity;
        logTimer.AutoReset = true;
        logTimer.Enabled = true;
    }

    private void StartGlobalHooks()
    {
        globalHook = Hook.GlobalEvents();
        globalHook.KeyPress += (sender, e) => keyPressCounter.Increment();
        globalHook.MouseClick += (sender, e) => mouseClickCounter.Increment();
    }
    private void TrayIcon_DoubleClick(object sender, EventArgs e)
    {
        UpdateTrayIconText();
    }

    private void UpdateTrayIconText()
    {
        string tooltipText = $"Keystrokes: {keyPressCounter.TotalCount}, Clicks: {mouseClickCounter.TotalCount}";
        if (tooltipText.Length >= 63) // Ensure the tooltip text length does not exceed the typical limit
        {
            tooltipText = $"{tooltipText.Substring(0, 60)}...";
        }
        trayIcon.Text = tooltipText;
    }

    // Make sure to dispose of the timers and globalHook properly
    protected override void Dispose(bool disposing)
    {
        if (disposing)
        {
            trayIcon?.Dispose();
            logTimer?.Dispose();
            dailyLogTimer?.Dispose();
            globalHook?.Dispose();
        }
        base.Dispose(disposing);
    }
}
