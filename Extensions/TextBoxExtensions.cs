using System;
using System.Runtime.InteropServices;
using System.Windows.Forms;

namespace Recorder.Extensions
{
    public static class TextBoxExtensions
    {
        private const int EM_SETCUEBANNER = 0x1501;

        [DllImport("user32.dll", CharSet = CharSet.Auto)]
        private static extern IntPtr SendMessage(IntPtr hWnd, int msg, int wParam, [MarshalAs(UnmanagedType.LPWStr)] string lParam);

        public static void SetPlaceholder(this TextBox textBox, string placeholderText)
        {
            // Force creation of the handle if it hasn't been created yet
            if (!textBox.IsHandleCreated)
            {
                textBox.HandleCreated += (s, e) =>
                {
                    SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholderText);
                };
                return;
            }

            // Handle already exists, send message immediately
            SendMessage(textBox.Handle, EM_SETCUEBANNER, 0, placeholderText);
        }
    }
}
