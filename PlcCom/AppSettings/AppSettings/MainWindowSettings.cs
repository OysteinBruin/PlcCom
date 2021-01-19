using System;

namespace Settings.AppSettings
{
    [Serializable]
    public class MainWindowSettings
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }
        public bool IsDarkMode { get; set; }
        public bool IsWindowStateMaximized { get; set; }

        public MainWindowSettings()
        {
        }
    }
}
