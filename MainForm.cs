using Gma.System.MouseKeyHook;
using System.Windows.Forms.ComponentModel;
using System.IO;
using System;
using System.Windows.Forms;
using System.Windows.Forms.VisualStyles;

namespace mwhKeyLogger
{

    public partial class MainForm : Form
    {
        private IKeyboardMouseEvents globalHook;
        private int keyPressCount = 0;
        private int mouseClickCount = 0;
        private readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ActivityLog.txt");

        public MainForm()
        {
            InitializeComponent();
            StartGlobalHooks();
            InitializeTrayIcon();
        }

        private void StartGlobalHooks()
        {
            globalHook = Hook.GlobalEvents();
            globalHook.KeyPress += GlobalHookKeyPress;
            globalHook.MouseClick += GlobalHookMouseClick;
        }

        private void GlobalHookKeyPress(object sender, KeyPressEventArgs e)
        {
            keyPressCount++;
            UpdateLog();
        }

        private void GlobalHookMouseClick(object sender, MouseEventArgs e)
        {
            mouseClickCount++;
            UpdateLog();
        }

        private void UpdateLog()
        {
            string log = $"{DateTime.Now:yyyy-MM-dd}: Keystrokes: {keyPressCount}, Mouse Clicks: {mouseClickCount}";
            File.WriteAllText(logFilePath, log);
        }

        private void InitializeTrayIcon()
        {
            NotifyIcon trayIcon = new NotifyIcon
            {
                Icon = SystemIcons.Application,
                Visible = true,
                Text = "Activity Tracker"
            };

            trayIcon.DoubleClick += (sender, args) => { this.Show(); this.WindowState = FormWindowState.Normal; };
            ContextMenuStrip trayMenu = new();
            trayMenu.Items.Add("Exit", null, OnExit); // null can be replaced with an image
            trayIcon.ContextMenuStrip = trayMenu;
        }

        private void OnExit(object sender, EventArgs e)
        {
            globalHook.KeyPress -= GlobalHookKeyPress;
            globalHook.MouseClick -= GlobalHookMouseClick;
            globalHook.Dispose();
            Application.Exit();
        }

        protected override void OnResize(EventArgs e)
        {
            base.OnResize(e);
            if (this.WindowState == FormWindowState.Minimized) { this.Hide(); }
        }
    }
}
