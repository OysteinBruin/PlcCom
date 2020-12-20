using System;

namespace AppSettings
{
    [Serializable]
    public class MainWindowSettings
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }

        public MainWindowSettings()
        {
        }
    }
}
