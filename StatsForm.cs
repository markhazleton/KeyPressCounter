using System.Drawing.Drawing2D;
using System.Diagnostics;
using System.Text;

namespace MWH.KeyPressCounter
{
    public partial class StatsForm : Form
    {
        private readonly Counter keyPressCounter;
        private readonly Counter mouseClickCounter;
        private readonly SystemPerformanceMonitor performanceMonitor;
        private readonly System.Windows.Forms.Timer refreshTimer = new();
        
        // For tracking historical data
        private readonly Queue<float> cpuHistory = new(60); // Last 60 seconds of data
        private readonly Queue<float> memoryHistory = new(60);
        private readonly Queue<float> diskReadHistory = new(60);
        private readonly Queue<float> diskWriteHistory = new(60);
        private readonly Queue<float> networkDownloadHistory = new(60);
        private readonly Queue<float> networkUploadHistory = new(60);
        
        public StatsForm(Counter keyPressCounter, Counter mouseClickCounter, SystemPerformanceMonitor performanceMonitor)
        {
            InitializeComponent();
            
            this.keyPressCounter = keyPressCounter;
            this.mouseClickCounter = mouseClickCounter;
            this.performanceMonitor = performanceMonitor;
            
            // Set up refresh timer to update stats
            refreshTimer.Interval = 1000; // 1 second
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();
            
            // Initialize the UI with current data
            UpdateUI();
        }
        
        private void RefreshTimer_Tick(object? sender, EventArgs e)
        {
            UpdateUI();
        }
        
        private void UpdateUI()
        {
            try
            {
                // Update keystroke and mouse click stats
                UpdateInputStats();
                
                // Update system performance stats
                UpdateSystemStats();
                
                // Update historical data for graphs
                UpdateHistoricalData();
                
                // Refresh graph panels
                perfGraphPanel.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating UI: {ex.Message}");
            }
        }
        
        private void UpdateInputStats()
        {
            // Update keystroke and mouse click stats
            keystrokeCountLabel.Text = $"Total Keystrokes: {keyPressCounter.TotalCount:N0}";
            keystrokesPerMinuteLabel.Text = $"Max Keystrokes/Min: {keyPressCounter.MaxPerInterval:N0}";
            
            mouseClickCountLabel.Text = $"Total Mouse Clicks: {mouseClickCounter.TotalCount:N0}";
            mouseClicksPerMinuteLabel.Text = $"Max Clicks/Min: {mouseClickCounter.MaxPerInterval:N0}";
            
            longestIdleLabel.Text = $"Longest Idle Period: {mouseClickCounter.LongestIntervalWithoutIncrement} minutes";
        }
        
        private void UpdateSystemStats()
        {
            // Update real-time system performance stats
            cpuUsageLabel.Text = $"CPU Usage: {performanceMonitor.CpuUsagePercent:F1}%";
            memoryUsageLabel.Text = $"Memory Usage: {performanceMonitor.MemoryUsagePercent:F1}% ({(performanceMonitor.TotalMemoryGB - (performanceMonitor.AvailableMemoryMB / 1024)):F1} GB of {performanceMonitor.TotalMemoryGB:F1} GB)";
            
            diskReadLabel.Text = $"Disk Read: {performanceMonitor.DiskReadKBPerSec:F1} KB/s";
            diskWriteLabel.Text = $"Disk Write: {performanceMonitor.DiskWriteKBPerSec:F1} KB/s";
            
            networkDownloadLabel.Text = $"Network Down: {performanceMonitor.NetworkDownloadKBPerSec:F1} KB/s";
            networkUploadLabel.Text = $"Network Up: {performanceMonitor.NetworkUploadKBPerSec:F1} KB/s";
            
            uptimeLabel.Text = $"System Uptime: {performanceMonitor.SystemUptime}";
        }
        
        private void UpdateHistoricalData()
        {
            // Add current values to history queues
            cpuHistory.Enqueue(performanceMonitor.CpuUsagePercent);
            memoryHistory.Enqueue(performanceMonitor.MemoryUsagePercent);
            diskReadHistory.Enqueue(performanceMonitor.DiskReadKBPerSec);
            diskWriteHistory.Enqueue(performanceMonitor.DiskWriteKBPerSec);
            networkDownloadHistory.Enqueue(performanceMonitor.NetworkDownloadKBPerSec);
            networkUploadHistory.Enqueue(performanceMonitor.NetworkUploadKBPerSec);
            
            // Keep queue at max size by removing oldest item if necessary
            if (cpuHistory.Count > 60) cpuHistory.Dequeue();
            if (memoryHistory.Count > 60) memoryHistory.Dequeue();
            if (diskReadHistory.Count > 60) diskReadHistory.Dequeue();
            if (diskWriteHistory.Count > 60) diskWriteHistory.Dequeue();
            if (networkDownloadHistory.Count > 60) networkDownloadHistory.Dequeue();
            if (networkUploadHistory.Count > 60) networkUploadHistory.Dequeue();
        }
        
        private void perfGraphPanel_Paint(object sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;
            
            int width = perfGraphPanel.Width;
            int height = perfGraphPanel.Height;
            
            // Draw background
            g.FillRectangle(new SolidBrush(Color.FromArgb(240, 240, 240)), 0, 0, width, height);
            
            // Draw CPU usage graph
            if (cpuHistory.Count > 1)
            {
                DrawGraph(g, cpuHistory.ToArray(), width, height, Color.FromArgb(220, 0, 0), 100);
            }
            
            // Draw memory usage graph
            if (memoryHistory.Count > 1)
            {
                DrawGraph(g, memoryHistory.ToArray(), width, height, Color.FromArgb(0, 180, 0), 100);
            }
            
            // Draw horizontal reference lines (25%, 50%, 75%, 100%)
            using Pen gridPen = new(Color.FromArgb(100, 100, 100, 100));
            for (int i = 1; i <= 4; i++)
            {
                float y = height - (height * i * 0.25f);
                g.DrawLine(gridPen, 0, y, width, y);
                g.DrawString($"{i * 25}%", Font, Brushes.Gray, 5, y - 15);
            }
            
            // Draw legend
            DrawLegend(g, width, height);
        }
        
        private void DrawGraph(Graphics g, float[] data, int width, int height, Color color, float maxValue)
        {
            // Skip if no data
            if (data.Length <= 1) return;
            
            using Pen graphPen = new(color, 2);
            
            float xScale = (float)width / (data.Length - 1);
            float yScale = height / maxValue;
            
            Point[] points = new Point[data.Length];
            
            for (int i = 0; i < data.Length; i++)
            {
                float x = i * xScale;
                // Clamp value to max to prevent graph from going off-scale
                float valueToPlot = Math.Min(data[i], maxValue);
                float y = height - (valueToPlot * yScale);
                
                points[i] = new Point((int)x, (int)y);
            }
            
            g.DrawLines(graphPen, points);
        }
        
        private void DrawLegend(Graphics g, int width, int height)
        {
            int legendY = 10;
            int legendX = width - 150;
            
            // CPU Legend
            g.FillRectangle(new SolidBrush(Color.FromArgb(220, 0, 0)), legendX, legendY, 15, 15);
            g.DrawString("CPU", Font, Brushes.Black, legendX + 20, legendY);
            
            // Memory Legend
            legendY += 20;
            g.FillRectangle(new SolidBrush(Color.FromArgb(0, 180, 0)), legendX, legendY, 15, 15);
            g.DrawString("Memory", Font, Brushes.Black, legendX + 20, legendY);
        }
        
        private void systemInfoButton_Click(object sender, EventArgs e)
        {
            // Display detailed system information
            MessageBox.Show(performanceMonitor.GetSystemInfo(), "System Information", MessageBoxButtons.OK, MessageBoxIcon.Information);
        }
        
        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
            }
            
            base.Dispose(disposing);
        }
        
        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();
            this.tabControl = new System.Windows.Forms.TabControl();
            this.inputStatsTab = new System.Windows.Forms.TabPage();
            this.longestIdleLabel = new System.Windows.Forms.Label();
            this.mouseClicksPerMinuteLabel = new System.Windows.Forms.Label();
            this.mouseClickCountLabel = new System.Windows.Forms.Label();
            this.keystrokesPerMinuteLabel = new System.Windows.Forms.Label();
            this.keystrokeCountLabel = new System.Windows.Forms.Label();
            this.inputStatsHeaderLabel = new System.Windows.Forms.Label();
            this.sysPerformanceTab = new System.Windows.Forms.TabPage();
            this.systemInfoButton = new System.Windows.Forms.Button();
            this.uptimeLabel = new System.Windows.Forms.Label();
            this.networkUploadLabel = new System.Windows.Forms.Label();
            this.networkDownloadLabel = new System.Windows.Forms.Label();
            this.diskWriteLabel = new System.Windows.Forms.Label();
            this.diskReadLabel = new System.Windows.Forms.Label();
            this.memoryUsageLabel = new System.Windows.Forms.Label();
            this.cpuUsageLabel = new System.Windows.Forms.Label();
            this.performanceHeaderLabel = new System.Windows.Forms.Label();
            this.perfGraphPanel = new System.Windows.Forms.Panel();
            this.tabControl.SuspendLayout();
            this.inputStatsTab.SuspendLayout();
            this.sysPerformanceTab.SuspendLayout();
            this.SuspendLayout();
            // 
            // tabControl
            // 
            this.tabControl.Controls.Add(this.inputStatsTab);
            this.tabControl.Controls.Add(this.sysPerformanceTab);
            this.tabControl.Dock = System.Windows.Forms.DockStyle.Fill;
            this.tabControl.Location = new System.Drawing.Point(0, 0);
            this.tabControl.Name = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size = new System.Drawing.Size(584, 461);
            this.tabControl.TabIndex = 0;
            // 
            // inputStatsTab
            // 
            this.inputStatsTab.Controls.Add(this.longestIdleLabel);
            this.inputStatsTab.Controls.Add(this.mouseClicksPerMinuteLabel);
            this.inputStatsTab.Controls.Add(this.mouseClickCountLabel);
            this.inputStatsTab.Controls.Add(this.keystrokesPerMinuteLabel);
            this.inputStatsTab.Controls.Add(this.keystrokeCountLabel);
            this.inputStatsTab.Controls.Add(this.inputStatsHeaderLabel);
            this.inputStatsTab.Location = new System.Drawing.Point(4, 24);
            this.inputStatsTab.Name = "inputStatsTab";
            this.inputStatsTab.Padding = new System.Windows.Forms.Padding(3);
            this.inputStatsTab.Size = new System.Drawing.Size(576, 433);
            this.inputStatsTab.TabIndex = 0;
            this.inputStatsTab.Text = "Input Statistics";
            this.inputStatsTab.UseVisualStyleBackColor = true;
            // 
            // longestIdleLabel
            // 
            this.longestIdleLabel.AutoSize = true;
            this.longestIdleLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.longestIdleLabel.Location = new System.Drawing.Point(20, 200);
            this.longestIdleLabel.Name = "longestIdleLabel";
            this.longestIdleLabel.Size = new System.Drawing.Size(178, 21);
            this.longestIdleLabel.TabIndex = 5;
            this.longestIdleLabel.Text = "Longest Idle Period: 0 min";
            // 
            // mouseClicksPerMinuteLabel
            // 
            this.mouseClicksPerMinuteLabel.AutoSize = true;
            this.mouseClicksPerMinuteLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mouseClicksPerMinuteLabel.Location = new System.Drawing.Point(20, 160);
            this.mouseClicksPerMinuteLabel.Name = "mouseClicksPerMinuteLabel";
            this.mouseClicksPerMinuteLabel.Size = new System.Drawing.Size(147, 21);
            this.mouseClicksPerMinuteLabel.TabIndex = 4;
            this.mouseClicksPerMinuteLabel.Text = "Max Clicks/Min: 0";
            // 
            // mouseClickCountLabel
            // 
            this.mouseClickCountLabel.AutoSize = true;
            this.mouseClickCountLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.mouseClickCountLabel.Location = new System.Drawing.Point(20, 130);
            this.mouseClickCountLabel.Name = "mouseClickCountLabel";
            this.mouseClickCountLabel.Size = new System.Drawing.Size(149, 21);
            this.mouseClickCountLabel.TabIndex = 3;
            this.mouseClickCountLabel.Text = "Total Mouse Clicks: 0";
            // 
            // keystrokesPerMinuteLabel
            // 
            this.keystrokesPerMinuteLabel.AutoSize = true;
            this.keystrokesPerMinuteLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.keystrokesPerMinuteLabel.Location = new System.Drawing.Point(20, 90);
            this.keystrokesPerMinuteLabel.Name = "keystrokesPerMinuteLabel";
            this.keystrokesPerMinuteLabel.Size = new System.Drawing.Size(186, 21);
            this.keystrokesPerMinuteLabel.TabIndex = 2;
            this.keystrokesPerMinuteLabel.Text = "Max Keystrokes/Min: 0";
            // 
            // keystrokeCountLabel
            // 
            this.keystrokeCountLabel.AutoSize = true;
            this.keystrokeCountLabel.Font = new System.Drawing.Font("Segoe UI", 12F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.keystrokeCountLabel.Location = new System.Drawing.Point(20, 60);
            this.keystrokeCountLabel.Name = "keystrokeCountLabel";
            this.keystrokeCountLabel.Size = new System.Drawing.Size(138, 21);
            this.keystrokeCountLabel.TabIndex = 1;
            this.keystrokeCountLabel.Text = "Total Keystrokes: 0";
            // 
            // inputStatsHeaderLabel
            // 
            this.inputStatsHeaderLabel.AutoSize = true;
            this.inputStatsHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.inputStatsHeaderLabel.Location = new System.Drawing.Point(20, 20);
            this.inputStatsHeaderLabel.Name = "inputStatsHeaderLabel";
            this.inputStatsHeaderLabel.Size = new System.Drawing.Size(286, 25);
            this.inputStatsHeaderLabel.TabIndex = 0;
            this.inputStatsHeaderLabel.Text = "Keyboard and Mouse Statistics";
            // 
            // sysPerformanceTab
            // 
            this.sysPerformanceTab.Controls.Add(this.perfGraphPanel);
            this.sysPerformanceTab.Controls.Add(this.systemInfoButton);
            this.sysPerformanceTab.Controls.Add(this.uptimeLabel);
            this.sysPerformanceTab.Controls.Add(this.networkUploadLabel);
            this.sysPerformanceTab.Controls.Add(this.networkDownloadLabel);
            this.sysPerformanceTab.Controls.Add(this.diskWriteLabel);
            this.sysPerformanceTab.Controls.Add(this.diskReadLabel);
            this.sysPerformanceTab.Controls.Add(this.memoryUsageLabel);
            this.sysPerformanceTab.Controls.Add(this.cpuUsageLabel);
            this.sysPerformanceTab.Controls.Add(this.performanceHeaderLabel);
            this.sysPerformanceTab.Location = new System.Drawing.Point(4, 24);
            this.sysPerformanceTab.Name = "sysPerformanceTab";
            this.sysPerformanceTab.Padding = new System.Windows.Forms.Padding(3);
            this.sysPerformanceTab.Size = new System.Drawing.Size(576, 433);
            this.sysPerformanceTab.TabIndex = 1;
            this.sysPerformanceTab.Text = "System Performance";
            this.sysPerformanceTab.UseVisualStyleBackColor = true;
            // 
            // systemInfoButton
            // 
            this.systemInfoButton.Location = new System.Drawing.Point(403, 20);
            this.systemInfoButton.Name = "systemInfoButton";
            this.systemInfoButton.Size = new System.Drawing.Size(153, 29);
            this.systemInfoButton.TabIndex = 9;
            this.systemInfoButton.Text = "System Information";
            this.systemInfoButton.UseVisualStyleBackColor = true;
            this.systemInfoButton.Click += new System.EventHandler(this.systemInfoButton_Click);
            // 
            // uptimeLabel
            // 
            this.uptimeLabel.AutoSize = true;
            this.uptimeLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.uptimeLabel.Location = new System.Drawing.Point(20, 210);
            this.uptimeLabel.Name = "uptimeLabel";
            this.uptimeLabel.Size = new System.Drawing.Size(100, 17);
            this.uptimeLabel.TabIndex = 7;
            this.uptimeLabel.Text = "System Uptime: ";
            // 
            // networkUploadLabel
            // 
            this.networkUploadLabel.AutoSize = true;
            this.networkUploadLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.networkUploadLabel.Location = new System.Drawing.Point(20, 180);
            this.networkUploadLabel.Name = "networkUploadLabel";
            this.networkUploadLabel.Size = new System.Drawing.Size(90, 17);
            this.networkUploadLabel.TabIndex = 6;
            this.networkUploadLabel.Text = "Network Up: 0";
            // 
            // networkDownloadLabel
            // 
            this.networkDownloadLabel.AutoSize = true;
            this.networkDownloadLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.networkDownloadLabel.Location = new System.Drawing.Point(20, 160);
            this.networkDownloadLabel.Name = "networkDownloadLabel";
            this.networkDownloadLabel.Size = new System.Drawing.Size(106, 17);
            this.networkDownloadLabel.TabIndex = 5;
            this.networkDownloadLabel.Text = "Network Down: 0";
            // 
            // diskWriteLabel
            // 
            this.diskWriteLabel.AutoSize = true;
            this.diskWriteLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.diskWriteLabel.Location = new System.Drawing.Point(20, 130);
            this.diskWriteLabel.Name = "diskWriteLabel";
            this.diskWriteLabel.Size = new System.Drawing.Size(75, 17);
            this.diskWriteLabel.TabIndex = 4;
            this.diskWriteLabel.Text = "Disk Write: ";
            // 
            // diskReadLabel
            // 
            this.diskReadLabel.AutoSize = true;
            this.diskReadLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.diskReadLabel.Location = new System.Drawing.Point(20, 110);
            this.diskReadLabel.Name = "diskReadLabel";
            this.diskReadLabel.Size = new System.Drawing.Size(71, 17);
            this.diskReadLabel.TabIndex = 3;
            this.diskReadLabel.Text = "Disk Read: ";
            // 
            // memoryUsageLabel
            // 
            this.memoryUsageLabel.AutoSize = true;
            this.memoryUsageLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.memoryUsageLabel.Location = new System.Drawing.Point(20, 80);
            this.memoryUsageLabel.Name = "memoryUsageLabel";
            this.memoryUsageLabel.Size = new System.Drawing.Size(105, 17);
            this.memoryUsageLabel.TabIndex = 2;
            this.memoryUsageLabel.Text = "Memory Usage: ";
            // 
            // cpuUsageLabel
            // 
            this.cpuUsageLabel.AutoSize = true;
            this.cpuUsageLabel.Font = new System.Drawing.Font("Segoe UI", 9.75F, System.Drawing.FontStyle.Regular, System.Drawing.GraphicsUnit.Point);
            this.cpuUsageLabel.Location = new System.Drawing.Point(20, 60);
            this.cpuUsageLabel.Name = "cpuUsageLabel";
            this.cpuUsageLabel.Size = new System.Drawing.Size(81, 17);
            this.cpuUsageLabel.TabIndex = 1;
            this.cpuUsageLabel.Text = "CPU Usage: ";
            // 
            // performanceHeaderLabel
            // 
            this.performanceHeaderLabel.AutoSize = true;
            this.performanceHeaderLabel.Font = new System.Drawing.Font("Segoe UI", 14.25F, System.Drawing.FontStyle.Bold, System.Drawing.GraphicsUnit.Point);
            this.performanceHeaderLabel.Location = new System.Drawing.Point(20, 20);
            this.performanceHeaderLabel.Name = "performanceHeaderLabel";
            this.performanceHeaderLabel.Size = new System.Drawing.Size(197, 25);
            this.performanceHeaderLabel.TabIndex = 0;
            this.performanceHeaderLabel.Text = "System Performance";
            // 
            // perfGraphPanel
            // 
            this.perfGraphPanel.Anchor = ((System.Windows.Forms.AnchorStyles)((((System.Windows.Forms.AnchorStyles.Top | System.Windows.Forms.AnchorStyles.Bottom) 
            | System.Windows.Forms.AnchorStyles.Left) 
            | System.Windows.Forms.AnchorStyles.Right)));
            this.perfGraphPanel.BorderStyle = System.Windows.Forms.BorderStyle.FixedSingle;
            this.perfGraphPanel.Location = new System.Drawing.Point(20, 240);
            this.perfGraphPanel.Name = "perfGraphPanel";
            this.perfGraphPanel.Size = new System.Drawing.Size(536, 175);
            this.perfGraphPanel.TabIndex = 10;
            this.perfGraphPanel.Paint += new System.Windows.Forms.PaintEventHandler(this.perfGraphPanel_Paint);
            // 
            // StatsForm
            // 
            this.AutoScaleDimensions = new System.Drawing.SizeF(7F, 15F);
            this.AutoScaleMode = System.Windows.Forms.AutoScaleMode.Font;
            this.ClientSize = new System.Drawing.Size(584, 461);
            this.Controls.Add(this.tabControl);
            this.MinimumSize = new System.Drawing.Size(600, 500);
            this.Name = "StatsForm";
            this.StartPosition = System.Windows.Forms.FormStartPosition.CenterScreen;
            this.Text = "KeyPressCounter Statistics";
            this.tabControl.ResumeLayout(false);
            this.inputStatsTab.ResumeLayout(false);
            this.inputStatsTab.PerformLayout();
            this.sysPerformanceTab.ResumeLayout(false);
            this.sysPerformanceTab.PerformLayout();
            this.ResumeLayout(false);

        }

        #endregion

        private TabControl tabControl;
        private TabPage inputStatsTab;
        private TabPage sysPerformanceTab;
        private Label inputStatsHeaderLabel;
        private Label keystrokesPerMinuteLabel;
        private Label keystrokeCountLabel;
        private Label mouseClicksPerMinuteLabel;
        private Label mouseClickCountLabel;
        private Label longestIdleLabel;
        private Label performanceHeaderLabel;
        private Label cpuUsageLabel;
        private Label memoryUsageLabel;
        private Label diskReadLabel;
        private Label diskWriteLabel;
        private Label networkDownloadLabel;
        private Label networkUploadLabel;
        private Label uptimeLabel;
        private Button systemInfoButton;
        private Panel perfGraphPanel;
    }
}