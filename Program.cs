using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    internal static class Program
    {
        [DllImport("user32.dll")]
        private static extern bool SetProcessDPIAware();

        [DllImport("user32.dll")]
        private static extern bool SetProcessDpiAwarenessContext(IntPtr value);

        private static readonly IntPtr DPI_AWARENESS_CONTEXT_PER_MONITOR_AWARE_V2 = new IntPtr(-4);

        /// <summary>
        /// The main entry point for the application.
        /// </summary>
        [STAThread]
        static void Main()
        {
            Application.EnableVisualStyles();
            Application.SetCompatibleTextRenderingDefault(false);

            // Handle unhandled exceptions on the UI thread
            Application.ThreadException += new ThreadExceptionEventHandler(GlobalThreadExceptionHandler);

            // Force all unhandled UI exceptions to go through ThreadException
            Application.SetUnhandledExceptionMode(UnhandledExceptionMode.CatchException);

            // Handle unhandled exceptions on non-UI background threads
            AppDomain.CurrentDomain.UnhandledException += new UnhandledExceptionEventHandler(GlobalDomainExceptionHandler);

            Application.Run(new TrayApplicationContext());
        }

        private static void GlobalThreadExceptionHandler(object sender, ThreadExceptionEventArgs e)
        {
            // Ignore ERROR_ACCESS_DENIED (5) exceptions caused by protected system processes
            if (e.Exception is System.ComponentModel.Win32Exception winEx && winEx.NativeErrorCode == 5)
            {
                return;
            }

            MessageBox.Show(
                $"An unexpected error occurred: {e.Exception.Message}",
                "Error",
                MessageBoxButtons.OK,
                MessageBoxIcon.Error
            );
        }

        private static void GlobalDomainExceptionHandler(object sender, UnhandledExceptionEventArgs e)
        {
            if (e.ExceptionObject is Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Unhandled Domain Exception: {ex.Message}");
            }
        }
    }
}

