using PlcComLibrary.Common;
using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Tests.PlcCom
{
    class TestableCpuConfig : ICpuConfig
    {
        public string Name { get; set; }
        public Enums.S7CpuType CpuType { get; set; }
        public string Ip { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }

    }
}
