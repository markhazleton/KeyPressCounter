# KeyPressCounter ⌨️🖱️📊

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)
![C#](https://img.shields.io/badge/C%23-13-239120)

A lightweight Windows system tray utility that monitors keyboard and mouse input patterns alongside real-time system performance metrics. KeyPressCounter runs silently in the background, tracking your activity without recording what you type and without transmitting any data.

## Features

### Input Monitoring
- **Keystroke & Mouse Click Tracking** — counts total inputs since last reset
- **Peak Activity Metrics** — tracks maximum keystrokes and clicks per minute
- **Inactivity Detection** — measures longest continuous idle period in minutes
- **Idle Time Filtering** — optionally exclude idle periods from statistics (configurable threshold, default 5 minutes)

### System Performance Monitoring
- **Real-Time Metrics** — CPU usage, memory utilization, disk read/write speeds, network upload/download rates
- **Historical Graphs** — 60-second rolling line graphs for CPU and memory (dark-themed, anti-aliased)
- **System Uptime** — current uptime displayed in the performance dashboard
- **Hardware Information** — processor details, total RAM, GPU, disk configuration via WMI
- **Process Monitor** — top 10 processes ranked by CPU and memory consumption

### Statistics Dashboard
Three-tab interface, opened by double-clicking the tray icon or via the context menu:

1. **Input Statistics** — large-format keystroke and mouse click totals with peak rate and idle tracking
2. **System Performance** — real-time CPU/memory gauges, disk/network metrics, and the 60-second historical graph
3. **About** — application info, author credits, technology stack, and privacy statement

### System Tray Integration
- Runs unobtrusively with a rich right-click context menu
- Double-click tray icon to open the statistics dashboard
- Balloon tip notifications for key events
- Single-instance enforcement — prevents duplicate processes

### Logging
- **Activity Log** — records counters at a configurable interval (default: 60 seconds)
- **Daily Summary Log** — end-of-day statistics appended automatically; counters reset at midnight
- **Log Folder Access** — open the log directory directly from the tray menu

### Settings
- **Start with Windows** — writes to `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`; synchronized with registry on each launch
- **Detect Idle Periods** — toggle idle filtering on/off; threshold configurable in `config.json`
- **Configuration persistence** — JSON file at `%APPDATA%\MWH.KeyPressCounter\config.json`

### System Tools (Quick Launch)
Launch built-in Windows utilities directly from the tray menu:
- Task Manager (`taskmgr.exe`)
- Resource Monitor (`resmon.exe`)
- Performance Monitor (`perfmon.exe`)
- System Information (`msinfo32.exe`)

---

## Prerequisites

- Windows 10 or later (x64)
- [.NET 10.0 Runtime](https://dotnet.microsoft.com/download/dotnet/10.0)
- Administrative permissions may be required for some performance counter categories

---

## Installation

1. Download the latest `KeyPressCounter-vX.X.X-win-x64.zip` from the [Releases](https://github.com/MarkHazleton/KeyPressCounter/releases) page
2. Extract to any folder
3. Run `MWH.KeyPressCounter.exe`
4. The application appears in the system tray immediately

To start automatically with Windows, enable **Settings → Start with Windows** from the tray menu.

---

## Usage

| Action | Result |
|---|---|
| Double-click tray icon | Opens the statistics dashboard |
| Right-click tray icon | Opens the context menu |
| **Statistics → Input Statistics** | Dashboard — Input Statistics tab |
| **Statistics → System Performance** | Shows current metrics in a message box |
| **Statistics → System Information** | Shows hardware details |
| **Statistics → Running Processes** | Top 10 processes by CPU and memory |
| **Logs → Open Activity Log** | Opens `ActivityLog.txt` in default text editor |
| **Logs → Open Daily Summary Log** | Opens `DailySummaryLog.txt` |
| **Logs → Open Log Folder** | Opens the log directory in Explorer |
| **Settings → Start with Windows** | Toggles Windows startup registration |
| **Settings → Detect Idle Periods** | Toggles idle-period filtering |
| **Reset Counters** | Zeroes all counters after confirmation |
| **Exit** | Closes the application |

---

## Log Files

Both files are created in `Documents\` by default (configurable):

| File | Content | When written |
|---|---|---|
| `ActivityLog.txt` | Timestamp, total keystrokes, total clicks, peak rates | Every 60 seconds (configurable) |
| `DailySummaryLog.txt` | Daily totals, average clicks/min, longest idle period | Once per day at midnight |

---

## Configuration

Stored at `%APPDATA%\MWH.KeyPressCounter\config.json`. Created with defaults on first run.

| Setting | Default | Description |
|---|---|---|
| `LogDirectory` | `Documents\` | Where log files are written |
| `ActivityLogFileName` | `ActivityLog.txt` | Activity log file name |
| `DailySummaryLogFileName` | `DailySummaryLog.txt` | Daily summary file name |
| `LogIntervalSeconds` | `60` | Seconds between activity log entries |
| `StartWithWindows` | `false` | Synced with registry on each launch |
| `MinimizeToTrayOnStart` | `true` | Start minimized to tray |
| `DetectIdlePeriods` | `true` | Exclude idle time from statistics |
| `IdleThresholdSeconds` | `300` | Seconds before user is considered idle |

---

## Building from Source

```bash
git clone https://github.com/MarkHazleton/KeyPressCounter.git
cd KeyPressCounter
dotnet build MWH.KeyPressCounter.csproj --configuration Release
dotnet run --project MWH.KeyPressCounter.csproj
```

### Dependencies

| Package | Version | Purpose |
|---|---|---|
| [SharpHook](https://github.com/TolikPylypchuk/SharpHook) | 7.1.1 | Global keyboard and mouse event hooking |
| [System.Management](https://www.nuget.org/packages/System.Management) | 10.0.3 | WMI access for hardware information |

Windows Performance Counters (`System.Diagnostics.PerformanceCounter`) and User32 P/Invoke (`GetLastInputInfo`) are part of the .NET Windows platform and require no additional packages.

---

## Project Structure

| File | Responsibility |
|---|---|
| `Program.cs` | Entry point; single-instance check, global exception handlers, `Application.Run` |
| `CustomApplicationContext.cs` | `ApplicationContext` subclass — owns the tray icon, context menu, timers, and global hooks |
| `Counter.cs` | Thread-safe counter with lock-protected increment, per-interval peak tracking, and idle-interval measurement |
| `AppConfig.cs` | JSON configuration (load/save to `%APPDATA%`) and Windows startup registry management |
| `StatsForm.cs` | Three-tab WinForms statistics dashboard; 1-second UI refresh timer; GDI+ graph rendering |
| `SystemPerformanceMonitor.cs` | Wraps seven Windows Performance Counters; caches static hardware info at startup; `IDisposable` |
| `UserIdleDetector.cs` | Static P/Invoke wrapper around `GetLastInputInfo` (User32.dll) |

---

## How It Works

### 1. Input Tracking

`TaskPoolGlobalHook` (SharpHook) runs on a background thread and raises `KeyPressed` and `MousePressed` events. Each event calls `Counter.Increment()`, which holds a `lock` while incrementing `currentCount` and `totalCount`. No key identities, scan codes, or cursor positions are stored — only the running integer totals.

### 2. Peak Rate & Idle Measurement

Every `LogIntervalSeconds` (default: 60), the activity timer calls `Counter.UpdateIntervalMetrics()`:
- If `currentCount` beats the stored `MaxPerInterval`, it replaces it (this becomes the "peak rate")
- If `currentCount` is zero the interval is counted as idle; if it extends the `LongestIntervalWithoutIncrement` record, that record is updated
- `currentCount` is reset to zero for the next interval

Because the default interval is 60 seconds, peak rates are effectively per-minute. Changing `LogIntervalSeconds` scales the measurement accordingly.

### 3. Idle Detection

`UserIdleDetector.GetIdleTimeSeconds()` P/Invokes `GetLastInputInfo` from `User32.dll` and computes `(Environment.TickCount - lastInputInfo.dwTime) / 1000`. When the result meets or exceeds `IdleThresholdSeconds` (default: 300 s), the global hook event handlers return early without calling `Increment()`, and the activity logger skips writing an entry.

### 4. Performance Monitoring

`SystemPerformanceMonitor` initialises seven `PerformanceCounter` objects at startup and reads them on demand (every 1 second when the StatsForm refresh timer fires):

| Counter category | Counter name | Instance | Exposed as |
|---|---|---|---|
| `Processor` | `% Processor Time` | `_Total` | `CpuUsagePercent` |
| `Memory` | `Available MBytes` | _(none)_ | `AvailableMemoryMB` |
| `System` | `System Up Time` | _(none)_ | `SystemUptime` |
| `PhysicalDisk` | `Disk Read Bytes/sec` | `_Total` | `DiskReadKBPerSec` ÷ 1 024 |
| `PhysicalDisk` | `Disk Write Bytes/sec` | `_Total` | `DiskWriteKBPerSec` ÷ 1 024 |
| `Network Interface` | `Bytes Sent/sec` | primary adapter | `NetworkUploadKBPerSec` ÷ 1 024 |
| `Network Interface` | `Bytes Received/sec` | primary adapter | `NetworkDownloadKBPerSec` ÷ 1 024 |

The primary network adapter is resolved at startup via `System.Net.NetworkInformation.NetworkInterface`: the first non-loopback, operationally-up adapter that has sent at least one byte is selected. If that fails, it falls back to the first instance returned by the `Network Interface` performance counter category.

### 5. Hardware Information

Static hardware details are read once at construction and cached in fields:

| Data | Source |
|---|---|
| OS name | Registry — `HKLM\SOFTWARE\Microsoft\Windows NT\CurrentVersion\ProductName` + `Environment.OSVersion` |
| CPU name | Registry — `HKLM\HARDWARE\DESCRIPTION\System\CentralProcessor\0\ProcessorNameString` |
| Processor count | `Environment.ProcessorCount` |
| Total RAM | WMI — `Win32_ComputerSystem.TotalPhysicalMemory` |
| GPU name | WMI — `Win32_VideoController.Name` _(queried on demand in GetSystemInfo)_ |
| Disk space | `DriveInfo.GetDrives()` _(queried on demand in GetSystemInfo)_ |

### 6. Logging

Two `System.Timers.Timer` instances fire on the thread pool:

- **Activity timer** — fires every `LogIntervalSeconds` seconds; calls `Counter.UpdateIntervalMetrics()` on both counters then appends one line to `ActivityLog.txt` via `File.AppendAllTextAsync`. Skipped entirely when the user is idle and idle detection is enabled.
- **Daily summary timer** — fires once at midnight (interval set to remaining milliseconds in the current day); appends one summary line to `DailySummaryLog.txt`, resets both counters, then re-arms itself for the next midnight.

### 7. Single Instance

At startup, `Process.GetProcessesByName(currentProcess.ProcessName)` is called. If more than one matching process is found, a message box is shown and the new instance exits immediately without reaching `Application.Run`.

### 8. Configuration

`System.Text.Json` serializes `AppConfig` to a JSON file at `%APPDATA%\MWH.KeyPressCounter\config.json`. On every load, `SyncStartupSetting()` reads `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run` and reconciles the stored `StartWithWindows` flag with the actual registry state, saving the corrected value if they differ.

---

## Privacy & Ethics

- **No keystroke content recorded** — only counts are stored
- **Local storage only** — no data leaves the machine
- **No network access** — the application makes no outbound connections
- **Transparent operation** — all monitoring is visible via the tray icon and context menu

When using this tool in an organizational context, obtain explicit consent from monitored users and ensure compliance with applicable privacy laws.

---

## Known Issues & Limitations

- Network interface performance counters may fail on some systems; the application falls back to the first available interface automatically
- GPU information retrieval via WMI may return "Unknown" on some hardware configurations
- Some performance counter categories require elevated permissions; run as Administrator if metrics show as unavailable
- The application relies on `favicon.ico` being present in the application directory; a missing icon file will log a warning and use the default Windows Forms icon

---

## Troubleshooting

**Application won't start**
- Confirm .NET 10.0 Runtime is installed (`dotnet --version`)
- Check the system tray for an existing instance before launching again

**Performance counters unavailable**
- Run as Administrator
- Rebuild the performance counter library from an elevated command prompt: `lodctr /r`

**"Start with Windows" not persisting**
- Verify the application has write access to `HKCU\SOFTWARE\Microsoft\Windows\CurrentVersion\Run`

**Log files not created**
- Check write permissions on the `Documents` folder (or configured `LogDirectory`)

---

## Contributing

1. Fork the repository
2. Create a feature branch (`git checkout -b feature/your-feature`)
3. Commit your changes (`git commit -m 'Add your feature'`)
4. Push to the branch (`git push origin feature/your-feature`)
5. Open a Pull Request

**Development guidelines:**
- Maintain thread safety for all counter and timer interactions
- Use XML doc comments on all public members
- Dispose performance counters, registry keys, and WMI objects explicitly
- Test on multiple Windows versions

---

## License

MIT License — see [LICENSE](LICENSE) for details.

## Contact

**Mark Hazleton** — [github.com/MarkHazleton](https://github.com/MarkHazleton)

Project: [github.com/MarkHazleton/KeyPressCounter](https://github.com/MarkHazleton/KeyPressCounter)
Issues: [github.com/MarkHazleton/KeyPressCounter/issues](https://github.com/MarkHazleton/KeyPressCounter/issues)
