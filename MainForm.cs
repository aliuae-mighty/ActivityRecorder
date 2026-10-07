using Recorder.Configuration;
using Recorder.Core;
using Recorder.Extensions;
using Recorder.Forms;
using Recorder.Services;
using System;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder
{
    public partial class MainForm : Form
    {
        private ActionRecorderService _recorderService;
        private GlobalHotkeyManager _hotkeyManager;
        private OverlayForm _overlayForm;
        private ScheduledTurnOffService _turnOffService; 

        private bool _stopPlaybackRequested = false;

        public AppConfig ConfigManager { get; set; }
        public string ApplicationStatus { get; private set; }

        public MainForm()
        {
            InitializeComponent();

            // Recording service initialization
            _recorderService = new ActionRecorderService();
            _recorderService.StatusChanged += status => UpdateStatusLabel(status);

            _turnOffService = new ScheduledTurnOffService();
            _turnOffService.Triggered += TurnOffService_Triggered;
        }

        private void TurnOffService_Triggered(object sender, EventArgs e)
        {
            StopAllActivities("Turn Off All Events time reached.");
        }

        private void MainForm_Load(object sender, EventArgs e)
        {
            UpdateConfigManager();
        }

        public void UpdateConfigManager()
        {
            ConfigManager = Configuration.ConfigManager.Load();
            Configuration.ConfigManager.ApplyToParentForm(this, ConfigManager);
        }

        private void btnRecord_Click(object sender, EventArgs e)
        {
            _recorderService.Options = new ActionRecorderOptions
            {
                RecordKeyboard = ConfigManager.SettingsKeyboard,
                RecordMouseMove = ConfigManager.SettingsMouseMove,
                RecordMouseClicks = ConfigManager.SettingsMouseClicks,
                RecordSwitchApps = false
            };

            HideOrCloseApplication();
            _recorderService.StartRecording();

            btnRecord.Enabled = false;
            btnFinish.Enabled = true;
            btnPlay.Enabled = false;

            ShowOverlay("● Recording... Press Ctrl + F8 to Finish");
        }

        private async void btnPlay_Click(object sender, EventArgs e)
        {
            HideOrCloseApplication();
            await PlayRecorderAsync();
        }

        private void btnFinish_Click(object sender, EventArgs e)
        {
            _stopPlaybackRequested = true;
            _recorderService.StopRecording();
            _recorderService.StopPlayback();

            btnRecord.Enabled = true;
            btnFinish.Enabled = false;
            btnPlay.Enabled = true;
            HideOverlay();
        }

        private void btnSettings_Click(object sender, EventArgs e)
        {
            FormManager.ShowOrActivateForm<AppSettingsForm>(null,
                controlName => new AppSettingsForm(this, null));
        }

        private void StopAllActivities(string reason)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => StopAllActivities(reason)));
                return;
            }

            if (_recorderService != null)
            {
                _stopPlaybackRequested = true;
                _recorderService.StopPlayback();
                _recorderService.StopRecording();

                btnRecord.Enabled = true;
                btnFinish.Enabled = false;
                btnPlay.Enabled = true;
            }

            HideOverlay();
            UpdateStatusLabel(reason);
        }

        protected override void OnHandleCreated(EventArgs e)
        {
            base.OnHandleCreated(e);

            _hotkeyManager = new GlobalHotkeyManager(this.Handle);

            _hotkeyManager.OnRecordRequested += () => this.Invoke(new Action(() => btnRecord_Click(this, EventArgs.Empty)));
            _hotkeyManager.OnFinishRequested += () => this.Invoke(new Action(() => btnFinish_Click(this, EventArgs.Empty)));
            _hotkeyManager.OnPlayRequested += () => this.Invoke(new Action(async () => await PlayRecorderAsync()));
            _hotkeyManager.OnStopPlaybackRequested += () => this.Invoke(new Action(() => btnFinish_Click(this, EventArgs.Empty)));

            _hotkeyManager.RegisterHotkeys();
        }

        protected override void OnHandleDestroyed(EventArgs e)
        {
            _turnOffService?.Dispose();
            _hotkeyManager?.Dispose();
            _recorderService?.Dispose();

            if (_overlayForm != null && !_overlayForm.IsDisposed)
            {
                _overlayForm.HideOverlay();
                _overlayForm.Close();
                _overlayForm.Dispose();
            }

            base.OnHandleDestroyed(e);
        }

        private void HideOrCloseApplication()
        {
            if (ConfigManager.SettingsMinimizeOnPlay && ConfigManager.SettingsMinimizeToTray)
                this.Hide();
            if (ConfigManager.SettingsMinimizeOnPlay)
                this.WindowState = FormWindowState.Minimized;
        }

        private void UpdateStatusLabel(string text)
        {
            ApplicationStatus = text;
        }

        private void ShowOverlay(string message)
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(() => ShowOverlay(message)));
                return;
            }

            if (_overlayForm == null || _overlayForm.IsDisposed)
                _overlayForm = new OverlayForm();

            _overlayForm.SetLabelTitle(message);

            if (!_overlayForm.Visible)
            {
                if (ConfigManager.SettingsShowStatusOverlay)
                    _overlayForm.ShowOverlay(message);
            }
        }

        private void HideOverlay()
        {
            if (this.InvokeRequired)
            {
                this.Invoke(new Action(HideOverlay));
                return;
            }
            if (_overlayForm != null && !_overlayForm.IsDisposed)
            { 
                _overlayForm.HideOverlay();
            }
        }

        private async Task PlayRecorderAsync()
        {
            _stopPlaybackRequested = false;

            _turnOffService.StartIfEnabled(ConfigManager);
            ShowOverlay("▶ Playing... Press Ctrl + F8 to Stop");

            try
            {
                do
                {
                    btnRecord.Enabled = false;
                    btnFinish.Enabled = true;
                    btnPlay.Enabled = false;

                    UpdateStatusLabel("Playback started...");

                    await _recorderService.PlaybackAsync();

                    if (ConfigManager.SettingsLoop && !_stopPlaybackRequested)
                    {
                        UpdateStatusLabel("Looping: restarting playback...");
                        await Task.Delay(50);
                    }
                }
                while (ConfigManager.SettingsLoop && !_stopPlaybackRequested);
            }
            finally
            {
                _turnOffService.Stop();

                btnRecord.Enabled = true;
                btnPlay.Enabled = true;
                btnFinish.Enabled = false;
                HideOverlay();
                UpdateStatusLabel("Playback completed.");
            }
        }

        private void MainForm_FormClosing(object sender, FormClosingEventArgs e)
        {
            if (e.CloseReason != CloseReason.UserClosing) return;

            if (ConfigManager.SettingsMinimizeToTray)
            {
                e.Cancel = true;
                this.Hide();
                return;
            }

            DialogResult result = MessageBox.Show(
                 "Are you sure you want to exit?",
                 "Confirm Exit",
                 MessageBoxButtons.YesNo,
                 MessageBoxIcon.Question
             );

            if (result == DialogResult.Yes)
            {
                Application.Exit();
            }
        }

        private void tsDonateLabel_Click(object sender, EventArgs e)
        {
            FormManager.ShowOrActivateForm<DonateForm>(null,
                controlName => new DonateForm(this, null));
        }
    }
}