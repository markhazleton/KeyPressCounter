using System.Text.Json;
using Microsoft.Win32;

namespace MWH.KeyPressCounter;

/// <summary>
/// Provides configuration management for the KeyPressCounter application.
/// </summary>
public class AppConfig
{
    // Constants for configuration
    private const string APP_NAME = "MWH.KeyPressCounter";
    private const string RUN_REGISTRY_KEY = @"SOFTWARE\Microsoft\Windows\CurrentVersion\Run";

    // Default configuration values
    /// <summary>
    /// Gets or sets the directory where log files are stored.
    /// </summary>
    public string LogDirectory { get; set; } = Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments);

    /// <summary>
    /// Gets or sets the filename for the activity log file.
    /// </summary>
    public string ActivityLogFileName { get; set; } = "ActivityLog.txt";

    /// <summary>
    /// Gets or sets the filename for the daily summary log file.
    /// </summary>
    public string DailySummaryLogFileName { get; set; } = "DailySummaryLog.txt";

    /// <summary>
    /// Gets or sets the interval in seconds between activity log entries.
    /// </summary>
    public int LogIntervalSeconds { get; set; } = 60;

    /// <summary>
    /// Gets or sets whether the application should start with Windows.
    /// </summary>
    public bool StartWithWindows { get; set; } = false;

    /// <summary>
    /// Gets or sets whether the application should minimize to the system tray on startup.
    /// </summary>
    public bool MinimizeToTrayOnStart { get; set; } = true;

    /// <summary>
    /// Gets or sets whether to detect and ignore periods of user inactivity.
    /// </summary>
    public bool DetectIdlePeriods { get; set; } = true;

    /// <summary>
    /// Gets or sets the idle threshold in seconds before user is considered inactive.
    /// </summary>
    public int IdleThresholdSeconds { get; set; } = 300; // 5 minutes

    /// <summary>
    /// Path to the configuration file in the user's AppData folder.
    /// </summary>
    private static readonly string ConfigFilePath = Path.Combine(
        Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData),
        APP_NAME,
        "config.json");

    /// <summary>
    /// Gets the full path to the activity log file.
    /// </summary>
    /// <returns>The full path to the activity log file.</returns>
    public string GetActivityLogPath()
    {
        return Path.Combine(LogDirectory, ActivityLogFileName);
    }

    /// <summary>
    /// Gets the full path to the daily summary log file.
    /// </summary>
    /// <returns>The full path to the daily summary log file.</returns>
    public string GetDailySummaryLogPath()
    {
        return Path.Combine(LogDirectory, DailySummaryLogFileName);
    }

    /// <summary>
    /// Loads the application configuration from the configuration file.
    /// If the file doesn't exist or is invalid, creates and returns default configuration.
    /// </summary>
    /// <returns>The loaded configuration or a default configuration if loading fails.</returns>
    public static AppConfig Load()
    {
        try
        {
            if (!File.Exists(ConfigFilePath))
            {
                return CreateDefaultConfig();
            }

            string json = File.ReadAllText(ConfigFilePath);
            var config = JsonSerializer.Deserialize<AppConfig>(json);
            if (config == null)
            {
                System.Diagnostics.Debug.WriteLine("Failed to deserialize configuration, using defaults");
                return CreateDefaultConfig();
            }

            // Ensure Windows startup setting is in sync with registry
            config.SyncStartupSetting();
            return config;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to load configuration: {ex.Message}");
            return CreateDefaultConfig();
        }
    }

    /// <summary>
    /// Applies the startup with Windows setting to the Windows registry.
    /// </summary>
    public void ApplyStartupSetting()
    {
        try
        {
            SetStartWithWindows(StartWithWindows);
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to apply startup setting: {ex.Message}");
        }
    }

    /// <summary>
    /// Synchronizes the StartWithWindows property with the actual registry setting.
    /// </summary>
    private void SyncStartupSetting()
    {
        try
        {
            using var key = Registry.CurrentUser.OpenSubKey(RUN_REGISTRY_KEY);
            bool registryHasEntry = key?.GetValue(APP_NAME) != null;
            
            // If the settings don't match, prioritize the registry setting
            if (StartWithWindows != registryHasEntry)
            {
                StartWithWindows = registryHasEntry;
                Save(); // Update the saved configuration to match reality
            }
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to sync startup setting: {ex.Message}");
        }
    }

    /// <summary>
    /// Sets or removes the application from Windows startup.
    /// </summary>
    /// <param name="enable">True to enable startup with Windows, false to disable.</param>
    private static void SetStartWithWindows(bool enable)
    {
        string appPath = Application.ExecutablePath;
        using var key = Registry.CurrentUser.OpenSubKey(RUN_REGISTRY_KEY, true);
        
        if (key == null)
        {
            throw new InvalidOperationException("Could not access Windows registry startup key");
        }

        if (enable)
        {
            key.SetValue(APP_NAME, appPath);
        }
        else
        {
            if (key.GetValue(APP_NAME) != null)
            {
                key.DeleteValue(APP_NAME, false);
            }
        }
    }

    /// <summary>
    /// Saves the current configuration to the configuration file.
    /// </summary>
    /// <returns>True if the save was successful, false otherwise.</returns>
    public bool Save()
    {
        try
        {
            string? directoryPath = Path.GetDirectoryName(ConfigFilePath);
            if (!string.IsNullOrEmpty(directoryPath) && !Directory.Exists(directoryPath))
            {
                Directory.CreateDirectory(directoryPath);
            }

            var options = new JsonSerializerOptions 
            { 
                WriteIndented = true
            };
            
            string json = JsonSerializer.Serialize(this, options);
            File.WriteAllText(ConfigFilePath, json);
            return true;
        }
        catch (Exception ex)
        {
            System.Diagnostics.Debug.WriteLine($"Failed to save configuration: {ex.Message}");
            return false;
        }
    }

    /// <summary>
    /// Creates a default configuration and saves it to the configuration file.
    /// </summary>
    /// <returns>The created default configuration.</returns>
    private static AppConfig CreateDefaultConfig()
    {
        var config = new AppConfig();
        config.Save();
        return config;
    }
}