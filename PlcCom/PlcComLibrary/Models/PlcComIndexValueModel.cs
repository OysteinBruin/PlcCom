using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{
    public class PlcComIndexValueModel
    {
        public PlcComIndexValueModel(int cpuIndex, int dbIndex, int signalIndex, object value)  
        {
            CpuIndex = cpuIndex;
            DbIndex = dbIndex;
            SignalIndex = signalIndex;
            Value = value;
        }

        public int CpuIndex { get; set; }
        public int DbIndex { get; set; }
        public int SignalIndex { get; set; }
        public object Value { get; set; }
    }
}
