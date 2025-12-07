# KeyPressCounter 🖱️⌨️📊

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![.NET](https://img.shields.io/badge/.NET-10.0-512BD4)
![Platform](https://img.shields.io/badge/platform-Windows-lightgrey)

A comprehensive Windows utility for monitoring keyboard/mouse activities and system performance. KeyPressCounter runs silently in the system tray, tracking user interaction patterns and system metrics without interfering with your workflow.

<p align="center">
  <img src="https://via.placeholder.com/600x300?text=KeyPressCounter+Screenshot" alt="KeyPressCounter Screenshot" width="600">
</p>

## 🌟 Features

### Input Monitoring
- **Keystroke & Mouse Click Tracking**: Count and analyze keyboard and mouse usage patterns
- **Activity Metrics**: View maximum keystrokes/clicks per minute and total counts
- **Inactivity Detection**: Identify and measure periods of user inactivity
- **Idle Time Filtering**: Option to automatically exclude periods of inactivity from statistics

### System Performance Monitoring
- **Real-time System Metrics**:
  - CPU usage percentage with historical trends
  - Memory utilization and availability
  - Disk read/write speeds
  - Network upload/download rates
  - System uptime tracking
- **Hardware Information**:
  - Processor details and core count
  - Total installed RAM
  - GPU information
  - Disk space utilization
- **Process Monitoring**: Track top CPU and memory-consuming processes

### Visualization and Statistics
- **Graphical Interface**: View statistics in a tabbed interface with real-time graphs
- **Performance Charts**: Visual representation of CPU and memory usage over time
- **Comprehensive Statistics**: Detailed breakdown of usage patterns and system performance
- **Exportable Data**: All metrics are logged for analysis and record-keeping

### System Integration
- **System Tray Operation**: Runs unobtrusively in the system tray with rich context menu
- **Windows Startup Integration**: Option to launch automatically when Windows starts
- **Quick Access to System Tools**: Launch built-in Windows utilities directly from the app:
  - Task Manager
  - Resource Monitor
  - Performance Monitor
  - System Information

### Logging and Data Management
- **Detailed Activity Logs**: Automated logging of activity metrics at configurable intervals (default: 60 seconds)
- **Daily Summary Reports**: Comprehensive end-of-day statistics with automatic reset
- **Local Data Storage**: All data stored locally for privacy and security (Documents folder by default)
- **Configuration Management**: JSON-based configuration stored in AppData with settings persistence
- **Privacy-Focused Design**: Counts keystrokes without recording key content

### Advanced Features
- **Single Instance Protection**: Prevents multiple instances from running simultaneously
- **User Idle Detection**: Native Windows API integration for accurate idle time detection
- **Registry Integration**: Windows startup configuration managed through registry
- **Exception Handling**: Comprehensive error handling with detailed logging and user notifications
- **Configurable Idle Threshold**: Customizable idle detection period (default: 5 minutes)

## 📋 Prerequisites

- Windows OS
- .NET 10.0 Runtime
- Administrative permissions may be required for some performance monitoring features

## 🚀 Installation

1. **Download**: Get the latest release from the [Releases](https://github.com/MarkHazleton/KeyPressCounter/releases) page
2. **Install**: Extract the ZIP file to your preferred location
3. **Run**: Execute `MWH.KeyPressCounter.exe`

To have KeyPressCounter start automatically with Windows:
- Enable the "Start with Windows" option in the application's settings menu, or
- Create a shortcut in the Windows Startup folder (`shell:startup`)

## 💻 Usage

### Basic Operations
- **Launch**: Run the application to start monitoring (appears in system tray)
- **View Dashboard**: Double-click the tray icon to open the statistics dashboard
- **Quick Stats**: Hover over the tray icon to see basic metrics in a tooltip

### Tray Menu Options

#### Statistics Menu
- **Input Statistics**: Opens the statistics dashboard focused on keystroke/mouse data
- **System Performance**: Shows current system performance metrics
- **System Information**: Displays detailed hardware specifications
- **Running Processes**: Lists top CPU and memory-consuming processes

#### Logs Menu
- **Open Activity Log**: View the detailed activity log file
- **Open Daily Summary Log**: View the daily statistical summaries
- **Open Log Folder**: Access the directory containing all log files

#### Settings Menu
- **Start with Windows**: Toggle automatic startup with Windows
- **Detect Idle Periods**: Enable/disable filtering out idle time from statistics

#### System Tools Menu
- **Task Manager**: Launch Windows Task Manager
- **Resource Monitor**: Open Resource Monitor for detailed system analysis
- **Performance Monitor**: Access Windows Performance Monitor
- **System Information**: View detailed system information utility

#### Other Options
- **Reset Counters**: Zero all statistics counters
- **Exit**: Close the application

### Statistics Dashboard
The dashboard provides a tabbed interface with:

1. **Input Statistics Tab**:
   - Total keystrokes and mouse clicks since last reset
   - Maximum keystrokes and clicks per minute (peak activity)
   - Longest period of inactivity (in minutes)
   - Real-time counter updates

2. **System Performance Tab**:
   - Current CPU and memory usage with real-time line graphs
   - Historical data visualization (last 60 seconds)
   - Disk read/write speeds in KB/s
   - Network upload/download rates in KB/s
   - System uptime display
   - Direct access to detailed system information
   - Graph legend with color-coded metrics
   - Auto-refresh every second

### Log Files

KeyPressCounter creates two log files in your Documents folder by default:

- `ActivityLog.txt`: Detailed activity records logged at 60-second intervals (configurable)
- `DailySummaryLog.txt`: End-of-day statistical summaries with automatic daily reset

### Configuration File

Configuration is stored in JSON format at:
- Location: `%APPDATA%\MWH.KeyPressCounter\config.json`
- Automatically created with defaults on first run
- Includes settings for:
  - Log directory path
  - Log file names
  - Logging interval (seconds)
  - Startup with Windows preference
  - Idle detection settings
  - Idle threshold (seconds)

## 🛠️ Building from Source

```bash
# Clone the repository
git clone https://github.com/MarkHazleton/KeyPressCounter.git

# Navigate to the project directory
cd KeyPressCounter

# Build the project
dotnet build MWH.KeyPressCounter.csproj --configuration Release

# Run the application
dotnet run --project MWH.KeyPressCounter.csproj
```

### Dependencies
The project uses the following NuGet packages:
- **SharpHook** (v7.1.0) - Global keyboard and mouse event hooking
- **System.Management** (v10.0.0) - WMI access for hardware information
- **System.Diagnostics.PerformanceCounter** (v10.0.0) - System performance metrics

## 🔍 How It Works

KeyPressCounter combines several technologies to provide comprehensive monitoring:

1. **Input Tracking**: Uses the [SharpHook](https://github.com/TolikPylypchuk/SharpHook) library (v7.1.0) with TaskPoolGlobalHook for efficient global keyboard and mouse event monitoring
2. **Performance Monitoring**: Leverages Windows Performance Counters (System.Diagnostics.PerformanceCounter) for real-time system metrics including:
   - CPU usage (Processor - % Processor Time)
   - Memory availability (Memory - Available MBytes)
   - System uptime (System - System Up Time)
   - Disk I/O (PhysicalDisk - Bytes/sec)
   - Network activity (Network Interface - Bytes Sent/Received per sec)
3. **System Information**: Utilizes Windows Management Instrumentation (WMI) via System.Management for hardware details:
   - CPU name and specifications
   - Total physical memory
   - GPU information
   - Drive space and configuration
4. **Idle Detection**: Integrates with Windows User32.dll API using GetLastInputInfo for accurate user idle time calculation
5. **Process Analysis**: Uses the .NET Process API to monitor and display top CPU and memory-consuming applications
6. **Data Visualization**: Implements custom GDI+ drawing routines with anti-aliasing for smooth real-time performance graphs
7. **Configuration Management**: JSON serialization for settings persistence with automatic Registry integration for Windows startup
8. **Single Instance Enforcement**: Process enumeration to prevent multiple application instances

All data is processed through thread-safe Counter classes with proper locking mechanisms and displayed in an intuitive Windows Forms interface.

## 🔒 Privacy & Ethics

KeyPressCounter is designed with privacy as a top priority:

- **No Keystroke Content**: The application only counts keystrokes; it does not record which keys are pressed
- **Local Storage Only**: All data remains on your local machine
- **No Network Access**: The application does not transmit any data
- **Transparent Operation**: All monitoring activities are clearly indicated

### Important Notice

When using this tool in organizational settings, ensure you:

- Obtain explicit consent from users being monitored
- Comply with all applicable privacy laws and regulations
- Use the data only for legitimate purposes such as ergonomic studies or productivity analysis

## 🤝 Contributing

Contributions are welcome! Please feel free to submit a Pull Request.

1. Fork the repository
2. Create your feature branch (`git checkout -b feature/amazing-feature`)
3. Commit your changes (`git commit -m 'Add some amazing feature'`)
4. Push to the branch (`git push origin feature/amazing-feature`)
5. Open a Pull Request

### Development Guidelines
- Maintain thread-safe operations for all counter modifications
- Follow existing code documentation standards with XML comments
- Ensure proper disposal of system resources (Performance Counters, WMI objects)
- Test with different Windows versions and .NET runtime versions
- Maintain backward compatibility for configuration files

## 🐛 Known Issues & Limitations

- Network interface performance counters may fail on some systems; the app will fallback to the first available interface
- GPU information retrieval may not work on all hardware configurations
- Performance counter initialization requires sufficient system permissions
- The application icon (favicon.ico) must be present in the application directory

## 📊 Performance Considerations

- Logging interval can be adjusted to reduce I/O operations (default: 60 seconds)
- Performance counter polling runs at 1-second intervals for the dashboard
- Historical graph data is limited to 60 data points to minimize memory usage
- Global hook events are processed asynchronously to prevent input lag
- Idle time detection uses native Windows API for minimal overhead

## 🔧 Troubleshooting

### Application Won't Start
- Ensure .NET 10.0 runtime is installed
- Check that no other instance is already running (look in system tray)
- Verify favicon.ico exists in the application directory

### Performance Counters Not Working
- Run as Administrator for full performance monitoring capabilities
- Ensure Windows Performance Counter service is running
- Rebuild performance counter library: `lodctr /r` in elevated command prompt

### Configuration Not Saving
- Check write permissions to `%APPDATA%\MWH.KeyPressCounter\` directory
- Verify disk space is available
- Review application logs for serialization errors

### Network Counters Failing
- Verify network interfaces are properly configured in Windows
- Check Network Interface performance counter category exists
- Application will attempt to use fallback interface automatically

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

Mark Hazleton - GitHub: [@MarkHazleton](https://github.com/MarkHazleton)

Project Link: [https://github.com/MarkHazleton/KeyPressCounter](https://github.com/MarkHazleton/KeyPressCounter)

---

**Note**: This tool is intended for personal productivity tracking and system monitoring. Always ensure compliance with local laws and organizational policies regarding monitoring software.
