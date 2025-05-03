using System.Diagnostics;
using System.Management;
using System.Runtime.InteropServices;
using System.Text;
using System.Net.NetworkInformation;
using Microsoft.Win32;

namespace MWH.KeyPressCounter;

/// <summary>
/// Provides access to system performance metrics and hardware information.
/// </summary>
public class SystemPerformanceMonitor : IDisposable
{
    // Performance counters for real-time monitoring
    private readonly PerformanceCounter cpuCounter;
    private readonly PerformanceCounter ramCounter;
    private readonly PerformanceCounter uptimeCounter;
    private readonly PerformanceCounter diskReadCounter;
    private readonly PerformanceCounter diskWriteCounter;
    private readonly PerformanceCounter networkSendCounter;
    private readonly PerformanceCounter networkReceiveCounter;
    
    // System info caches
    private string cpuName = string.Empty;
    private string osVersion = string.Empty;
    private long totalMemoryBytes;
    private int processorCount;
    private string machineUptime = string.Empty;
    
    /// <summary>
    /// Initializes a new instance of the SystemPerformanceMonitor class.
    /// </summary>
    public SystemPerformanceMonitor()
    {
        try
        {
            // Initialize performance counters
            cpuCounter = new PerformanceCounter("Processor", "% Processor Time", "_Total");
            ramCounter = new PerformanceCounter("Memory", "Available MBytes");
            uptimeCounter = new PerformanceCounter("System", "System Up Time");
            diskReadCounter = new PerformanceCounter("PhysicalDisk", "Disk Read Bytes/sec", "_Total");
            diskWriteCounter = new PerformanceCounter("PhysicalDisk", "Disk Write Bytes/sec", "_Total");
            
            try
            {
                networkSendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", GetPrimaryNetworkInterface());
                networkReceiveCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", GetPrimaryNetworkInterface());
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error initializing network counters: {ex.Message}");
                // Fallback to first available network interface
                var networkInterfaces = new PerformanceCounterCategory("Network Interface").GetInstanceNames();
                if (networkInterfaces.Length > 0)
                {
                    networkSendCounter = new PerformanceCounter("Network Interface", "Bytes Sent/sec", networkInterfaces[0]);
                    networkReceiveCounter = new PerformanceCounter("Network Interface", "Bytes Received/sec", networkInterfaces[0]);
                }
                else
                {
                    // If no network interfaces found, create dummy counters
                    networkSendCounter = new PerformanceCounter();
                    networkReceiveCounter = new PerformanceCounter();
                }
            }
            
            // Pre-fetch some system information that doesn't change often
            InitializeSystemInfo();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing performance monitor: {ex.Message}");
            throw;
        }
    }
    
    /// <summary>
    /// Gets system CPU usage as a percentage.
    /// </summary>
    public float CpuUsagePercent
    {
        get
        {
            try
            {
                return cpuCounter.NextValue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting CPU usage: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets available RAM in MB.
    /// </summary>
    public float AvailableMemoryMB
    {
        get
        {
            try
            {
                return ramCounter.NextValue();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting available memory: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets the disk read speed in KB/s.
    /// </summary>
    public float DiskReadKBPerSec
    {
        get
        {
            try
            {
                return diskReadCounter.NextValue() / 1024;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting disk read speed: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets the disk write speed in KB/s.
    /// </summary>
    public float DiskWriteKBPerSec
    {
        get
        {
            try
            {
                return diskWriteCounter.NextValue() / 1024;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting disk write speed: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets the network upload speed in KB/s.
    /// </summary>
    public float NetworkUploadKBPerSec
    {
        get
        {
            try
            {
                return networkSendCounter.NextValue() / 1024;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting network upload speed: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets the network download speed in KB/s.
    /// </summary>
    public float NetworkDownloadKBPerSec
    {
        get
        {
            try
            {
                return networkReceiveCounter.NextValue() / 1024;
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting network download speed: {ex.Message}");
                return -1;
            }
        }
    }
    
    /// <summary>
    /// Gets the name of the CPU.
    /// </summary>
    public string CpuName => cpuName;
    
    /// <summary>
    /// Gets the operating system version and name.
    /// </summary>
    public string OsVersion => osVersion;
    
    /// <summary>
    /// Gets the total physical memory in GB.
    /// </summary>
    public double TotalMemoryGB => Math.Round(totalMemoryBytes / (1024.0 * 1024.0 * 1024.0), 2);
    
    /// <summary>
    /// Gets the number of processor cores.
    /// </summary>
    public int ProcessorCount => processorCount;
    
    /// <summary>
    /// Gets the current system uptime.
    /// </summary>
    public string SystemUptime 
    {
        get 
        {
            try 
            {
                TimeSpan uptime = TimeSpan.FromSeconds(uptimeCounter.NextValue());
                return $"{uptime.Days}d {uptime.Hours}h {uptime.Minutes}m {uptime.Seconds}s";
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error getting system uptime: {ex.Message}");
                return "Unknown";
            }
        }
    }
    
    /// <summary>
    /// Gets the memory usage percentage.
    /// </summary>
    public float MemoryUsagePercent
    {
        get
        {
            if (totalMemoryBytes <= 0) return 0;
            float availableMB = AvailableMemoryMB;
            if (availableMB < 0) return 0;
            
            float totalMB = totalMemoryBytes / (1024 * 1024);
            float usedMB = totalMB - availableMB;
            return (usedMB / totalMB) * 100;
        }
    }
    
    /// <summary>
    /// Returns a string with the current performance metrics.
    /// </summary>
    public string GetPerformanceSummary()
    {
        StringBuilder sb = new();
        sb.AppendLine("--- System Performance ---");
        sb.AppendLine($"CPU: {CpuUsagePercent:F1}%");
        sb.AppendLine($"RAM: {MemoryUsagePercent:F1}% ({(TotalMemoryGB - (AvailableMemoryMB / 1024)):F1}GB of {TotalMemoryGB:F1}GB)");
        sb.AppendLine($"Disk I/O: {DiskReadKBPerSec:F1} KB/s read, {DiskWriteKBPerSec:F1} KB/s write");
        sb.AppendLine($"Network: {NetworkDownloadKBPerSec:F1} KB/s down, {NetworkUploadKBPerSec:F1} KB/s up");
        sb.AppendLine($"Uptime: {SystemUptime}");
        return sb.ToString();
    }
    
    /// <summary>
    /// Returns a string with system hardware information.
    /// </summary>
    public string GetSystemInfo()
    {
        StringBuilder sb = new();
        sb.AppendLine("--- System Information ---");
        sb.AppendLine($"OS: {OsVersion}");
        sb.AppendLine($"CPU: {CpuName}");
        sb.AppendLine($"Cores: {ProcessorCount}");
        sb.AppendLine($"RAM: {TotalMemoryGB:F1} GB");
        
        // Add more system information like GPU, disk space, etc.
        try
        {
            string gpuInfo = GetGpuInfo();
            if (!string.IsNullOrEmpty(gpuInfo))
                sb.AppendLine($"GPU: {gpuInfo}");
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting GPU info: {ex.Message}");
        }
        
        try
        {
            DriveInfo[] drives = DriveInfo.GetDrives();
            sb.AppendLine("Disk Space:");
            foreach (DriveInfo drive in drives)
            {
                if (drive.IsReady)
                {
                    double totalGB = drive.TotalSize / (1024.0 * 1024.0 * 1024.0);
                    double freeGB = drive.AvailableFreeSpace / (1024.0 * 1024.0 * 1024.0);
                    sb.AppendLine($"  {drive.Name} {freeGB:F1} GB free of {totalGB:F1} GB");
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting drive info: {ex.Message}");
        }
        
        return sb.ToString();
    }
    
    /// <summary>
    /// Initializes system information that doesn't change often.
    /// </summary>
    private void InitializeSystemInfo()
    {
        try
        {
            // Get OS version
            osVersion = GetOperatingSystemInfo();
            
            // Get processor count
            processorCount = Environment.ProcessorCount;
            
            // Get CPU name
            cpuName = GetCpuName();
            
            // Get total memory
            totalMemoryBytes = GetTotalPhysicalMemory();
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error initializing system info: {ex.Message}");
        }
    }
    
    /// <summary>
    /// Gets the CPU name from the registry.
    /// </summary>
    private string GetCpuName()
    {
        try
        {
            using var key = Registry.LocalMachine.OpenSubKey(@"HARDWARE\DESCRIPTION\System\CentralProcessor\0");
            if (key != null)
            {
                return key.GetValue("ProcessorNameString")?.ToString() ?? "Unknown CPU";
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting CPU name: {ex.Message}");
        }
        
        return "Unknown CPU";
    }
    
    /// <summary>
    /// Gets the operating system version and name.
    /// </summary>
    private string GetOperatingSystemInfo()
    {
        try
        {
            var osInfo = Environment.OSVersion;
            string osName = "Windows";
            
            using var key = Registry.LocalMachine.OpenSubKey(@"SOFTWARE\Microsoft\Windows NT\CurrentVersion");
            if (key != null)
            {
                var productName = key.GetValue("ProductName")?.ToString();
                if (!string.IsNullOrEmpty(productName))
                    osName = productName;
            }
            
            return $"{osName} ({osInfo.Version})";
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting OS info: {ex.Message}");
            return "Windows (Unknown version)";
        }
    }
    
    /// <summary>
    /// Gets the total physical memory installed on the system.
    /// </summary>
    private long GetTotalPhysicalMemory()
    {
        try
        {
            // Use WMI to get total physical memory instead of Microsoft.VisualBasic.Devices.ComputerInfo
            using var searcher = new ManagementObjectSearcher("SELECT TotalPhysicalMemory FROM Win32_ComputerSystem");
            foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
            {
                if (mo["TotalPhysicalMemory"] != null)
                {
                    return Convert.ToInt64(mo["TotalPhysicalMemory"]);
                }
            }
            return 0;
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting total physical memory: {ex.Message}");
            return 0;
        }
    }
    
    /// <summary>
    /// Gets information about the GPU.
    /// </summary>
    private string GetGpuInfo()
    {
        try
        {
            using var searcher = new ManagementObjectSearcher("SELECT Name FROM Win32_VideoController");
            foreach (ManagementObject mo in searcher.Get().Cast<ManagementObject>())
            {
                var gpuName = mo["Name"]?.ToString();
                if (!string.IsNullOrEmpty(gpuName))
                    return gpuName;
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error querying GPU info: {ex.Message}");
        }
        
        return "Unknown GPU";
    }
    
    /// <summary>
    /// Gets the primary network interface name.
    /// </summary>
    private string GetPrimaryNetworkInterface()
    {
        try
        {
            NetworkInterface[] adapters = NetworkInterface.GetAllNetworkInterfaces();
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.OperationalStatus == OperationalStatus.Up &&
                    adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback &&
                    adapter.GetIPv4Statistics().BytesSent > 0)
                {
                    return adapter.Description;
                }
            }
            
            // Fallback to first non-loopback adapter
            foreach (NetworkInterface adapter in adapters)
            {
                if (adapter.NetworkInterfaceType != NetworkInterfaceType.Loopback)
                {
                    return adapter.Description;
                }
            }
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Error getting primary network interface: {ex.Message}");
        }
        
        // Get first available instance name
        var instanceNames = new PerformanceCounterCategory("Network Interface").GetInstanceNames();
        return instanceNames.Length > 0 ? instanceNames[0] : "_Total";
    }
    
    /// <summary>
    /// Disposes the performance counters.
    /// </summary>
    public void Dispose()
    {
        cpuCounter?.Dispose();
        ramCounter?.Dispose();
        uptimeCounter?.Dispose();
        diskReadCounter?.Dispose();
        diskWriteCounter?.Dispose();
        networkSendCounter?.Dispose();
        networkReceiveCounter?.Dispose();
    }
}