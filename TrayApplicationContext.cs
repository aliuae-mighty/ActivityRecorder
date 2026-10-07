using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{

    public class TrayApplicationContext : ApplicationContext
    {
        private NotifyIcon trayIcon;
        private ContextMenuStrip contextMenu;

        // Named mutex to ensure single-instance execution
        private static Mutex appMutex;
        private const string MutexName = "Global\\RAPC_SystemTrayApp_Mutex";

        public TrayApplicationContext()
        {
            // Check if an instance is already running
            appMutex = new Mutex(true, MutexName, out bool isNewInstance);

            if (!isNewInstance)
            {
                MessageBox.Show(
                    "Application 'Activity Recorder' is already running in the System Tray.",
                    "Application Exists",
                    MessageBoxButtons.OK,
                    MessageBoxIcon.Information
                );

                // Exit the duplicate instance immediately
                Environment.Exit(0);
                return;
            }

            contextMenu = new ContextMenuStrip();

            var openItem = new ToolStripMenuItem("Open", null, OpenForm_Click);
            contextMenu.Items.Add(openItem);

            contextMenu.Items.Add(new ToolStripSeparator());

            var exitItem = new ToolStripMenuItem("Exit", null, Exit_Click);
            contextMenu.Items.Add(exitItem);

            trayIcon = new NotifyIcon()
            {
                Icon = Properties.Resources.circle,
                ContextMenuStrip = contextMenu,
                Text = "Activity Recorder",
                Visible = true
            };

            trayIcon.DoubleClick += TrayIcon_DoubleClick;

            trayIcon.ShowBalloonTip(3000, "Application Started", "The application is running in the background.", ToolTipIcon.Info);

            ShowForm();
        }

        private void TrayIcon_DoubleClick(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void OpenForm_Click(object sender, EventArgs e)
        {
            ShowForm();
        }

        private void ShowForm()
        {
            // Check if the form is already open
            if (Application.OpenForms["MainForm"] is Form existingForm)
            {
                existingForm.Activate();
                existingForm.Show();
            }
            else
            {
                // Create and show a new form (replace MainForm with your form's name)
                MainForm form = new MainForm();
                form.Activate();
                form.Show();
            }
        }

        private void Exit_Click(object sender, EventArgs e)
        {
            // Hide the icon before exiting so it immediately disappears from the system tray
            trayIcon.Visible = false;
            trayIcon.Dispose();

            // Release the system-wide mutex
            if (appMutex != null)
            {
                appMutex.ReleaseMutex();
                appMutex.Dispose();
            }

            // Terminate the application process
            Application.Exit();
        }
    }

}
