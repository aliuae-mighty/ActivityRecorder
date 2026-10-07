using System;

namespace Recorder.Models
{
    public enum ActionType
    {
        KeyDown,
        KeyUp,
        MouseMove,
        MouseDown,
        MouseUp,
        AppSwitch
    }

    public class ActionRecord
    {
        public ActionType Type { get; set; }
        public long TimestampMs { get; set; }
        public int X { get; set; }
        public int Y { get; set; }
        public int KeyCode { get; set; }
        public string KeyName { get; set; }
        public string MouseButton { get; set; }
        public string ProcessName { get; set; }
        public string WindowTitle { get; set; }
        public IntPtr WindowHandle { get; set; }
    }
}