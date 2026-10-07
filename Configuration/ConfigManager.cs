using Recorder.Forms;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Xml.Serialization;

namespace Recorder.Configuration
{
    public static class ConfigManager
    {
        private static readonly string ConfigFileName = "app.xml";

        private static string ConfigPath => Path.Combine(AppDomain.CurrentDomain.BaseDirectory, ConfigFileName);

        /// <summary>
        /// Loads settings from an XML file.
        /// </summary>
        public static AppConfig Load()
        {
            if (!File.Exists(ConfigPath))
                return new AppConfig();

            try
            {
                var serializer = new XmlSerializer(typeof(AppConfig));
                using (var stream = new FileStream(ConfigPath, FileMode.Open, FileAccess.Read))
                {
                    return (AppConfig)serializer.Deserialize(stream) ?? new AppConfig();
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error loading config: {ex.Message}");
                return new AppConfig();
            }
        }

        /// <summary>
        /// Saves settings to an XML file.
        /// </summary>
        public static void Save(AppConfig config)
        {
            try
            {
                var serializer = new XmlSerializer(typeof(AppConfig));
                using (var stream = new FileStream(ConfigPath, FileMode.Create, FileAccess.Write))
                {
                    serializer.Serialize(stream, config);
                }
            }
            catch (Exception ex)
            {
                System.Diagnostics.Debug.WriteLine($"Error saving config: {ex.Message}");
            }
        }

        public static void ApplyToParentForm(MainForm parentForm, AppConfig config)
        {
            if (parentForm == null || config == null) return;

            // 1. Window position, size, and state
            ApplyWindowBounds(parentForm, config);

            if (!string.IsNullOrWhiteSpace(config.SettingsFormTitle))
                parentForm.Text = config.SettingsFormTitle;

            parentForm.TopMost =  config.SettingsAlwaysOnTop;
        }

        /// <summary>
        /// Applies settings from an AppConfig object to form controls and window properties.
        /// </summary>
        public static void ApplyToForm(AppSettingsForm form, AppConfig config)
        {
            if (form == null || config == null) return;

            // 2. ComboBox (event disable time)
            SetComboBoxValue(form.cbTurnOffAllEventsHours, config.CbTurnOffAllEventsHours);
            SetComboBoxValue(form.cbTurnOffAllEventsMinutes, config.CbTurnOffAllEventsMinutes);

            // 3. Settings Controls
            if (form.tbSettingsFormTitle != null)
                form.tbSettingsFormTitle.Text = config.SettingsFormTitle;

            // 4. CheckBox Settings
            if (form.cbSettingsShowStatusOverlay != null)
                form.cbSettingsShowStatusOverlay.Checked = config.SettingsShowStatusOverlay;

            if (form.cbSettingsMinimizeOnPlay != null)
                form.cbSettingsMinimizeOnPlay.Checked = config.SettingsMinimizeOnPlay;

            if (form.cbSettingsMinimizeToTray != null)
                form.cbSettingsMinimizeToTray.Checked = config.SettingsMinimizeToTray;

            if (form.cbSettingsAlwaysOnTop != null)
                form.cbSettingsAlwaysOnTop.Checked = config.SettingsAlwaysOnTop;

            if (form.cbSettingsKeyboard != null)
                form.cbSettingsKeyboard.Checked = config.SettingsKeyboard;

            if (form.cbSettingsMouseClicks != null)
                form.cbSettingsMouseClicks.Checked = config.SettingsMouseClicks;

            if (form.cbSettingsMouseMove != null)
                form.cbSettingsMouseMove.Checked = config.SettingsMouseMove;

            if (form.cbSettingsLoop != null)
                form.cbSettingsLoop.Checked = config.SettingsLoop;

            if (form.checkTurnOffAllEvents != null)
                form.checkTurnOffAllEvents.Checked = config.SettingsTurnOff;

            // Apply properties directly to the form
            if (!string.IsNullOrWhiteSpace(config.SettingsFormTitle))
                form.ParentForm.Text = config.SettingsFormTitle;

            form.TopMost = form.ParentForm.TopMost = config.SettingsAlwaysOnTop;
        }

        /// <summary>
        /// Extracts current values from the form and controls into an AppConfig object.
        /// </summary>
        public static AppConfig ExtractFromForm(AppSettingsForm form)
        {
            if (form == null) return new AppConfig();

            // Get exact bounds of original size (RestoreBounds) if window is maximized or minimized
            Rectangle bounds = form.ParentForm.WindowState == FormWindowState.Normal ? form.ParentForm.Bounds : form.ParentForm.RestoreBounds;

            return new AppConfig
            {
                // Window position and size
                WindowLeft = bounds.X,
                WindowTop = bounds.Y,
                WindowWidth = bounds.Width,
                WindowHeight = bounds.Height,
                WindowState = form.WindowState == FormWindowState.Minimized ? FormWindowState.Normal : form.WindowState,

                // ComboBox time
                CbTurnOffAllEventsHours = form.cbTurnOffAllEventsHours?.SelectedItem?.ToString() ?? form.cbTurnOffAllEventsHours?.Text ?? "00",
                CbTurnOffAllEventsMinutes = form.cbTurnOffAllEventsMinutes?.SelectedItem?.ToString() ?? form.cbTurnOffAllEventsMinutes?.Text ?? "00",

                // Form settings
                SettingsFormTitle = form.tbSettingsFormTitle?.Text ?? form.Text,
                SettingsShowStatusOverlay = form.cbSettingsShowStatusOverlay?.Checked ?? true,
                SettingsMinimizeOnPlay = form.cbSettingsMinimizeOnPlay?.Checked ?? false,
                SettingsMinimizeToTray = form.cbSettingsMinimizeToTray?.Checked ?? false,
                SettingsAlwaysOnTop = form.cbSettingsAlwaysOnTop?.Checked ?? form.TopMost,

                // CheckBox settings
                SettingsKeyboard = form.cbSettingsKeyboard?.Checked ?? true,
                SettingsMouseClicks = form.cbSettingsMouseClicks?.Checked ?? true,
                SettingsMouseMove = form.cbSettingsMouseMove?.Checked ?? true,
                SettingsLoop = form.cbSettingsLoop?.Checked ?? false,

                SettingsTurnOff = form.checkTurnOffAllEvents?.Checked ?? false
            };
        }

        private static void ApplyWindowBounds(MainForm form, AppConfig config)
        {
            if (config.WindowLeft >= 0 && config.WindowTop >= 0 && config.WindowWidth > 100 && config.WindowHeight > 100)
            {
                var targetBounds = new Rectangle(config.WindowLeft, config.WindowTop, config.WindowWidth, config.WindowHeight);

                if (IsVisibleOnAnyScreen(targetBounds))
                {
                    form.StartPosition = FormStartPosition.Manual;
                    form.DesktopBounds = targetBounds;
                }
            }

            if (config.WindowState == FormWindowState.Maximized)
            {
                form.WindowState = FormWindowState.Maximized;
            }
        }

        private static bool IsVisibleOnAnyScreen(Rectangle rect)
        {
            foreach (var screen in Screen.AllScreens)
            {
                if (screen.WorkingArea.IntersectsWith(rect))
                {
                    return true;
                }
            }
            return false;
        }

        private static void SetComboBoxValue(ComboBox cb, string value)
        {
            if (cb == null || string.IsNullOrEmpty(value)) return;

            int index = cb.FindStringExact(value);
            if (index != -1)
            {
                cb.SelectedIndex = index;
            }
            else
            {
                cb.Text = value;
            }
        }
    }
}