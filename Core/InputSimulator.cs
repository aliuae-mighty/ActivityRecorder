using System;
using System.Runtime.InteropServices;

namespace Recorder.Core
{
    public static class InputSimulator
    {
        // Import functions from user32.dll
        [DllImport("user32.dll", SetLastError = true)]
        public static extern bool SetCursorPos(int x, int y);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void mouse_event(uint dwFlags, uint dx, uint dy, uint dwData, UIntPtr dwExtraInfo);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        // Flags for mouse events
        private const uint MOUSEEVENTF_LEFTDOWN = 0x0002;
        private const uint MOUSEEVENTF_LEFTUP = 0x0004;
        private const uint MOUSEEVENTF_RIGHTDOWN = 0x0008;
        private const uint MOUSEEVENTF_RIGHTUP = 0x0010;
        private const uint MOUSEEVENTF_MIDDLEDOWN = 0x0020;
        private const uint MOUSEEVENTF_MIDDLEUP = 0x0040;
        private const uint MOUSEEVENTF_WHEEL = 0x0800;

        // Flags for keyboard events
        private const uint KEYEVENTF_KEYDOWN = 0x0000;
        private const uint KEYEVENTF_KEYUP = 0x0002;
        private const uint KEYEVENTF_EXTENDEDKEY = 0x0001;

        #region Mouse Control

        /// <summary>
        /// Moves the cursor to the specified screen coordinates.
        /// </summary>
        public static void MoveMouse(int x, int y)
        {
            SetCursorPos(x, y);
        }

        /// <summary>
        /// Presses down the specified mouse button at given coordinates.
        /// </summary>
        public static void MouseButtonDown(int x, int y, string mouseButton)
        {
            SetCursorPos(x, y);

            uint flag = MOUSEEVENTF_LEFTDOWN;
            string btn = mouseButton?.ToLower() ?? string.Empty;

            if (btn == "left" || btn == "0")
            {
                flag = MOUSEEVENTF_LEFTDOWN;
            }
            else if (btn == "right" || btn == "1")
            {
                flag = MOUSEEVENTF_RIGHTDOWN;
            }
            else if (btn == "middle" || btn == "2")
            {
                flag = MOUSEEVENTF_MIDDLEDOWN;
            }

            mouse_event(flag, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Releases the specified mouse button at given coordinates.
        /// </summary>
        public static void MouseButtonUp(int x, int y, string mouseButton)
        {
            SetCursorPos(x, y);

            uint flag = MOUSEEVENTF_LEFTUP;
            string btn = mouseButton?.ToLower() ?? string.Empty;

            if (btn == "left" || btn == "0")
            {
                flag = MOUSEEVENTF_LEFTUP;
            }
            else if (btn == "right" || btn == "1")
            {
                flag = MOUSEEVENTF_RIGHTUP;
            }
            else if (btn == "middle" || btn == "2")
            {
                flag = MOUSEEVENTF_MIDDLEUP;
            }

            mouse_event(flag, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Performs a left mouse button click.
        /// </summary>
        public static void ClickLeftMouseButton()
        {
            mouse_event(MOUSEEVENTF_LEFTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_LEFTUP, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Performs a right mouse button click.
        /// </summary>
        public static void ClickRightMouseButton()
        {
            mouse_event(MOUSEEVENTF_RIGHTDOWN, 0, 0, 0, UIntPtr.Zero);
            mouse_event(MOUSEEVENTF_RIGHTUP, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Presses or releases the left mouse button.
        /// </summary>
        public static void SetLeftMouseButtonState(bool down)
        {
            uint flag = down ? MOUSEEVENTF_LEFTDOWN : MOUSEEVENTF_LEFTUP;
            mouse_event(flag, 0, 0, 0, UIntPtr.Zero);
        }

        /// <summary>
        /// Scrolls the mouse wheel.
        /// </summary>
        /// <param name="scrollAmount">Positive value scrolls up, negative value scrolls down.</param>
        public static void ScrollMouseWheel(int scrollAmount)
        {
            mouse_event(MOUSEEVENTF_WHEEL, 0, 0, (uint)scrollAmount, UIntPtr.Zero);
        }

        #endregion

        #region Keyboard Control

        /// <summary>
        /// Presses down a key by its Virtual-Key Code.
        /// </summary>
        public static void KeyDown(byte virtualKeyCode)
        {
            keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYDOWN, UIntPtr.Zero);
        }

        /// <summary>
        /// Releases a key by its Virtual-Key Code.
        /// </summary>
        public static void KeyUp(byte virtualKeyCode)
        {
            keybd_event(virtualKeyCode, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);
        }

        /// <summary>
        /// Simulates a full key press (press + release).
        /// </summary>
        public static void KeyPress(byte virtualKeyCode, int delayMs = 50)
        {
            KeyDown(virtualKeyCode);
            if (delayMs > 0)
            {
                System.Threading.Thread.Sleep(delayMs);
            }
            KeyUp(virtualKeyCode);
        }

        /// <summary>
        /// Sends a two-key combination (e.g., Ctrl + C).
        /// </summary>
        public static void KeyCombination(byte modifierKeyCode, byte key)
        {
            KeyDown(modifierKeyCode);
            KeyPress(key);
            KeyUp(modifierKeyCode);
        }

        #endregion
    }
}