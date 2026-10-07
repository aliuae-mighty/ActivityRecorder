using System;
using System.Diagnostics;
using System.Runtime.InteropServices;
using System.Text;

namespace Recorder.Core
{
    public static class WindowFocusManager
    {
        // Win32 API Imports
        [DllImport("user32.dll")]
        private static extern IntPtr GetForegroundWindow();

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowText(IntPtr hWnd, StringBuilder lpString, int nMaxCount);

        [DllImport("user32.dll", CharSet = CharSet.Auto, SetLastError = true)]
        private static extern int GetWindowTextLength(IntPtr hWnd);

        [DllImport("user32.dll", SetLastError = true)]
        private static extern uint GetWindowThreadProcessId(IntPtr hWnd, out uint lpdwProcessId);

        [DllImport("user32.dll")]
        private static extern bool SetForegroundWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsIconic(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool ShowWindow(IntPtr hWnd, int nCmdShow);

        [DllImport("user32.dll")]
        private static extern bool AttachThreadInput(uint idAttach, uint idAttachTo, bool fAttach);

        [DllImport("user32.dll")]
        private static extern void keybd_event(byte bVk, byte bScan, uint dwFlags, UIntPtr dwExtraInfo);

        [DllImport("user32.dll")]
        private static extern bool IsWindow(IntPtr hWnd);

        [DllImport("user32.dll")]
        private static extern bool IsWindowVisible(IntPtr hWnd);

        private const int SW_RESTORE = 9;
        private const byte VK_MENU = 0x12;
        private const uint KEYEVENTF_KEYUP = 0x0002;

        /// <summary>
        /// Checks whether the window exists and its handle is valid.
        /// </summary>
        public static bool IsWindowValid(IntPtr hWnd)
        {
            return hWnd != IntPtr.Zero && IsWindow(hWnd);
        }

        /// <summary>
        /// Checks the existence of a window by process PID.
        /// </summary>
        public static bool IsProcessWindowValid(int processId)
        {
            if (processId <= 0) return false;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    return !process.HasExited && IsWindowValid(process.MainWindowHandle);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Returns the Handle (IntPtr) of the currently active (focused) window.
        /// </summary>
        public static IntPtr GetForegroundWindowHandle()
        {
            return GetForegroundWindow();
        }

        /// <summary>
        /// Returns the process name by window Handle.
        /// </summary>
        public static string GetProcessNameByHandle(IntPtr hwnd)
        {
            if (!IsWindowValid(hwnd))
                return string.Empty;

            GetWindowThreadProcessId(hwnd, out uint processId);
            if (processId == 0)
                return string.Empty;

            try
            {
                using (Process process = Process.GetProcessById((int)processId))
                {
                    return process.ProcessName;
                }
            }
            catch (Exception)
            {
                return string.Empty;
            }
        }

        /// <summary>
        /// Returns the window title by its Handle.
        /// </summary>
        public static string GetWindowTitleByHandle(IntPtr hwnd)
        {
            if (!IsWindowValid(hwnd))
                return string.Empty;

            int length = GetWindowTextLength(hwnd);
            if (length == 0)
                return string.Empty;

            StringBuilder builder = new StringBuilder(length + 1);
            GetWindowText(hwnd, builder, builder.Capacity);

            return builder.ToString();
        }

        /// <summary>
        /// Focuses on an application by its PID.
        /// </summary>
        public static void FocusApplication(int processId)
        {
            Process targetProcess;

            try
            {
                targetProcess = Process.GetProcessById(processId);
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving process: {ex.Message}");
                return;
            }

            targetProcess.Refresh();
            IntPtr handle = targetProcess.MainWindowHandle;

            if (handle == IntPtr.Zero)
            {
                Console.WriteLine("Process does not have a main window.");
                return;
            }

            ForceForegroundWindow(handle);
        }

        public static void FocusApplication(IntPtr hWnd)
        {
            if (!IsWindowValid(hWnd))
            {
                Console.WriteLine("Invalid window Handle.");
                return;
            }

            ForceForegroundWindow(hWnd);
        }

        private static void ForceForegroundWindow(IntPtr hWnd)
        {
            if (IsIconic(hWnd))
            {
                ShowWindow(hWnd, SW_RESTORE);
            }

            IntPtr currentForegroundHWnd = GetForegroundWindow();
            if (currentForegroundHWnd == hWnd) return;

            uint currentThreadId = (uint)AppDomain.GetCurrentThreadId();
            uint foregroundThreadId = GetWindowThreadProcessId(currentForegroundHWnd, out _);

            bool attached = false;

            try
            {
                if (foregroundThreadId != 0 && foregroundThreadId != currentThreadId)
                {
                    attached = AttachThreadInput(currentThreadId, foregroundThreadId, true);
                }

                keybd_event(VK_MENU, 0, 0, UIntPtr.Zero);
                bool success = SetForegroundWindow(hWnd);
                keybd_event(VK_MENU, 0, KEYEVENTF_KEYUP, UIntPtr.Zero);

                Console.WriteLine(success ? "Focus successfully transferred." : "Failed to transfer focus.");
            }
            finally
            {
                if (attached)
                {
                    AttachThreadInput(currentThreadId, foregroundThreadId, false);
                }
            }
        }

        /// <summary>
        /// Checks whether a process with the specified Process ID exists and is currently running.
        /// </summary>
        public static bool IsProcessExists(int processId)
        {
            if (processId <= 0) return false;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    return !process.HasExited;
                }
            }
            catch (ArgumentException)
            {
                // Process with this PID does not exist or has already terminated
                return false;
            }
            catch (InvalidOperationException)
            {
                // Process is in the middle of terminating
                return false;
            }
        }

        /// <summary>
        /// Checks if the window associated with the specified handle is minimized.
        /// </summary>
        public static bool IsWindowMinimized(IntPtr hWnd)
        {
            if (!IsWindowValid(hWnd))
                return false;

            return IsIconic(hWnd);
        }

        /// <summary>
        /// Checks if the main window of the specified process is minimized.
        /// </summary>
        public static bool IsProcessMinimized(int processId)
        {
            if (processId <= 0) return false;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    if (process.HasExited) return false;
                    return IsWindowMinimized(process.MainWindowHandle);
                }
            }
            catch
            {
                return false;
            }
        }

        /// <summary>
        /// Checks if the window is currently focused. If not, sets focus to it.
        /// </summary>
        /// <param name="hWnd">Target window handle.</param>
        /// <returns>True if focus was requested/already set; false if handle is invalid.</returns>
        public static bool FocusIfNotActive(IntPtr hWnd)
        {
            if (!IsWindowValid(hWnd))
            {
                Console.WriteLine("Invalid window handle.");
                return false;
            }

            // Check if the target handle is already the foreground window
            if (GetForegroundWindowHandle() == hWnd)
            {
                Console.WriteLine("Window is already focused.");
                return true;
            }

            // Bring it to the front
            FocusApplication(hWnd);
            return true;
        }

        /// <summary>
        /// Checks if the main window of a process is currently focused. If not, sets focus to it.
        /// </summary>
        /// <param name="processId">Target process ID.</param>
        public static bool FocusIfNotActive(int processId)
        {
            if (processId <= 0) return false;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    if (process.HasExited) return false;

                    process.Refresh();
                    IntPtr handle = process.MainWindowHandle;

                    return FocusIfNotActive(handle);
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving process: {ex.Message}");
                return false;
            }
        }

        /// <summary>
        /// Checks if the specified window handle currently has input focus.
        /// </summary>
        public static bool IsWindowFocused(IntPtr hWnd)
        {
            return IsWindowValid(hWnd) && GetForegroundWindowHandle() == hWnd;
        }

        /// <summary>
        /// Checks if the main window of a process is currently focused.
        /// If it is not active (or minimized), it restores and brings it to the foreground.
        /// </summary>
        /// <param name="processId">The Process ID of the target application.</param>
        /// <returns>True if focus was successfully applied or already active; false otherwise.</returns>
        public static bool EnsureProcessFocused(int processId)
        {
            if (processId <= 0) return false;

            try
            {
                using (Process process = Process.GetProcessById(processId))
                {
                    if (process.HasExited) return false;

                    process.Refresh();
                    IntPtr handle = process.MainWindowHandle;

                    if (handle == IntPtr.Zero || !IsWindowValid(handle))
                    {
                        Console.WriteLine("Process does not have a valid main window.");
                        return false;
                    }

                    // If the window is already active and not minimized, do nothing
                    if (GetForegroundWindow() == handle && !IsIconic(handle))
                    {
                        return true;
                    }

                    // Focus and restore window
                    FocusApplication(handle);
                    return true;
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error retrieving process: {ex.Message}");
                return false;
            }
        }


    }
}