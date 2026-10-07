using System;
using System.Collections.Generic;
using System.Windows.Forms;

namespace Recorder.Configuration
{


    [Serializable]
    public class AppConfig
    {
        // Window position, size, and state
        public int WindowLeft { get; set; } = -1;
        public int WindowTop { get; set; } = -1;
        public int WindowWidth { get; set; } = 800;
        public int WindowHeight { get; set; } = 600;
        public FormWindowState WindowState { get; set; } = FormWindowState.Normal;

        // Selected ComboBox values (event shutdown time)
        public string CbTurnOffAllEventsHours { get; set; } = "19";
        public string CbTurnOffAllEventsMinutes { get; set; } = "00";
        public string CbApplicationDuration { get; set; } = "Minutes";
        public string CbApplicationDelatySwitch { get; set; } = "Seconds";

        // Form and overlay settings (Settings Tab)
        public string SettingsFormTitle { get; set; } = "Activity Recorder";
        public bool SettingsShowStatusOverlay { get; set; } = true;
        public bool SettingsMinimizeOnPlay { get; set; } = true;
        public bool SettingsMinimizeToTray { get; set; } = false;
        public bool SettingsAlwaysOnTop { get; set; } = false;
        public bool SettingsTurnOff { get; set; } = false;

        // Recording / Input settings
        public bool SettingsKeyboard { get; set; } = true;
        public bool SettingsMouseClicks { get; set; } = true;
        public bool SettingsMouseMove { get; set; } = true;
        public bool SettingsLoop { get; set; } = false;

    }
}