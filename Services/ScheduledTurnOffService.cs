using System;
using System.Windows.Forms;
using Recorder.Configuration;

namespace Recorder.Services
{
    public class ScheduledTurnOffService : IDisposable
    {
        private readonly Timer _timer;
        private bool _disposed;

        /// <summary>
        /// Event triggered when the scheduled shut-down time is reached.
        /// </summary>
        public event EventHandler Triggered;

        public ScheduledTurnOffService()
        {
            _timer = new Timer
            {
                Interval = 1000 // Check every second
            };
            _timer.Tick += Timer_Tick;
        }

        /// <summary>
        /// Starts the timer if the SettingsTurnOff option is enabled in the configuration.
        /// </summary>
        public void StartIfEnabled(AppConfig config)
        {
            if (config != null && config.SettingsTurnOff)
            {
                _timer.Start();
            }
        }

        /// <summary>
        /// Stops the timer.
        /// </summary>
        public void Stop()
        {
            _timer.Stop();
        }

        private void Timer_Tick(object sender, EventArgs e)
        {
            // Method is invoked on the UI thread because it uses System.Windows.Forms.Timer
            CheckScheduledTime();
        }

        private void CheckScheduledTime()
        {
            // If the option was disabled in settings while running, stop the timer
            var config = ConfigManager.Load(); // Or pass the current config
            if (config == null || !config.SettingsTurnOff)
            {
                Stop();
                return;
            }

            string hoursStr = config.CbTurnOffAllEventsHours ?? "00";
            string minutesStr = config.CbTurnOffAllEventsMinutes ?? "00";

            if (int.TryParse(hoursStr, out int targetHours) &&
                int.TryParse(minutesStr, out int targetMinutes))
            {
                DateTime now = DateTime.Now;
                if (now.Hour == targetHours && now.Minute == targetMinutes)
                {
                    Stop();
                    Triggered?.Invoke(this, EventArgs.Empty);
                }
            }
        }

        public void Dispose()
        {
            if (_disposed) return;

            _timer?.Stop();
            _timer?.Dispose();
            _disposed = true;
        }
    }
}