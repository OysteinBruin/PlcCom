using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Config
{
    public class ConfigsProgressEventArgs : EventArgs
    {
        public int ProgressInput { get; set; }

        public int ProgressTotal { get; set; }
    }
}
