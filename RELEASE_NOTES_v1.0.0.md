# KeyPressCounter v1.0.0 ??

## Initial Release

This is the first official release of KeyPressCounter, a comprehensive Windows utility for monitoring keyboard/mouse activities and system performance.

### ?? Features

#### Input Monitoring
- **Keystroke & Mouse Click Tracking**: Real-time counting and analysis
- **Activity Metrics**: Maximum keystrokes/clicks per minute tracking
- **Inactivity Detection**: Automatic identification of idle periods
- **Idle Time Filtering**: Option to exclude idle periods from statistics

#### System Performance Monitoring
- **Real-time Metrics**: CPU, memory, disk I/O, and network monitoring
- **Performance Graphs**: Visual 60-second historical data
- **Hardware Information**: Detailed CPU, RAM, GPU, and disk information
- **Process Monitoring**: Top CPU and memory-consuming processes

#### User Interface
- **System Tray Integration**: Unobtrusive operation with rich context menu
- **Statistics Dashboard**: Tabbed interface with real-time updates
- **Performance Charts**: Smooth anti-aliased line graphs
- **Quick Access Tools**: Launch Task Manager, Resource Monitor, Performance Monitor, and System Information

#### Data Management
- **Automated Logging**: Configurable 60-second interval logging
- **Daily Summaries**: End-of-day statistics with automatic reset
- **Configuration Persistence**: JSON-based settings in AppData
- **Local Storage**: All data stored locally for privacy

### ?? Technical Specifications

- **Framework**: .NET 10.0
- **Platform**: Windows (x64)
- **Dependencies**:
  - SharpHook v7.1.0
  - System.Management v10.0.0
  - System.Diagnostics.PerformanceCounter v10.0.0

### ?? System Requirements

- Windows OS (Windows 10 or later recommended)
- .NET 10.0 Runtime
- ~120 MB disk space for application

### ?? Installation

1. Download `KeyPressCounter-v1.0.0-win-x64.zip`
2. Extract to your preferred location
3. Run `MWH.KeyPressCounter.exe`
4. (Optional) Enable "Start with Windows" in settings

### ?? Known Issues

- Network interface performance counters may fail on some systems (automatic fallback available)
- GPU information retrieval may not work on all hardware configurations
- Performance counter initialization requires sufficient system permissions

### ?? Privacy

- No keystroke content is recorded (only counts)
- All data stored locally
- No network transmission
- Full transparency of monitoring activities

### ?? Support

- Report issues: [GitHub Issues](https://github.com/MarkHazleton/KeyPressCounter/issues)
- Documentation: [README.md](https://github.com/MarkHazleton/KeyPressCounter/blob/main/README.md)

---

**Full Changelog**: https://github.com/MarkHazleton/KeyPressCounter/commits/v1.0.0
