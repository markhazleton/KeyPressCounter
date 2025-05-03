# KeyPressCounter 🖱️⌨️📊

[![License: MIT](https://img.shields.io/badge/License-MIT-blue.svg)](LICENSE)
![.NET](https://img.shields.io/badge/.NET-9.0-512BD4)
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
- **Detailed Activity Logs**: Automated logging of activity metrics at configurable intervals
- **Daily Summary Reports**: Comprehensive end-of-day statistics
- **Local Data Storage**: All data stored locally for privacy and security
- **Privacy-Focused Design**: Counts keystrokes without recording key content

## 📋 Prerequisites

- Windows OS
- .NET 9.0 Runtime
- Administrative permissions (for global keyboard hooking and performance monitoring)

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
   - Total keystrokes and mouse clicks
   - Maximum keystrokes and clicks per minute
   - Longest period of inactivity

2. **System Performance Tab**:
   - Current CPU and memory usage with real-time graphs
   - Disk and network activity metrics
   - System uptime
   - Button to view detailed system information

### Log Files

KeyPressCounter creates two log files in your Documents folder by default:

- `ActivityLog.txt`: Detailed activity records at configured intervals
- `DailySummaryLog.txt`: End-of-day statistical summaries

## 🛠️ Building from Source

```bash
# Clone the repository
git clone https://github.com/MarkHazleton/KeyPressCounter.git

# Navigate to the project directory
cd KeyPressCounter

# Build the project
dotnet build KeyLogger.sln --configuration Release

# Run the application
dotnet run --project MWH.KeyPressCounter.csproj
```

## 🔍 How It Works

KeyPressCounter combines several technologies to provide comprehensive monitoring:

1. **Input Tracking**: Uses the [MouseKeyHook](https://github.com/gmamaladze/globalmousekeyhook) library to establish global keyboard and mouse hooks
2. **Performance Monitoring**: Leverages Windows Performance Counters for system metrics
3. **System Information**: Utilizes Windows Management Instrumentation (WMI) for hardware details
4. **Process Analysis**: Uses the .NET Process API to monitor running applications
5. **Data Visualization**: Implements custom drawing routines for real-time performance graphs

All data is processed through thread-safe counters and displayed in an intuitive interface.

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

## 📄 License

This project is licensed under the MIT License - see the [LICENSE](LICENSE) file for details.

## 📞 Contact

Mark Hazleton - [@YourTwitterHandle](https://twitter.com/YourTwitterHandle)

Project Link: [https://github.com/MarkHazleton/KeyPressCounter](https://github.com/MarkHazleton/KeyPressCounter)
