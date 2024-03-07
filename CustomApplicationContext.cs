using Gma.System.MouseKeyHook;
using System;
using System.IO;
using System.Windows.Forms;

public class CustomApplicationContext : ApplicationContext
{
    private IKeyboardMouseEvents globalHook;
    private int keyPressCount = 0;
    private int mouseClickCount = 0;
    private readonly string logFilePath = Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.MyDocuments), "ActivityLog.txt");
    private NotifyIcon trayIcon;

    public CustomApplicationContext()
    {
        InitializeContext();
        StartGlobalHooks();
    }

    private void InitializeContext()
    {
        trayIcon = new NotifyIcon
        {
            Icon = SystemIcons.Application,
            ContextMenuStrip = new ContextMenuStrip(),
            Visible = true,
            Text = "Activity Tracker"
        };
        trayIcon.ContextMenuStrip.Items.Add("Exit", null, OnExit);

        // Optional: Double click to show some UI or perform an action
        // trayIcon.DoubleClick += TrayIcon_DoubleClick;
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

    private void OnExit(object sender, EventArgs e)
    {
        globalHook.KeyPress -= GlobalHookKeyPress;
        globalHook.MouseClick -= GlobalHookMouseClick;
        globalHook.Dispose();
        trayIcon.Visible = false;
        Application.Exit();
    }
}
