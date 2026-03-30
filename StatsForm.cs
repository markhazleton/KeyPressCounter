using System.Drawing.Drawing2D;
using System.Diagnostics;

namespace MWH.KeyPressCounter
{
    public partial class StatsForm : Form
    {
        private readonly Counter keyPressCounter;
        private readonly Counter mouseClickCounter;
        private readonly SystemPerformanceMonitor performanceMonitor;
        private readonly System.Windows.Forms.Timer refreshTimer = new();

        // Historical data queues (60 seconds)
        private readonly Queue<float> cpuHistory = new(60);
        private readonly Queue<float> memoryHistory = new(60);
        private readonly Queue<float> diskReadHistory = new(60);
        private readonly Queue<float> diskWriteHistory = new(60);
        private readonly Queue<float> networkDownloadHistory = new(60);
        private readonly Queue<float> networkUploadHistory = new(60);
        private readonly ToolTip toolTip = new();

        // Color palette
        private static readonly Color PrimaryColor     = Color.FromArgb(21, 101, 192);
        private static readonly Color KeyboardAccent   = Color.FromArgb(21, 101, 192);
        private static readonly Color MouseAccent      = Color.FromArgb(46, 125, 50);
        private static readonly Color CardBackground   = Color.White;
        private static readonly Color FormBackground   = Color.FromArgb(245, 247, 250);
        private static readonly Color HeaderText       = Color.White;
        private static readonly Color SubtleText       = Color.FromArgb(117, 117, 117);

        public StatsForm(Counter keyPressCounter, Counter mouseClickCounter, SystemPerformanceMonitor performanceMonitor)
        {
            InitializeComponent();

            this.keyPressCounter = keyPressCounter;
            this.mouseClickCounter = mouseClickCounter;
            this.performanceMonitor = performanceMonitor;

            // Fix: set form icon to match the system tray icon
            try
            {
                string iconPath = Path.Combine(AppContext.BaseDirectory, "favicon.ico");
                if (File.Exists(iconPath))
                    this.Icon = new Icon(iconPath);
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Could not load form icon: {ex.Message}");
            }

            refreshTimer.Interval = 1000;
            refreshTimer.Tick += RefreshTimer_Tick;
            refreshTimer.Start();

            UpdateUI();
        }

        private void RefreshTimer_Tick(object? sender, EventArgs e) => UpdateUI();

        private void UpdateUI()
        {
            try
            {
                UpdateInputStats();
                UpdateSystemStats();
                UpdateHistoricalData();
                perfGraphPanel.Invalidate();
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error updating UI: {ex.Message}");
            }
        }

        private void UpdateInputStats()
        {
            keystrokeCountLabel.Text      = $"{keyPressCounter.TotalCount:N0}";
            keystrokesPerMinuteLabel.Text = $"{keyPressCounter.MaxPerInterval:N0}";
            mouseClickCountLabel.Text     = $"{mouseClickCounter.TotalCount:N0}";
            mouseClicksPerMinuteLabel.Text= $"{mouseClickCounter.MaxPerInterval:N0}";
            longestIdleLabel.Text         = $"{mouseClickCounter.LongestIntervalWithoutIncrement} min";
        }

        private void UpdateSystemStats()
        {
            double usedGB = performanceMonitor.TotalMemoryGB - (performanceMonitor.AvailableMemoryMB / 1024.0);
            cpuUsageLabel.Text           = $"{performanceMonitor.CpuUsagePercent:F1}%";
            memoryUsageLabel.Text        = $"{performanceMonitor.MemoryUsagePercent:F1}%  ({usedGB:F1} GB / {performanceMonitor.TotalMemoryGB:F1} GB)";
            diskReadLabel.Text           = $"{performanceMonitor.DiskReadKBPerSec:F1} KB/s";
            diskWriteLabel.Text          = $"{performanceMonitor.DiskWriteKBPerSec:F1} KB/s";
            networkDownloadLabel.Text    = $"{performanceMonitor.NetworkDownloadKBPerSec:F1} KB/s";
            networkUploadLabel.Text      = $"{performanceMonitor.NetworkUploadKBPerSec:F1} KB/s";
            uptimeLabel.Text             = $"{performanceMonitor.SystemUptime}";
        }

        private void UpdateHistoricalData()
        {
            cpuHistory.Enqueue(performanceMonitor.CpuUsagePercent);
            memoryHistory.Enqueue(performanceMonitor.MemoryUsagePercent);
            diskReadHistory.Enqueue(performanceMonitor.DiskReadKBPerSec);
            diskWriteHistory.Enqueue(performanceMonitor.DiskWriteKBPerSec);
            networkDownloadHistory.Enqueue(performanceMonitor.NetworkDownloadKBPerSec);
            networkUploadHistory.Enqueue(performanceMonitor.NetworkUploadKBPerSec);

            if (cpuHistory.Count > 60)             cpuHistory.Dequeue();
            if (memoryHistory.Count > 60)          memoryHistory.Dequeue();
            if (diskReadHistory.Count > 60)        diskReadHistory.Dequeue();
            if (diskWriteHistory.Count > 60)       diskWriteHistory.Dequeue();
            if (networkDownloadHistory.Count > 60) networkDownloadHistory.Dequeue();
            if (networkUploadHistory.Count > 60)   networkUploadHistory.Dequeue();
        }

        private void perfGraphPanel_Paint(object? sender, PaintEventArgs e)
        {
            Graphics g = e.Graphics;
            g.SmoothingMode = SmoothingMode.AntiAlias;

            int width  = perfGraphPanel.Width;
            int height = perfGraphPanel.Height;

            // Respect high contrast mode to keep the graph readable.
            Color graphBackground = SystemInformation.HighContrast
                ? SystemColors.Window
                : Color.FromArgb(18, 28, 45);
            using (var bgBrush = new SolidBrush(graphBackground))
                g.FillRectangle(bgBrush, 0, 0, width, height);

            // Grid lines at 25%, 50%, 75%, 100%
            using var gridPen  = new Pen(Color.FromArgb(55, 255, 255, 255));
            using var gridFont = new Font("Segoe UI", 7.5f);
            for (int i = 1; i <= 4; i++)
            {
                float y = height - (height * i * 0.25f);
                g.DrawLine(gridPen, 0, y, width, y);
                g.DrawString($"{i * 25}%", gridFont, Brushes.DimGray, 4, y - 14);
            }

            if (cpuHistory.Count > 1)
                DrawGraph(g, cpuHistory.ToArray(), width, height, Color.FromArgb(255, 82, 82), 100);
            if (memoryHistory.Count > 1)
                DrawGraph(g, memoryHistory.ToArray(), width, height, Color.FromArgb(105, 240, 174), 100);

            DrawLegend(g, width);
        }

        private static void DrawGraph(Graphics g, float[] data, int width, int height, Color color, float maxValue)
        {
            if (data.Length <= 1) return;
            using Pen graphPen = new(color, 2);
            float xScale = (float)width / (data.Length - 1);
            float yScale  = height / maxValue;
            Point[] points = new Point[data.Length];
            for (int i = 0; i < data.Length; i++)
            {
                float x = i * xScale;
                float y = height - (Math.Min(data[i], maxValue) * yScale);
                points[i] = new Point((int)x, (int)y);
            }
            g.DrawLines(graphPen, points);
        }

        private static void DrawLegend(Graphics g, int width)
        {
            using var legendFont = new Font("Segoe UI", 8.5f);
            int legendX = width - 110;
            int legendY = 8;

            using (var b = new SolidBrush(Color.FromArgb(255, 82, 82)))
                g.FillRectangle(b, legendX, legendY, 12, 12);
            g.DrawString("CPU", legendFont, Brushes.White, legendX + 16, legendY);

            legendY += 19;
            using (var b = new SolidBrush(Color.FromArgb(105, 240, 174)))
                g.FillRectangle(b, legendX, legendY, 12, 12);
            g.DrawString("Memory", legendFont, Brushes.White, legendX + 16, legendY);
        }

        private void systemInfoButton_Click(object? sender, EventArgs e)
        {
            MessageBox.Show(performanceMonitor.GetSystemInfo(), "System Information",
                MessageBoxButtons.OK, MessageBoxIcon.Information);
        }

        private void githubLinkLabel_LinkClicked(object? sender, LinkLabelLinkClickedEventArgs e)
        {
            try
            {
                Process.Start(new ProcessStartInfo
                {
                    FileName = "https://github.com/MarkHazleton/KeyPressCounter",
                    UseShellExecute = true
                });
            }
            catch (Exception ex)
            {
                Debug.WriteLine($"Error opening GitHub link: {ex.Message}");
            }
        }

        protected override void Dispose(bool disposing)
        {
            if (disposing)
            {
                refreshTimer.Stop();
                refreshTimer.Dispose();
                toolTip.Dispose();
            }
            base.Dispose(disposing);
        }

        #region Windows Form Designer generated code

        private System.ComponentModel.IContainer components = null!;

        private void InitializeComponent()
        {
            this.components = new System.ComponentModel.Container();

            // ── Tab infrastructure ─────────────────────────────────────────
            this.tabControl      = new TabControl();
            this.inputStatsTab   = new TabPage();
            this.sysPerformanceTab = new TabPage();
            this.aboutTab        = new TabPage();

            // ── Input Statistics Tab ───────────────────────────────────────
            this.inputHeaderPanel      = new Panel();
            this.inputStatsHeaderLabel = new Label();
            this.keyboardGroupBox      = new GroupBox();
            this.mouseGroupBox         = new GroupBox();

            // dynamic labels (updated by UpdateInputStats)
            this.keystrokeCountLabel       = new Label();
            this.keystrokesPerMinuteLabel  = new Label();
            this.mouseClickCountLabel      = new Label();
            this.mouseClicksPerMinuteLabel = new Label();
            this.longestIdleLabel          = new Label();

            // ── System Performance Tab ─────────────────────────────────────
            this.perfHeaderPanel       = new Panel();
            this.performanceHeaderLabel = new Label();
            this.cpuGroupBox           = new GroupBox();
            this.diskNetGroupBox       = new GroupBox();
            this.cpuUsageLabel         = new Label();
            this.memoryUsageLabel      = new Label();
            this.diskReadLabel         = new Label();
            this.diskWriteLabel        = new Label();
            this.networkDownloadLabel  = new Label();
            this.networkUploadLabel    = new Label();
            this.uptimeLabel           = new Label();
            this.systemInfoButton      = new Button();
            this.perfGraphPanel        = new Panel();

            // ── About Tab ─────────────────────────────────────────────────
            this.aboutHeaderPanel  = new Panel();
            this.aboutTitleLabel   = new Label();
            this.aboutVersionLabel = new Label();
            this.aboutDescLabel    = new Label();
            this.githubLinkLabel   = new LinkLabel();
            this.aboutTechLabel    = new Label();
            this.aboutLicenseLabel = new Label();
            this.aboutPrivacyLabel = new Label();

            this.tabControl.SuspendLayout();
            this.SuspendLayout();

            // ══════════════════════════════════════════════════════════════
            // tabControl
            // ══════════════════════════════════════════════════════════════
            this.tabControl.Controls.Add(this.inputStatsTab);
            this.tabControl.Controls.Add(this.sysPerformanceTab);
            this.tabControl.Controls.Add(this.aboutTab);
            this.tabControl.Dock          = DockStyle.Fill;
            this.tabControl.Font          = new Font("Segoe UI", 9.5f);
            this.tabControl.Location      = new Point(0, 0);
            this.tabControl.Name          = "tabControl";
            this.tabControl.SelectedIndex = 0;
            this.tabControl.Size          = new Size(640, 530);
            this.tabControl.TabIndex      = 0;
            this.tabControl.AccessibleName = "Statistics Tabs";
            this.tabControl.AccessibleDescription = "Switch between input statistics, system performance, and about information.";

            // ══════════════════════════════════════════════════════════════
            // INPUT STATISTICS TAB
            // ══════════════════════════════════════════════════════════════
            this.inputStatsTab.BackColor = FormBackground;
            this.inputStatsTab.Name      = "inputStatsTab";
            this.inputStatsTab.Padding   = new Padding(0);
            this.inputStatsTab.TabIndex  = 0;
            this.inputStatsTab.Text      = "&Input Statistics";

            // Header panel
            this.inputHeaderPanel.Dock      = DockStyle.Top;
            this.inputHeaderPanel.Height    = 52;
            this.inputHeaderPanel.BackColor = PrimaryColor;

            this.inputStatsHeaderLabel.Text      = "  Keyboard & Mouse Activity";
            this.inputStatsHeaderLabel.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            this.inputStatsHeaderLabel.ForeColor = HeaderText;
            this.inputStatsHeaderLabel.Dock      = DockStyle.Fill;
            this.inputStatsHeaderLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.inputHeaderPanel.Controls.Add(this.inputStatsHeaderLabel);
            this.inputStatsTab.Controls.Add(this.inputHeaderPanel);

            // ── Keyboard GroupBox ────────────────────────────────────────
            this.keyboardGroupBox.Text      = "Keyboard";
            this.keyboardGroupBox.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.keyboardGroupBox.ForeColor = KeyboardAccent;
            this.keyboardGroupBox.BackColor = CardBackground;
            this.keyboardGroupBox.Location  = new Point(16, 68);
            this.keyboardGroupBox.Size      = new Size(290, 160);

            // Big total number
            this.keystrokeCountLabel.Font      = new Font("Segoe UI", 30f, FontStyle.Bold);
            this.keystrokeCountLabel.ForeColor = KeyboardAccent;
            this.keystrokeCountLabel.Text      = "0";
            this.keystrokeCountLabel.AutoSize  = false;
            this.keystrokeCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.keystrokeCountLabel.Location  = new Point(8, 22);
            this.keystrokeCountLabel.Size      = new Size(274, 68);
            this.keyboardGroupBox.Controls.Add(this.keystrokeCountLabel);

            // Static "Total Keystrokes" caption
            var kbTotalCap = MakeCaption("Total Keystrokes", new Point(8, 92));
            this.keyboardGroupBox.Controls.Add(kbTotalCap);

            // Peak rate row
            var kbPeakCap = MakeCaption("Peak rate / min", new Point(8, 118));
            this.keyboardGroupBox.Controls.Add(kbPeakCap);

            this.keystrokesPerMinuteLabel.Font      = new Font("Segoe UI", 12f, FontStyle.Bold);
            this.keystrokesPerMinuteLabel.ForeColor = KeyboardAccent;
            this.keystrokesPerMinuteLabel.Text      = "0";
            this.keystrokesPerMinuteLabel.AutoSize  = false;
            this.keystrokesPerMinuteLabel.TextAlign = ContentAlignment.MiddleRight;
            this.keystrokesPerMinuteLabel.Location  = new Point(8, 113);
            this.keystrokesPerMinuteLabel.Size      = new Size(274, 28);
            this.keyboardGroupBox.Controls.Add(this.keystrokesPerMinuteLabel);

            this.inputStatsTab.Controls.Add(this.keyboardGroupBox);

            // ── Mouse GroupBox ───────────────────────────────────────────
            this.mouseGroupBox.Text      = "Mouse";
            this.mouseGroupBox.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.mouseGroupBox.ForeColor = MouseAccent;
            this.mouseGroupBox.BackColor = CardBackground;
            this.mouseGroupBox.Location  = new Point(322, 68);
            this.mouseGroupBox.Size      = new Size(290, 200);

            // Big total number
            this.mouseClickCountLabel.Font      = new Font("Segoe UI", 30f, FontStyle.Bold);
            this.mouseClickCountLabel.ForeColor = MouseAccent;
            this.mouseClickCountLabel.Text      = "0";
            this.mouseClickCountLabel.AutoSize  = false;
            this.mouseClickCountLabel.TextAlign = ContentAlignment.MiddleCenter;
            this.mouseClickCountLabel.Location  = new Point(8, 22);
            this.mouseClickCountLabel.Size      = new Size(274, 68);
            this.mouseGroupBox.Controls.Add(this.mouseClickCountLabel);

            var msTotalCap = MakeCaption("Total Clicks", new Point(8, 92));
            this.mouseGroupBox.Controls.Add(msTotalCap);

            var msPeakCap = MakeCaption("Peak rate / min", new Point(8, 118));
            this.mouseGroupBox.Controls.Add(msPeakCap);

            this.mouseClicksPerMinuteLabel.Font      = new Font("Segoe UI", 12f, FontStyle.Bold);
            this.mouseClicksPerMinuteLabel.ForeColor = MouseAccent;
            this.mouseClicksPerMinuteLabel.Text      = "0";
            this.mouseClicksPerMinuteLabel.AutoSize  = false;
            this.mouseClicksPerMinuteLabel.TextAlign = ContentAlignment.MiddleRight;
            this.mouseClicksPerMinuteLabel.Location  = new Point(8, 113);
            this.mouseClicksPerMinuteLabel.Size      = new Size(274, 28);
            this.mouseGroupBox.Controls.Add(this.mouseClicksPerMinuteLabel);

            var msIdleCap = MakeCaption("Longest idle period", new Point(8, 153));
            this.mouseGroupBox.Controls.Add(msIdleCap);

            this.longestIdleLabel.Font      = new Font("Segoe UI", 12f, FontStyle.Bold);
            this.longestIdleLabel.ForeColor = Color.FromArgb(183, 28, 28);
            this.longestIdleLabel.Text      = "0 min";
            this.longestIdleLabel.AutoSize  = false;
            this.longestIdleLabel.TextAlign = ContentAlignment.MiddleRight;
            this.longestIdleLabel.Location  = new Point(8, 148);
            this.longestIdleLabel.Size      = new Size(274, 28);
            this.mouseGroupBox.Controls.Add(this.longestIdleLabel);

            this.inputStatsTab.Controls.Add(this.mouseGroupBox);

            // ── Session info hint ────────────────────────────────────────
            var sessionNote = new Label
            {
                Text      = "Counters reset daily at midnight. Right-click tray icon to reset manually.",
                Font      = new Font("Segoe UI", 8.5f, FontStyle.Italic),
                ForeColor = SubtleText,
                Location  = new Point(16, 285),
                AutoSize  = true
            };
            this.inputStatsTab.Controls.Add(sessionNote);

            // ══════════════════════════════════════════════════════════════
            // SYSTEM PERFORMANCE TAB
            // ══════════════════════════════════════════════════════════════
            this.sysPerformanceTab.BackColor = FormBackground;
            this.sysPerformanceTab.Name      = "sysPerformanceTab";
            this.sysPerformanceTab.Padding   = new Padding(0);
            this.sysPerformanceTab.TabIndex  = 1;
            this.sysPerformanceTab.Text      = "&System Performance";

            this.perfHeaderPanel.Dock      = DockStyle.Top;
            this.perfHeaderPanel.Height    = 52;
            this.perfHeaderPanel.BackColor = Color.FromArgb(38, 50, 56);

            this.performanceHeaderLabel.Text      = "  Real-Time System Performance";
            this.performanceHeaderLabel.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            this.performanceHeaderLabel.ForeColor = HeaderText;
            this.performanceHeaderLabel.Dock      = DockStyle.Fill;
            this.performanceHeaderLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.perfHeaderPanel.Controls.Add(this.performanceHeaderLabel);
            this.sysPerformanceTab.Controls.Add(this.perfHeaderPanel);

            // ── CPU / Memory GroupBox ────────────────────────────────────
            this.cpuGroupBox.Text      = "CPU & Memory";
            this.cpuGroupBox.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.cpuGroupBox.ForeColor = Color.FromArgb(38, 50, 56);
            this.cpuGroupBox.BackColor = CardBackground;
            this.cpuGroupBox.Location  = new Point(16, 68);
            this.cpuGroupBox.Size      = new Size(590, 100);
            this.sysPerformanceTab.Controls.Add(this.cpuGroupBox);

            this.cpuUsageLabel.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            this.cpuUsageLabel.ForeColor = Color.FromArgb(183, 28, 28);
            this.cpuUsageLabel.Text      = "CPU: —";
            this.cpuUsageLabel.AutoSize  = true;
            this.cpuUsageLabel.Location  = new Point(12, 26);
            this.cpuGroupBox.Controls.Add(this.cpuUsageLabel);

            var cpuCap = MakeCaption("CPU Usage", new Point(12, 52));
            this.cpuGroupBox.Controls.Add(cpuCap);

            this.memoryUsageLabel.Font      = new Font("Segoe UI", 11f, FontStyle.Bold);
            this.memoryUsageLabel.ForeColor = Color.FromArgb(27, 94, 32);
            this.memoryUsageLabel.Text      = "Memory: —";
            this.memoryUsageLabel.AutoSize  = true;
            this.memoryUsageLabel.Location  = new Point(200, 26);
            this.cpuGroupBox.Controls.Add(this.memoryUsageLabel);

            var memCap = MakeCaption("Memory Usage", new Point(200, 52));
            this.cpuGroupBox.Controls.Add(memCap);

            // ── Disk / Network GroupBox ──────────────────────────────────
            this.diskNetGroupBox.Text      = "Disk & Network";
            this.diskNetGroupBox.Font      = new Font("Segoe UI", 10f, FontStyle.Bold);
            this.diskNetGroupBox.ForeColor = Color.FromArgb(38, 50, 56);
            this.diskNetGroupBox.BackColor = CardBackground;
            this.diskNetGroupBox.Location  = new Point(16, 178);
            this.diskNetGroupBox.Size      = new Size(590, 100);
            this.sysPerformanceTab.Controls.Add(this.diskNetGroupBox);

            this.diskReadLabel.Font      = new Font("Segoe UI", 10f);
            this.diskReadLabel.ForeColor = Color.DimGray;
            this.diskReadLabel.Text      = "— KB/s";
            this.diskReadLabel.AutoSize  = true;
            this.diskReadLabel.Location  = new Point(12, 26);
            this.diskNetGroupBox.Controls.Add(this.diskReadLabel);
            this.diskNetGroupBox.Controls.Add(MakeCaption("Disk Read", new Point(12, 52)));

            this.diskWriteLabel.Font      = new Font("Segoe UI", 10f);
            this.diskWriteLabel.ForeColor = Color.DimGray;
            this.diskWriteLabel.Text      = "— KB/s";
            this.diskWriteLabel.AutoSize  = true;
            this.diskWriteLabel.Location  = new Point(145, 26);
            this.diskNetGroupBox.Controls.Add(this.diskWriteLabel);
            this.diskNetGroupBox.Controls.Add(MakeCaption("Disk Write", new Point(145, 52)));

            this.networkDownloadLabel.Font      = new Font("Segoe UI", 10f);
            this.networkDownloadLabel.ForeColor = Color.DimGray;
            this.networkDownloadLabel.Text      = "— KB/s";
            this.networkDownloadLabel.AutoSize  = true;
            this.networkDownloadLabel.Location  = new Point(285, 26);
            this.diskNetGroupBox.Controls.Add(this.networkDownloadLabel);
            this.diskNetGroupBox.Controls.Add(MakeCaption("Net Download", new Point(285, 52)));

            this.networkUploadLabel.Font      = new Font("Segoe UI", 10f);
            this.networkUploadLabel.ForeColor = Color.DimGray;
            this.networkUploadLabel.Text      = "— KB/s";
            this.networkUploadLabel.AutoSize  = true;
            this.networkUploadLabel.Location  = new Point(420, 26);
            this.diskNetGroupBox.Controls.Add(this.networkUploadLabel);
            this.diskNetGroupBox.Controls.Add(MakeCaption("Net Upload", new Point(420, 52)));

            // Uptime + System Info button row
            this.uptimeLabel.Font      = new Font("Segoe UI", 9.5f);
            this.uptimeLabel.ForeColor = SubtleText;
            this.uptimeLabel.Text      = "System Uptime: —";
            this.uptimeLabel.AutoSize  = true;
            this.uptimeLabel.Location  = new Point(16, 285);
            this.sysPerformanceTab.Controls.Add(this.uptimeLabel);

            this.systemInfoButton.Text      = "&System Information...";
            this.systemInfoButton.Font      = new Font("Segoe UI", 9.5f);
            this.systemInfoButton.Location  = new Point(430, 280);
            this.systemInfoButton.Size      = new Size(176, 30);
            this.systemInfoButton.FlatStyle = FlatStyle.System;
            this.systemInfoButton.TabIndex  = 9;
            this.systemInfoButton.AccessibleName = "System Information";
            this.systemInfoButton.AccessibleDescription = "Opens a dialog with detailed system hardware information.";
            this.systemInfoButton.Click    += systemInfoButton_Click;
            this.sysPerformanceTab.Controls.Add(this.systemInfoButton);

            // Graph panel
            this.perfGraphPanel.Anchor      = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right;
            this.perfGraphPanel.BorderStyle = BorderStyle.FixedSingle;
            this.perfGraphPanel.BackColor   = Color.FromArgb(18, 28, 45);
            this.perfGraphPanel.Location    = new Point(16, 320);
            this.perfGraphPanel.Size        = new Size(590, 155);
            this.perfGraphPanel.TabIndex    = 10;
            this.perfGraphPanel.AccessibleName = "CPU and Memory History Graph";
            this.perfGraphPanel.AccessibleDescription = "Shows recent CPU and memory usage history for the last 60 seconds.";
            this.perfGraphPanel.Paint      += perfGraphPanel_Paint;
            this.sysPerformanceTab.Controls.Add(this.perfGraphPanel);

            // ══════════════════════════════════════════════════════════════
            // ABOUT TAB
            // ══════════════════════════════════════════════════════════════
            this.aboutTab.BackColor = FormBackground;
            this.aboutTab.Name      = "aboutTab";
            this.aboutTab.Padding   = new Padding(0);
            this.aboutTab.TabIndex  = 2;
            this.aboutTab.Text      = "&About";

            this.aboutHeaderPanel.Dock      = DockStyle.Top;
            this.aboutHeaderPanel.Height    = 52;
            this.aboutHeaderPanel.BackColor = Color.FromArgb(49, 27, 146);

            this.aboutTitleLabel.Text      = "  KeyPressCounter";
            this.aboutTitleLabel.Font      = new Font("Segoe UI", 14f, FontStyle.Bold);
            this.aboutTitleLabel.ForeColor = HeaderText;
            this.aboutTitleLabel.Dock      = DockStyle.Fill;
            this.aboutTitleLabel.TextAlign = ContentAlignment.MiddleLeft;
            this.aboutHeaderPanel.Controls.Add(this.aboutTitleLabel);
            this.aboutTab.Controls.Add(this.aboutHeaderPanel);

            // Content panel (scrollable)
            var aboutScroll = new Panel
            {
                AutoScroll = true,
                Location   = new Point(0, 52),
                Anchor     = AnchorStyles.Top | AnchorStyles.Bottom | AnchorStyles.Left | AnchorStyles.Right,
                Size       = new Size(628, 450),
                BackColor  = FormBackground
            };
            this.aboutTab.Controls.Add(aboutScroll);

            int y = 18;

            // App name + version card
            var infoCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(16, y),
                Size      = new Size(590, 115)
            };
            aboutScroll.Controls.Add(infoCard);

            this.aboutVersionLabel.Text      = "v1.0.0  —  Windows System Tray Utility";
            this.aboutVersionLabel.Font      = new Font("Segoe UI", 10f);
            this.aboutVersionLabel.ForeColor = SubtleText;
            this.aboutVersionLabel.AutoSize  = true;
            this.aboutVersionLabel.Location  = new Point(14, 12);
            infoCard.Controls.Add(this.aboutVersionLabel);

            this.aboutDescLabel.Text      = "KeyPressCounter monitors keyboard and mouse input counts, tracks system performance " +
                                            "metrics in real time, and logs daily activity summaries — all from the Windows system tray. " +
                                            "Keystrokes are counted, never recorded.";
            this.aboutDescLabel.Font      = new Font("Segoe UI", 9.5f);
            this.aboutDescLabel.ForeColor = Color.FromArgb(66, 66, 66);
            this.aboutDescLabel.AutoSize  = false;
            this.aboutDescLabel.Size      = new Size(562, 72);
            this.aboutDescLabel.Location  = new Point(14, 35);
            infoCard.Controls.Add(this.aboutDescLabel);

            y += 127;

            // Author card
            var authorCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(16, y),
                Size      = new Size(590, 75)
            };
            aboutScroll.Controls.Add(authorCard);
            authorCard.Controls.Add(MakeSectionHeader("Author", new Point(14, 10)));

            var authorNameLabel = new Label
            {
                Text      = "Mark Hazleton",
                Font      = new Font("Segoe UI", 10.5f, FontStyle.Bold),
                ForeColor = Color.FromArgb(33, 33, 33),
                AutoSize  = true,
                Location  = new Point(14, 32)
            };
            authorCard.Controls.Add(authorNameLabel);

            this.githubLinkLabel.Text      = "github.com/MarkHazleton/KeyPressCounter";
            this.githubLinkLabel.Font      = new Font("Segoe UI", 10f);
            this.githubLinkLabel.LinkColor = PrimaryColor;
            this.githubLinkLabel.AutoSize  = true;
            this.githubLinkLabel.Location  = new Point(160, 34);
            this.githubLinkLabel.Links.Clear();
            this.githubLinkLabel.Links.Add(0, this.githubLinkLabel.Text.Length, "https://github.com/MarkHazleton/KeyPressCounter");
            this.githubLinkLabel.LinkClicked += githubLinkLabel_LinkClicked;
            authorCard.Controls.Add(this.githubLinkLabel);

            y += 87;

            // Tech stack card
            var techCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(16, y),
                Size      = new Size(590, 115)
            };
            aboutScroll.Controls.Add(techCard);
            techCard.Controls.Add(MakeSectionHeader("Technology", new Point(14, 10)));

            this.aboutTechLabel.Text =
                "  \u2022  .NET 10.0 / Windows Forms\r\n" +
                "  \u2022  SharpHook v7.1.1  — global keyboard & mouse hook (MIT)\r\n" +
                "  \u2022  System.Management v10.0.5  — WMI hardware information\r\n" +
                "  \u2022  Windows Performance Counters  — CPU, memory, disk, network metrics";
            this.aboutTechLabel.Font      = new Font("Segoe UI", 9.5f);
            this.aboutTechLabel.ForeColor = Color.FromArgb(55, 55, 55);
            this.aboutTechLabel.AutoSize  = false;
            this.aboutTechLabel.Size      = new Size(562, 88);
            this.aboutTechLabel.Location  = new Point(14, 30);
            techCard.Controls.Add(this.aboutTechLabel);

            y += 127;

            // Privacy & License card
            var licenseCard = new Panel
            {
                BackColor = CardBackground,
                Location  = new Point(16, y),
                Size      = new Size(590, 100)
            };
            aboutScroll.Controls.Add(licenseCard);
            licenseCard.Controls.Add(MakeSectionHeader("Privacy & License", new Point(14, 10)));

            this.aboutPrivacyLabel.Text =
                "Keystrokes are counted, never recorded. All data is stored locally; " +
                "no information is transmitted over the network.";
            this.aboutPrivacyLabel.Font      = new Font("Segoe UI", 9.5f);
            this.aboutPrivacyLabel.ForeColor = Color.FromArgb(55, 55, 55);
            this.aboutPrivacyLabel.AutoSize  = false;
            this.aboutPrivacyLabel.Size      = new Size(562, 36);
            this.aboutPrivacyLabel.Location  = new Point(14, 30);
            licenseCard.Controls.Add(this.aboutPrivacyLabel);

            this.aboutLicenseLabel.Text      = "Released under the MIT License";
            this.aboutLicenseLabel.Font      = new Font("Segoe UI", 9.5f, FontStyle.Italic);
            this.aboutLicenseLabel.ForeColor = SubtleText;
            this.aboutLicenseLabel.AutoSize  = true;
            this.aboutLicenseLabel.Location  = new Point(14, 70);
            licenseCard.Controls.Add(this.aboutLicenseLabel);

            // ══════════════════════════════════════════════════════════════
            // FORM
            // ══════════════════════════════════════════════════════════════
            this.AutoScaleDimensions = new SizeF(7F, 15F);
            this.AutoScaleMode       = AutoScaleMode.Font;
            this.ClientSize          = new Size(640, 530);
            this.MinimumSize         = new Size(640, 570);
            this.Controls.Add(this.tabControl);
            this.Name          = "StatsForm";
            this.StartPosition = FormStartPosition.CenterScreen;
            this.Text          = "KeyPressCounter Statistics";
            this.BackColor     = FormBackground;
            this.KeyPreview    = true;

            toolTip.SetToolTip(this.systemInfoButton, "Open detailed hardware and system information.");
            toolTip.SetToolTip(this.perfGraphPanel, "CPU and memory usage history over the last minute.");

            this.tabControl.ResumeLayout(false);
            this.ResumeLayout(false);
        }

        // ── Helper factory methods ─────────────────────────────────────────

        private static Label MakeCaption(string text, Point location) => new()
        {
            Text      = text,
            Font      = new Font("Segoe UI", 8f),
            ForeColor = SubtleText,
            AutoSize  = true,
            Location  = location
        };

        private static Label MakeSectionHeader(string text, Point location) => new()
        {
            Text      = text.ToUpperInvariant(),
            Font      = new Font("Segoe UI", 8f, FontStyle.Bold),
            ForeColor = SubtleText,
            AutoSize  = true,
            Location  = location
        };

        #endregion

        // ── Control fields ─────────────────────────────────────────────────
        private TabControl tabControl = null!;
        private TabPage inputStatsTab = null!;
        private TabPage sysPerformanceTab = null!;
        private TabPage aboutTab = null!;

        // Input Statistics
        private Panel inputHeaderPanel = null!;
        private Label inputStatsHeaderLabel = null!;
        private GroupBox keyboardGroupBox = null!;
        private GroupBox mouseGroupBox = null!;
        private Label keystrokeCountLabel = null!;
        private Label keystrokesPerMinuteLabel = null!;
        private Label mouseClickCountLabel = null!;
        private Label mouseClicksPerMinuteLabel = null!;
        private Label longestIdleLabel = null!;

        // System Performance
        private Panel perfHeaderPanel = null!;
        private Label performanceHeaderLabel = null!;
        private GroupBox cpuGroupBox = null!;
        private GroupBox diskNetGroupBox = null!;
        private Label cpuUsageLabel = null!;
        private Label memoryUsageLabel = null!;
        private Label diskReadLabel = null!;
        private Label diskWriteLabel = null!;
        private Label networkDownloadLabel = null!;
        private Label networkUploadLabel = null!;
        private Label uptimeLabel = null!;
        private Button systemInfoButton = null!;
        private Panel perfGraphPanel = null!;

        // About
        private Panel aboutHeaderPanel = null!;
        private Label aboutTitleLabel = null!;
        private Label aboutVersionLabel = null!;
        private Label aboutDescLabel = null!;
        private LinkLabel githubLinkLabel = null!;
        private Label aboutTechLabel = null!;
        private Label aboutLicenseLabel = null!;
        private Label aboutPrivacyLabel = null!;
    }
}
