using System;
using System.ComponentModel;
using System.Data;
using System.Drawing;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder.Forms
{
    public partial class OverlayForm : Form
    {
        private System.Windows.Forms.Timer _timer;
        private int _secondsElapsed;
        private string _baseHintText = string.Empty;

        public OverlayForm()
        {
            InitializeComponent();

            // Disable taskbar entry and set the form to TopMost by default
            this.ShowInTaskbar = false;
            this.TopMost = true;
            this.StartPosition = FormStartPosition.Manual;
            this.FormBorderStyle = FormBorderStyle.None;

            // Timer initialization
            InitTimer();
        }

        private void InitTimer()
        {
            _timer = new System.Windows.Forms.Timer();
            _timer.Interval = 1000; // 1 second
            _timer.Tick += Timer_Tick;
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            _secondsElapsed++;
            UpdateTextWithTimer();
        }

        private void UpdateTextWithTimer()
        {
            TimeSpan time = TimeSpan.FromSeconds(_secondsElapsed);
            string timeString = time.ToString(@"mm\:ss");

            // Format the final text (e.g.: "Hint text (01:15)")
            string fullText = string.IsNullOrEmpty(_baseHintText)
                ? timeString
                : $"{_baseHintText} ({timeString})";

            if (lblHint.InvokeRequired)
            {
                lblHint.Invoke(new Action(() => lblHint.Text = fullText));
            }
            else
            {
                lblHint.Text = fullText;
            }
        }

        public void SetLabelTitle(string text)
        {
            _baseHintText = text;
            UpdateTextWithTimer();
        }

        public void ShowOverlay(string hintText)
        {
            _baseHintText = hintText;

            // Reset and start the timer
            _secondsElapsed = 0;
            UpdateTextWithTimer();
            _timer.Start();

            if (!this.Visible)
            {
                // Display the form without taking focus
                //ShowInactiveTopmost();
                Show();
            }
        }

        public void HideOverlay()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HideOverlay));
                return;
            }

            // Stop and reset the timer when hiding
            _timer.Stop();
            _secondsElapsed = 0;

            if (this.Visible)
            {
                this.Hide();
            }
        }

        private void ShowInactiveTopmost()
        {
            // SW_SHOWNOACTIVATE = 4 (shows the window without activating it)
            ShowWindow(this.Handle, 4);
        }

        protected override void OnLoad(EventArgs e)
        {
            base.OnLoad(e);

            // Rounded corners
            this.Region = System.Drawing.Region.FromHrgn(CreateRoundRectangleRgn(0, 0, this.Width, this.Height, 10, 10));

            // Position at the top-center of the screen
            Rectangle screen = Screen.PrimaryScreen.WorkingArea;
            int x = (screen.Width - this.Width) / 2;
            int y = 10;
            this.Location = new Point(x, y);

            // Extended styles: mouse click-through + hide from Alt+Tab
            int exStyle = GetWindowLong(this.Handle, GWL_EXSTYLE);
            SetWindowLong(this.Handle, GWL_EXSTYLE, exStyle | WS_EX_TRANSPARENT | WS_EX_TOOLWINDOW);
        }

        #region WinAPI Native Methods

        private const int GWL_EXSTYLE = -20;
        private const int WS_EX_TRANSPARENT = 0x20;
        private const int WS_EX_TOOLWINDOW = 0x80;

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int GetWindowLong(IntPtr hWnd, int nIndex);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern int SetWindowLong(IntPtr hWnd, int nIndex, int dwNewLong);

        [System.Runtime.InteropServices.DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [System.Runtime.InteropServices.DllImport("gdi32.dll", EntryPoint = "CreateRoundRectRgn")]
        private static extern IntPtr CreateRoundRectangleRgn(int x1, int y1, int x2, int y2, int cx, int cy);

        #endregion
    }
}