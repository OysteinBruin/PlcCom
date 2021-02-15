using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models.Signal
{
    public class SignalModelContext// : ISignalModelContext
    {
        public int CpuIndex { get; set; }
        public int DbIndex { get; set; }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int DbNumber { get; set; }
        public string DataTypeStr { get; set; }
        public int ByteIndex { get; set; }

        public int BitNumber { get; set; } = -1;
    }
}
