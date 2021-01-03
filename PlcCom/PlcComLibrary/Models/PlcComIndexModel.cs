using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{
    public class PlcComIndexModel
    {
        public PlcComIndexModel(int cpuIndex, int dbIndex, int signalIndex)
        {
            CpuIndex = cpuIndex;
            DbIndex = dbIndex;
            SignalIndex = signalIndex;
        }

        public int CpuIndex { get; set; } = -1;
        public int DbIndex { get; set; } = -1;
        public int SignalIndex { get; set; } = -1;
    }
}
