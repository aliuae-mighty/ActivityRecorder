using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Recorder.Core
{
    public class GlobalHookListener : IDisposable
    {
        public event EventHandler<KeyEventArgs> KeyDown;
        public event EventHandler<KeyEventArgs> KeyUp;
        public event EventHandler<MouseEventArgs> MouseMove;
        public event EventHandler<MouseEventArgs> MouseDown;
        public event EventHandler<MouseEventArgs> MouseUp;

        private LowLevelKeyboardProc _keyboardProc;
        private LowLevelMouseProc _mouseProc;

        private IntPtr _keyboardHookId = IntPtr.Zero;
        private IntPtr _mouseHookId = IntPtr.Zero;

        public bool IsStarted { get; private set; }

        public void Start()
        {
            if (IsStarted) return;

            _keyboardProc = KeyboardHookCallback;
            _mouseProc = MouseHookCallback;

            using (Process currentProcess = Process.GetCurrentProcess())
            using (ProcessModule currentModule = currentProcess.MainModule)
            {
                IntPtr moduleHandle = GetModuleHandle(currentModule.ModuleName);

                _keyboardHookId = SetWindowsHookEx(WH_KEYBOARD_LL, _keyboardProc, moduleHandle, 0);
                _mouseHookId = SetWindowsHookEx(WH_MOUSE_LL, _mouseProc, moduleHandle, 0);
            }

            IsStarted = true;
        }

        public void Stop()
        {
            if (!IsStarted) return;

            if (_keyboardHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_keyboardHookId);
                _keyboardHookId = IntPtr.Zero;
            }

            if (_mouseHookId != IntPtr.Zero)
            {
                UnhookWindowsHookEx(_mouseHookId);
                _mouseHookId = IntPtr.Zero;
            }

            IsStarted = false;
        }

        #region Helper Methods

        private POINT GetCurrentMousePosition()
        {
            // First, try to get physical coordinates without DPI virtualization
            if (GetPhysicalCursorPos(out POINT pt))
            {
                return pt;
            }
            // Fallback to GetCursorPos if GetPhysicalCursorPos fails
            if (GetCursorPos(out pt))
            {
                return pt;
            }

            // Return 0,0 in case of an error
            return new POINT { x = 0, y = 0 };
        }

        #endregion

        #region Callbacks

        private IntPtr KeyboardHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int vkCode = Marshal.ReadInt32(lParam);
                Keys key = (Keys)vkCode;

                int msg = wParam.ToInt32();
                if (msg == WM_KEYDOWN || msg == WM_SYSKEYDOWN)
                {
                    KeyDown?.Invoke(this, new KeyEventArgs(key));
                }
                else if (msg == WM_KEYUP || msg == WM_SYSKEYUP)
                {
                    KeyUp?.Invoke(this, new KeyEventArgs(key));
                }
            }

            return CallNextHookEx(_keyboardHookId, nCode, wParam, lParam);
        }

        private IntPtr MouseHookCallback(int nCode, IntPtr wParam, IntPtr lParam)
        {
            if (nCode >= 0)
            {
                int msg = wParam.ToInt32();

                // Get accurate physical cursor coordinates
                POINT cursorPt = GetCurrentMousePosition();

                switch (msg)
                {
                    case WM_MOUSEMOVE:
                        MouseMove?.Invoke(this, new MouseEventArgs(MouseButtons.None, 0, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_LBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_LBUTTONUP:
                        MouseUp?.Invoke(this, new MouseEventArgs(MouseButtons.Left, 1, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_RBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseEventArgs(MouseButtons.Right, 1, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_RBUTTONUP:
                        MouseUp?.Invoke(this, new MouseEventArgs(MouseButtons.Right, 1, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_MBUTTONDOWN:
                        MouseDown?.Invoke(this, new MouseEventArgs(MouseButtons.Middle, 1, cursorPt.x, cursorPt.y, 0));
                        break;

                    case WM_MBUTTONUP:
                        MouseUp?.Invoke(this, new MouseEventArgs(MouseButtons.Middle, 1, cursorPt.x, cursorPt.y, 0));
                        break;
                }
            }

            return CallNextHookEx(_mouseHookId, nCode, wParam, lParam);
        }

        #endregion

        #region Win32 Native Imports

        private const int WH_KEYBOARD_LL = 13;
        private const int WH_MOUSE_LL = 14;

        private const int WM_KEYDOWN = 0x0100;
        private const int WM_KEYUP = 0x0101;
        private const int WM_SYSKEYDOWN = 0x0104;
        private const int WM_SYSKEYUP = 0x0105;

        private const int WM_MOUSEMOVE = 0x0200;
        private const int WM_LBUTTONDOWN = 0x0201;
        private const int WM_LBUTTONUP = 0x0202;
        private const int WM_RBUTTONDOWN = 0x0204;
        private const int WM_RBUTTONUP = 0x0205;
        private const int WM_MBUTTONDOWN = 0x0207;
        private const int WM_MBUTTONUP = 0x0208;

        private delegate IntPtr LowLevelKeyboardProc(int nCode, IntPtr wParam, IntPtr lParam);
        private delegate IntPtr LowLevelMouseProc(int nCode, IntPtr wParam, IntPtr lParam);

        [StructLayout(LayoutKind.Sequential)]
        private struct POINT
        {
            public int x;
            public int y;
        }

        [StructLayout(LayoutKind.Sequential)]
        private struct MSLLHOOKSTRUCT
        {
            public POINT pt;
            public uint mouseData;
            public uint flags;
            public uint time;
            public IntPtr dwExtraInfo;
        }

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetPhysicalCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool GetCursorPos(out POINT lpPoint);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr SetWindowsHookEx(int idHook, Delegate lpfn, IntPtr hMod, uint dwThreadId);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        [return: MarshalAs(UnmanagedType.Bool)]
        private static extern bool UnhookWindowsHookEx(IntPtr hhk);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr CallNextHookEx(IntPtr hhk, int nCode, IntPtr wParam, IntPtr lParam);

        [DllImport("kernel32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern IntPtr GetModuleHandle(string lpModuleName);

        #endregion

        public void Dispose()
        {
            Stop();
        }
    }
}