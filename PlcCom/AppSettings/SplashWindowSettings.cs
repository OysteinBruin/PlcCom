using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AppSettings
{
    [Serializable]
    public class SplashWindowSettings
    {
        public int Left { get; set; }
        public int Top { get; set; }

        public SplashWindowSettings()
        {
        }
    }
}
