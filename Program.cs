using System.Diagnostics;
using System.Windows.Forms;

namespace MWH.KeyPressCounter;

/// <summary>
/// Main entry point for the KeyPressCounter application.
/// </summary>
internal static class Program
{
    // Application name for logging purposes
    private const string APPLICATION_NAME = "KeyPressCounter";

    /// <summary>
    /// The main entry point for the application.
    /// </summary>
    [STAThread]
    static void Main()
    {
        // Enable visual styles for better UI appearance
        ApplicationConfiguration.Initialize();

        try
        {
            // Allow only one instance of the application to run
            if (IsApplicationAlreadyRunning())
            {
                MessageBox.Show("Another instance of KeyPressCounter is already running.",
                    "Application Already Running", MessageBoxButtons.OK, MessageBoxIcon.Information);
                return;
            }

            // Set up unhandled exception handling for the application
            Application.ThreadException += Application_ThreadException;
            AppDomain.CurrentDomain.UnhandledException += CurrentDomain_UnhandledException;

            // Log application startup
            Debug.WriteLine($"{APPLICATION_NAME} starting at {DateTime.Now}");

            // Start the application with our custom context
            Application.Run(new CustomApplicationContext());
        }
        catch (Exception ex)
        {
            Debug.WriteLine($"Unhandled exception in Main: {ex}");
            MessageBox.Show($"An unexpected error occurred: {ex.Message}\n\nThe application will now exit.",
                "Application Error", MessageBoxButtons.OK, MessageBoxIcon.Error);
        }
    }

    /// <summary>
    /// Checks if another instance of the application is already running.
    /// </summary>
    /// <returns>True if another instance is running, false otherwise.</returns>
    private static bool IsApplicationAlreadyRunning()
    {
        Process currentProcess = Process.GetCurrentProcess();
        Process[] processes = Process.GetProcessesByName(currentProcess.ProcessName);
        
        // If there's more than one process with the same name, the application is already running
        return processes.Length > 1;
    }

    /// <summary>
    /// Handles unhandled exceptions in the UI thread.
    /// </summary>
    private static void Application_ThreadException(object sender, System.Threading.ThreadExceptionEventArgs e)
    {
        Debug.WriteLine($"Thread exception: {e.Exception}");
        MessageBox.Show($"An unexpected error occurred: {e.Exception.Message}\n\nThe application will now exit.",
            "Thread Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);
        Application.Exit();
    }

    /// <summary>
    /// Handles unhandled exceptions in non-UI threads.
    /// </summary>
    private static void CurrentDomain_UnhandledException(object sender, UnhandledExceptionEventArgs e)
    {
        string errorMessage = "A critical error occurred.";
        
        if (e.ExceptionObject is Exception ex)
        {
            Debug.WriteLine($"Unhandled exception: {ex}");
            errorMessage = $"A critical error occurred: {ex.Message}";
        }
        
        MessageBox.Show($"{errorMessage}\n\nThe application will now exit.",
            "Unhandled Exception", MessageBoxButtons.OK, MessageBoxIcon.Error);

        // Force immediate termination
        Environment.Exit(1);
    }
}