using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{
    public class PlcComIndexValueModel : PlcComIndexModel
    {
        public PlcComIndexValueModel(int cpuIndex, int dbIndex, int signalIndex, double value)  
            : base(cpuIndex, dbIndex, signalIndex)
        {
            Value = value;
        }

        public double Value { get; set; }
    }
}
