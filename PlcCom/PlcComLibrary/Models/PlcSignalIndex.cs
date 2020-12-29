using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{
    public class PlcSignalIndex
    {
        public PlcSignalIndex()
        { }
        public PlcSignalIndex(int plcIndex, int dbIndex, int signalIndex, double value)
        {
            PlcIndex = plcIndex;
            DatablockIndex = dbIndex;
            SignalIndex = signalIndex;
            Value = value;
        }
        public int PlcIndex { get; set; }
        public int DatablockIndex { get; set; }
        public int SignalIndex { get; set; }
        public double Value { get; set; }
    }
}
