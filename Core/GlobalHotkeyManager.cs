using System;
using System.Runtime.InteropServices;
using System.Threading;
using System.Threading.Tasks;
using System.Windows.Forms;

namespace Recorder.Core
{
    public class GlobalHotkeyManager : IDisposable
    {
        [DllImport("user32.dll")]
        private static extern bool RegisterHotKey(IntPtr hWnd, int id, uint fsModifiers, uint vk);

        [DllImport("user32.dll")]
        private static extern bool UnregisterHotKey(IntPtr hWnd, int id);

        [DllImport("user32.dll")]
        private static extern short GetAsyncKeyState(int vKey);

        public const int HOTKEY_ID_F5 = 1001;
        public const int HOTKEY_ID_F6 = 1002;
        public const int HOTKEY_ID_F7 = 1003;
        public const int HOTKEY_ID_F8 = 1004;
        public const int HOTKEY_ID_F9 = 1005;
        public const int HOTKEY_ID_F10 = 1006;

        private const uint VK_F5 = 0x74;
        private const uint VK_F6 = 0x75;
        private const uint VK_F7 = 0x76;
        private const uint VK_F8 = 0x77;
        private const uint VK_F9 = 0x78;
        private const uint VK_F10 = 0x79;

        private const uint MOD_CONTROL = 0x0002;
        private const int VK_CONTROL = 0x11;
        public const int WM_HOTKEY = 0x0312;

        private CancellationTokenSource _keyMonitorCts;
        private readonly IntPtr _windowHandle;

        public event Action OnStartRequested;
        public event Action OnStopRequested;
        public event Action OnTogglePauseRequested;
        public event Action OnRecordRequested;
        public event Action OnFinishRequested;
        public event Action OnPlayRequested;
        public event Action OnStopPlaybackRequested;

        public GlobalHotkeyManager(IntPtr windowHandle)
        {
            _windowHandle = windowHandle;
        }

        public void RegisterHotkeys()
        {
            RegisterHotKey(_windowHandle, HOTKEY_ID_F5, MOD_CONTROL, VK_F5);
            RegisterHotKey(_windowHandle, HOTKEY_ID_F6, MOD_CONTROL, VK_F6);
            RegisterHotKey(_windowHandle, HOTKEY_ID_F7, MOD_CONTROL, VK_F7);
            RegisterHotKey(_windowHandle, HOTKEY_ID_F8, MOD_CONTROL, VK_F8);
            RegisterHotKey(_windowHandle, HOTKEY_ID_F9, MOD_CONTROL, VK_F9);
            RegisterHotKey(_windowHandle, HOTKEY_ID_F10, MOD_CONTROL, VK_F10);
            StartGlobalCtrlListener();
        }

        public void ProcessMessage(Message m)
        {
            if (m.Msg == WM_HOTKEY)
            {
                int id = m.WParam.ToInt32();
                switch (id)
                {
                    case HOTKEY_ID_F5: OnStartRequested?.Invoke(); break;
                    case HOTKEY_ID_F6: OnStopRequested?.Invoke(); break;
                    case HOTKEY_ID_F7: OnRecordRequested?.Invoke(); break;
                    case HOTKEY_ID_F8: OnFinishRequested?.Invoke(); break;
                    case HOTKEY_ID_F9: OnPlayRequested?.Invoke(); break;
                    case HOTKEY_ID_F10: OnStopPlaybackRequested?.Invoke(); break;
                }
            }
        }

        private void StartGlobalCtrlListener()
        {
            _keyMonitorCts = new CancellationTokenSource();
            CancellationToken token = _keyMonitorCts.Token;

            Task.Run(async () =>
            {
                bool wasPressed = false;

                while (!token.IsCancellationRequested)
                {
                    bool isPressed = (GetAsyncKeyState(VK_CONTROL) & 0x8000) != 0;
                    bool isF5 = (GetAsyncKeyState((int)VK_F5) & 0x8000) != 0;
                    bool isF6 = (GetAsyncKeyState((int)VK_F6) & 0x8000) != 0;

                    if (isPressed && !wasPressed && !isF5 && !isF6)
                    {
                        wasPressed = true;
                        //OnTogglePauseRequested?.Invoke();
                    }
                    else if (!isPressed)
                    {
                        wasPressed = false;
                    }

                    await Task.Delay(50, token);
                }
            }, token);
        }

        public void Dispose()
        {
            UnregisterHotKey(_windowHandle, HOTKEY_ID_F5);
            UnregisterHotKey(_windowHandle, HOTKEY_ID_F6);
            _keyMonitorCts?.Cancel();
            _keyMonitorCts?.Dispose();
        }
    }
}