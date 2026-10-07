using Recorder.Core;
using Recorder.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder.Services
{
    public class ActionRecorderOptions
    {
        public bool RecordKeyboard { get; set; } = true;
        public bool RecordMouseMove { get; set; } = true;
        public bool RecordMouseClicks { get; set; } = true;
        public bool RecordSwitchApps { get; set; } = true;
    }

    public class ActionRecorderService : IDisposable
    {
        private readonly GlobalHookListener _hookListener;
        private readonly List<ActionRecord> _recordedActions;
        private Stopwatch _stopwatch;
        private IntPtr _lastActiveHwnd = IntPtr.Zero;
        private CancellationTokenSource _playbackCts;

        public ActionRecorderOptions Options { get; set; } = new ActionRecorderOptions();
        public bool IsRecording { get; private set; }
        public bool IsPlaying { get; private set; }

        public event Action<string> StatusChanged;

        public ActionRecorderService()
        {
            _recordedActions = new List<ActionRecord>();
            _hookListener = new GlobalHookListener();

            // Subscriptions to global events
            _hookListener.KeyDown += OnKeyDown;
            _hookListener.KeyUp += OnKeyUp;
            _hookListener.MouseMove += OnMouseMove;
            _hookListener.MouseDown += OnMouseDown;
            _hookListener.MouseUp += OnMouseUp;
        }

        public void StartRecording()
        {
            _recordedActions.Clear();
            _lastActiveHwnd = IntPtr.Zero;
            _stopwatch = Stopwatch.StartNew();
            IsRecording = true;

            _hookListener.Start();
            StatusChanged?.Invoke("Recording started...");
        }

        public void StopRecording()
        {
            if (!IsRecording) return;

            IsRecording = false;
            _hookListener.Stop();
            _stopwatch?.Stop();

            StatusChanged?.Invoke($"Recording stopped. Total actions recorded: {_recordedActions.Count}");
        }

        #region Hook Event Handlers

        private void CheckAndRecordAppSwitch()
        {
            // 4. Switch Applications: tracks foreground window/process changes
            if (!Options.RecordSwitchApps) return;

            IntPtr currentHwnd = WindowFocusManager.GetForegroundWindowHandle();
            if (currentHwnd != IntPtr.Zero && currentHwnd != _lastActiveHwnd)
            {
                _lastActiveHwnd = currentHwnd;
                string processName = WindowFocusManager.GetProcessNameByHandle(currentHwnd);
                string windowTitle = WindowFocusManager.GetWindowTitleByHandle(currentHwnd);

                _recordedActions.Add(new ActionRecord
                {
                    Type = ActionType.AppSwitch,
                    TimestampMs = _stopwatch.ElapsedMilliseconds,
                    ProcessName = processName,
                    WindowTitle = windowTitle,
                    WindowHandle = currentHwnd
                });
            }
        }

        private void OnKeyDown(object sender, KeyEventArgs e)
        {
            if (!IsRecording || !Options.RecordKeyboard) return;

            CheckAndRecordAppSwitch();

            // 1. Keyboard: records key press
            _recordedActions.Add(new ActionRecord
            {
                Type = ActionType.KeyDown,
                TimestampMs = _stopwatch.ElapsedMilliseconds,
                KeyCode = (int)e.KeyCode,
                KeyName = e.KeyCode.ToString()
            });
        }

        private void OnKeyUp(object sender, KeyEventArgs e)
        {
            if (!IsRecording || !Options.RecordKeyboard) return;

            CheckAndRecordAppSwitch();

            _recordedActions.Add(new ActionRecord
            {
                Type = ActionType.KeyUp,
                TimestampMs = _stopwatch.ElapsedMilliseconds,
                KeyCode = (int)e.KeyCode,
                KeyName = e.KeyCode.ToString()
            });
        }

        private void OnMouseMove(object sender, MouseEventArgs e)
        {
            // 2. Mouse Move: records mouse coordinates
            if (!IsRecording || !Options.RecordMouseMove) return;

            _recordedActions.Add(new ActionRecord
            {
                Type = ActionType.MouseMove,
                TimestampMs = _stopwatch.ElapsedMilliseconds,
                X = e.X,
                Y = e.Y
            });
        }

        private void OnMouseDown(object sender, MouseEventArgs e)
        {
            // 3. Mouse Clicks: records mouse button press
            if (!IsRecording || !Options.RecordMouseClicks) return;

            CheckAndRecordAppSwitch();

            _recordedActions.Add(new ActionRecord
            {
                Type = ActionType.MouseDown,
                TimestampMs = _stopwatch.ElapsedMilliseconds,
                X = e.X,
                Y = e.Y,
                MouseButton = e.Button.ToString()
            });
        }

        private void OnMouseUp(object sender, MouseEventArgs e)
        {
            // 3. Mouse Clicks: records mouse button release
            if (!IsRecording || !Options.RecordMouseClicks) return;

            _recordedActions.Add(new ActionRecord
            {
                Type = ActionType.MouseUp,
                TimestampMs = _stopwatch.ElapsedMilliseconds,
                X = e.X,
                Y = e.Y,
                MouseButton = e.Button.ToString()
            });
        }

        #endregion

        #region Playback

        public async Task PlaybackAsync()
        {
            if (_recordedActions.Count == 0)
            {
                StatusChanged?.Invoke("No recorded actions to play.");
                return;
            }

            IsPlaying = true;
            _playbackCts = new CancellationTokenSource();
            CancellationToken token = _playbackCts.Token;

            StatusChanged?.Invoke("Playback in progress...");
            long lastTime = 0;

            try
            {
                foreach (var action in _recordedActions)
                {
                    token.ThrowIfCancellationRequested();

                    long delay = action.TimestampMs - lastTime;
                    if (delay > 0)
                    {
                        await Task.Delay((int)delay, token);
                    }
                    lastTime = action.TimestampMs;

                    switch (action.Type)
                    {
                        case ActionType.AppSwitch:
                            WindowFocusManager.FocusApplication(action.WindowHandle);                            
                            break;

                        case ActionType.MouseMove:
                            InputSimulator.SetCursorPos(action.X, action.Y);
                            break;

                        case ActionType.MouseDown:
                            InputSimulator.MouseButtonDown(action.X, action.Y, action.MouseButton);
                            break;

                        case ActionType.MouseUp:
                            InputSimulator.MouseButtonUp(action.X, action.Y, action.MouseButton);
                            break;

                        case ActionType.KeyDown:
                            InputSimulator.KeyDown((byte)action.KeyCode);
                            break;

                        case ActionType.KeyUp:
                            InputSimulator.KeyUp((byte)action.KeyCode);
                            break;
                    }
                }

                StatusChanged?.Invoke("Playback completed successfully.");
            }
            catch (OperationCanceledException)
            {
                StatusChanged?.Invoke("Playback stopped by user.");
            }
            catch (Exception ex)
            {
                StatusChanged?.Invoke($"Playback error: {ex.Message}");
            }
            finally
            {
                IsPlaying = false;
                _playbackCts?.Dispose();
                _playbackCts = null;
            }
        }

        public void StopPlayback()
        {
            _playbackCts?.Cancel();
        }

        #endregion

        public void Dispose()
        {
            _playbackCts?.Cancel();
            _playbackCts?.Dispose();
            _hookListener?.Dispose();
        }
    }
}