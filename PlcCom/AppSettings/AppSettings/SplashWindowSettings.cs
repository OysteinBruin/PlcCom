using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Settings.AppSettings
{
    [Serializable]
    public class SplashWindowSettings
    {
        public int Height { get; set; }
        public int Width { get; set; }
        public int Left { get; set; }
        public int Top { get; set; }

        public SplashWindowSettings()
        {
        }
    }
}
