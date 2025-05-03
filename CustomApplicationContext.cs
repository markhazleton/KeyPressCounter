using Gma.System.MouseKeyHook;
using System.Timers;
using System.Diagnostics;
using System.Text;

namespace MWH.KeyPressCounter;

/// <summary>
/// Manages the application context for the system tray application.
/// </summary>
public class CustomApplicationContext : ApplicationContext, IDisposable
{
    // Constants for the application
    private const int SHORT_NOTIFICATION_DURATION = 3000;
    private const int LONG_NOTIFICATION_DURATION = 5000;
    
    // Fields initialized in constructor
    private readonly AppConfig config = null!;
    private readonly string dailyLogFilePath = null!;
    private System.Timers.Timer dailyLogTimer = null!;
    private IKeyboardMouseEvents globalHook = null!;
    private readonly Counter keyPressCounter = new();
    private readonly string logFilePath = null!;
    private System.Timers.Timer logTimer = null!;
    private readonly Counter mouseClickCounter = new();
    private NotifyIcon trayIcon = null!;
    
    // System performance monitoring
    private readonly SystemPerformanceMonitor performanceMonitor;
    
    // Statistics form
    private StatsForm? statsForm;
    
    // Track if Dispose has been called
    private bool disposed;

    /// <summary>
    /// Initializes a new instance of the CustomApplicationContext class.
    /// </summary>
    public CustomApplicationContext()
    {
        try
        {
            // Initialize system performance monitor
            performanceMonitor = new SystemPerformanceMonitor();
            
            // Load application configuration
            config = AppConfig.Load();
            dailyLogFilePath = config.GetDailySummaryLogPath();
            logFilePath = config.GetActivityLogPath();

            // Apply startup settings to match configuration
            config.ApplyStartupSetting();

            InitializeContext();
            StartGlobalHooks();
            SetupLogTimer();
            SetupDailyLogTimer();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing application: {ex}");
            MessageBox.Show($"Error initializing application: {ex.Message}", "Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
            Application.Exit();
        }
    }

    /// <summary>
    /// Creates the context menu for the system tray icon.
    /// </summary>
    private ContextMenuStrip CreateContextMenu()
    {
        ContextMenuStrip menu = new();

        // View Statistics submenu - expanded with more options
        ToolStripMenuItem statsMenu = new("Statistics");
        
        // Basic input stats
        ToolStripMenuItem inputStatsItem = new("Input Statistics", null, (s, e) => ShowOrActivateStatsForm());
        statsMenu.DropDownItems.Add(inputStatsItem);
        
        // Performance stats
        ToolStripMenuItem perfStatsItem = new("System Performance", null, (s, e) => ShowPerformanceInfo());
        statsMenu.DropDownItems.Add(perfStatsItem);
        
        // System info
        ToolStripMenuItem sysInfoItem = new("System Information", null, (s, e) => ShowSystemInfo());
        statsMenu.DropDownItems.Add(sysInfoItem);
        
        // Running processes (Top processes by CPU/Memory)
        ToolStripMenuItem processesItem = new("Running Processes", null, (s, e) => ShowTopProcesses());
        statsMenu.DropDownItems.Add(processesItem);
        
        menu.Items.Add(statsMenu);

        // Open logs submenu
        ToolStripMenuItem logsMenu = new("Logs");

        // Activity log item
        ToolStripMenuItem activityLogItem = new("Open Activity Log", null, (s, e) => OpenFile(logFilePath, "Activity log file has not been created yet."));
        logsMenu.DropDownItems.Add(activityLogItem);

        // Summary log item
        ToolStripMenuItem summaryLogItem = new("Open Daily Summary Log", null, (s, e) => OpenFile(dailyLogFilePath, "Daily summary log file has not been created yet."));
        logsMenu.DropDownItems.Add(summaryLogItem);

        // Open log folder item
        ToolStripMenuItem openLogFolderItem = new("Open Log Folder", null, (s, e) => OpenLogFolder());
        logsMenu.DropDownItems.Add(openLogFolderItem);

        menu.Items.Add(logsMenu);

        // Settings submenu
        ToolStripMenuItem settingsMenu = new("Settings");
        
        // Start with Windows option
        ToolStripMenuItem startupItem = new("Start with Windows")
        {
            CheckOnClick = true,
            Checked = config.StartWithWindows
        };
        startupItem.Click += (s, e) => 
        {
            config.StartWithWindows = startupItem.Checked;
            config.ApplyStartupSetting();
            config.Save();
        };
        settingsMenu.DropDownItems.Add(startupItem);
        
        // Detect idle periods option
        ToolStripMenuItem idleDetectionItem = new("Detect Idle Periods")
        {
            CheckOnClick = true,
            Checked = config.DetectIdlePeriods
        };
        idleDetectionItem.Click += (s, e) => 
        {
            config.DetectIdlePeriods = idleDetectionItem.Checked;
            config.Save();
        };
        settingsMenu.DropDownItems.Add(idleDetectionItem);
        
        menu.Items.Add(settingsMenu);

        // System tools submenu - new!
        ToolStripMenuItem toolsMenu = new("System Tools");
        
        // Open Task Manager
        ToolStripMenuItem taskManagerItem = new("Task Manager", null, (s, e) => LaunchSystemTool("taskmgr.exe"));
        toolsMenu.DropDownItems.Add(taskManagerItem);
        
        // Open Resource Monitor
        ToolStripMenuItem resMonItem = new("Resource Monitor", null, (s, e) => LaunchSystemTool("resmon.exe"));
        toolsMenu.DropDownItems.Add(resMonItem);
        
        // Open Performance Monitor
        ToolStripMenuItem perfMonItem = new("Performance Monitor", null, (s, e) => LaunchSystemTool("perfmon.exe"));
        toolsMenu.DropDownItems.Add(perfMonItem);
        
        // Open System Information
        ToolStripMenuItem msInfoItem = new("System Information", null, (s, e) => LaunchSystemTool("msinfo32.exe"));
        toolsMenu.DropDownItems.Add(msInfoItem);
        
        menu.Items.Add(toolsMenu);

        // Reset counters option
        ToolStripMenuItem resetCountersItem = new("Reset Counters", null, (s, e) =>
        {
            if (MessageBox.Show("Are you sure you want to reset all counters?", "Confirm Reset",
                    MessageBoxButtons.YesNo, MessageBoxIcon.Question) == DialogResult.Yes)
            {
                keyPressCounter.ResetTotalMetrics();
                mouseClickCounter.ResetTotalMetrics();
                trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Reset Complete", "All counters have been reset.", ToolTipIcon.Info);
            }
        });
        menu.Items.Add(resetCountersItem);

        menu.Items.Add(new ToolStripSeparator());

        // Exit menu item
        ToolStripMenuItem exitItem = new("Exit", null, Exit_Click);
        menu.Items.Add(exitItem);

        return menu;
    }

    /// <summary>
    /// Launches a system tool with the specified executable name.
    /// </summary>
    private void LaunchSystemTool(string exeName)
    {
        try
        {
            Process.Start(new ProcessStartInfo
            {
                FileName = exeName,
                UseShellExecute = true
            });
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error launching system tool {exeName}: {ex.Message}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Error", $"Could not launch {exeName}: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Shows or activates the statistics form.
    /// </summary>
    private void ShowOrActivateStatsForm()
    {
        if (statsForm == null || statsForm.IsDisposed)
        {
            statsForm = new StatsForm(keyPressCounter, mouseClickCounter, performanceMonitor);
            statsForm.FormClosed += (s, e) => statsForm = null;
            statsForm.Show();
        }
        else
        {
            statsForm.Activate();
            if (statsForm.WindowState == FormWindowState.Minimized)
            {
                statsForm.WindowState = FormWindowState.Normal;
            }
        }
    }

    /// <summary>
    /// Shows the system performance information.
    /// </summary>
    private void ShowPerformanceInfo()
    {
        string performanceInfo = performanceMonitor.GetPerformanceSummary();
        MessageBox.Show(performanceInfo, "System Performance", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Shows detailed system information.
    /// </summary>
    private void ShowSystemInfo()
    {
        string systemInfo = performanceMonitor.GetSystemInfo();
        MessageBox.Show(systemInfo, "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
    }

    /// <summary>
    /// Shows the top processes by CPU and memory usage.
    /// </summary>
    private void ShowTopProcesses()
    {
        try
        {
            StringBuilder sb = new();
            sb.AppendLine("--- Top Processes by CPU Usage ---");
            
            // Get all processes and order by CPU usage (approximated by total processor time)
            Process[] processes = Process.GetProcesses();
            var orderedByCpu = processes
                .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                .OrderByDescending(p => {
                    try { return p.TotalProcessorTime.TotalMilliseconds; }
                    catch { return 0; }
                })
                .Take(10);
                
            foreach (var proc in orderedByCpu)
            {
                try
                {
                    // Format process info
                    string cpuTime = proc.TotalProcessorTime.TotalSeconds.ToString("F1");
                    string memoryMB = (proc.WorkingSet64 / (1024.0 * 1024.0)).ToString("F1");
                    sb.AppendLine($"{proc.ProcessName} (ID: {proc.Id}): CPU: {cpuTime}s, Memory: {memoryMB} MB");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting process info: {ex.Message}");
                }
            }
            
            sb.AppendLine("\n--- Top Processes by Memory Usage ---");
            var orderedByMemory = processes
                .Where(p => !string.IsNullOrEmpty(p.ProcessName))
                .OrderByDescending(p => {
                    try { return p.WorkingSet64; }
                    catch { return 0; }
                })
                .Take(10);
                
            foreach (var proc in orderedByMemory)
            {
                try
                {
                    string memoryMB = (proc.WorkingSet64 / (1024.0 * 1024.0)).ToString("F1");
                    sb.AppendLine($"{proc.ProcessName} (ID: {proc.Id}): Memory: {memoryMB} MB");
                }
                catch (Exception ex)
                {
                    Debug.WriteLine($"Error getting process info: {ex.Message}");
                }
            }
            
            MessageBox.Show(sb.ToString(), "Top Processes", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error showing top processes: {ex.Message}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Error", $"Could not get process information: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Opens a file in the default associated application.
    /// </summary>
    private void OpenFile(string filePath, string notFoundMessage)
    {
        try
        {
            if (File.Exists(filePath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = filePath,
                    UseShellExecute = true
                });
            }
            else
            {
                trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "File Not Found", notFoundMessage, ToolTipIcon.Warning);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening file: {ex}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Error", $"Could not open file: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Opens the log folder in File Explorer.
    /// </summary>
    private void OpenLogFolder()
    {
        try
        {
            string? directoryPath = Path.GetDirectoryName(logFilePath);
            if (!string.IsNullOrEmpty(directoryPath))
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = directoryPath,
                    UseShellExecute = true
                });
            }
            else
            {
                trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Error", "Could not determine log folder path.", ToolTipIcon.Error);
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error opening log folder: {ex}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Error", $"Could not open log folder: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Handles the Exit menu item click.
    /// </summary>
    private void Exit_Click(object? sender, EventArgs e)
    {
        Application.Exit();
    }

    /// <summary>
    /// Calculates the time remaining until the end of the current day.
    /// </summary>
    private TimeSpan GetRemainingTimeUntilEndOfDay()
    {
        DateTime now = DateTime.Now;
        DateTime endOfDay = new DateTime(now.Year, now.Month, now.Day).AddDays(1);
        return endOfDay - now;
    }

    /// <summary>
    /// Initializes the application context, including the system tray icon.
    /// </summary>
    private void InitializeContext()
    {
        try
        {
            trayIcon = new NotifyIcon()
            {
                Icon = new Icon(Path.Combine(AppDomain.CurrentDomain.BaseDirectory, "favicon.ico")),
                ContextMenuStrip = CreateContextMenu(),
                Visible = true,
                Text = "KeyPressCounter - Double Click for Stats"
            };
            
            trayIcon.DoubleClick += TrayIcon_DoubleClick;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to initialize application context: {ex}");
            throw new ApplicationException($"Failed to initialize application context: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Logs current activity statistics to the log file.
    /// </summary>
    private async void LogActivity(object? sender, ElapsedEventArgs e)
    {
        try
        {
            // Skip logging if the user is idle and idle detection is enabled
            if (config.DetectIdlePeriods && UserIdleDetector.IsUserIdle(config.IdleThresholdSeconds))
            {
                Debug.WriteLine("Skipping logging due to user idle state");
                return;
            }
            
            keyPressCounter.UpdateIntervalMetrics();
            mouseClickCounter.UpdateIntervalMetrics();

            string log = $"{DateTime.Now:yyyy-MM-dd HH:mm:ss}: " +
                         $"Keystrokes: {keyPressCounter.TotalCount}, " +
                         $"Mouse Clicks: {mouseClickCounter.TotalCount}, " +
                         $"Max Keystrokes/Min: {keyPressCounter.MaxPerInterval}, " +
                         $"Max Clicks/Min: {mouseClickCounter.MaxPerInterval}";
            
            await File.AppendAllTextAsync(logFilePath, $"{log}{Environment.NewLine}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to log activity: {ex}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Logging Error", $"Failed to log activity: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Logs daily summary statistics and resets counters for the new day.
    /// </summary>
    private async void LogDailySummary(object? sender, ElapsedEventArgs e)
    {
        try
        {
            double averageClicksPerMinute = mouseClickCounter.TotalCount / 1440.0; // 1440 minutes in a day
            string log = $"{DateTime.Now:yyyy-MM-dd}: " +
                         $"Total Keystrokes: {keyPressCounter.TotalCount}, " +
                         $"Total Mouse Clicks: {mouseClickCounter.TotalCount}, " +
                         $"Avg Clicks/Min: {averageClicksPerMinute:F2}, " +
                         $"Longest No Click Period: {mouseClickCounter.LongestIntervalWithoutIncrement} minutes";
            
            await File.AppendAllTextAsync(dailyLogFilePath, $"{log}{Environment.NewLine}");

            keyPressCounter.ResetTotalMetrics();
            mouseClickCounter.ResetTotalMetrics();

            // Set up the timer for the next day
            SetupDailyLogTimer();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to log daily summary: {ex}");
            trayIcon.ShowBalloonTip(SHORT_NOTIFICATION_DURATION, "Logging Error", $"Failed to log daily summary: {ex.Message}", ToolTipIcon.Error);
        }
    }

    /// <summary>
    /// Sets up the daily log timer to trigger at the end of the day.
    /// </summary>
    private void SetupDailyLogTimer()
    {
        dailyLogTimer?.Dispose();
        dailyLogTimer = new System.Timers.Timer(GetRemainingTimeUntilEndOfDay().TotalMilliseconds);
        dailyLogTimer.Elapsed += LogDailySummary;
        dailyLogTimer.AutoReset = false; // Only trigger once
        dailyLogTimer.Enabled = true;
    }

    /// <summary>
    /// Sets up the activity log timer to trigger at regular intervals.
    /// </summary>
    private void SetupLogTimer()
    {
        logTimer = new System.Timers.Timer(config.LogIntervalSeconds * 1000); // Convert seconds to milliseconds
        logTimer.Elapsed += LogActivity;
        logTimer.AutoReset = true;
        logTimer.Enabled = true;
    }

    /// <summary>
    /// Initializes the global keyboard and mouse hooks.
    /// </summary>
    private void StartGlobalHooks()
    {
        try
        {
            globalHook = Hook.GlobalEvents();
            
            globalHook.KeyPress += (sender, e) => 
            {
                // Skip tracking if user is idle and idle detection is enabled
                if (config.DetectIdlePeriods && UserIdleDetector.IsUserIdle(config.IdleThresholdSeconds))
                    return;
                    
                keyPressCounter.Increment();
            };
            
            globalHook.MouseClick += (sender, e) => 
            {
                // Skip tracking if user is idle and idle detection is enabled
                if (config.DetectIdlePeriods && UserIdleDetector.IsUserIdle(config.IdleThresholdSeconds))
                    return;
                    
                mouseClickCounter.Increment();
            };
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Failed to set up global hooks: {ex}");
            throw new ApplicationException($"Failed to set up global hooks: {ex.Message}", ex);
        }
    }

    /// <summary>
    /// Handles the double-click event for the system tray icon.
    /// </summary>
    private void TrayIcon_DoubleClick(object? sender, EventArgs e)
    {
        // Open the statistics form instead of just showing a balloon tip
        ShowOrActivateStatsForm();
    }

    /// <summary>
    /// Updates the system tray icon tooltip with current statistics.
    /// </summary>
    private void UpdateTrayIconText()
    {
        StringBuilder sb = new();
        
        // Input statistics
        sb.AppendLine($"Keystrokes: {keyPressCounter.TotalCount} (Max/min: {keyPressCounter.MaxPerInterval})");
        sb.AppendLine($"Mouse Clicks: {mouseClickCounter.TotalCount} (Max/min: {mouseClickCounter.MaxPerInterval})");
        
        // Include basic system performance info
        sb.AppendLine($"CPU: {performanceMonitor.CpuUsagePercent:F1}%");
        sb.AppendLine($"Memory: {performanceMonitor.MemoryUsagePercent:F1}%");
                     
        trayIcon.ShowBalloonTip(LONG_NOTIFICATION_DURATION, "KeyPressCounter Stats", sb.ToString(), ToolTipIcon.Info);
    }

    #region IDisposable Implementation

    /// <summary>
    /// Disposes of resources used by the application context.
    /// </summary>
    public new void Dispose()
    {
        Dispose(true);
        GC.SuppressFinalize(this);
    }

    /// <summary>
    /// Releases the unmanaged resources used by the application context and optionally releases the managed resources.
    /// </summary>
    /// <param name="disposing">true to release both managed and unmanaged resources; false to release only unmanaged resources.</param>
    protected override void Dispose(bool disposing)
    {
        if (!disposed)
        {
            if (disposing)
            {
                // Dispose managed resources
                statsForm?.Dispose();
                trayIcon?.Dispose();
                logTimer?.Dispose();
                dailyLogTimer?.Dispose();
                globalHook?.Dispose();
                performanceMonitor?.Dispose();
            }

            // Set the flag to prevent redundant calls
            disposed = true;
        }
        
        base.Dispose(disposing);
    }
    
    /// <summary>
    /// Finalizer to ensure resources are released when the object is garbage collected.
    /// </summary>
    ~CustomApplicationContext()
    {
        Dispose(false);
    }

    #endregion
}
