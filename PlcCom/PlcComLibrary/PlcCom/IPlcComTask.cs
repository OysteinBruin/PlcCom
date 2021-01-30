using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcComTask
    {
        void Execute(S7.Net.Plc _plc);
        string Address { get; set; }
        double Value { get; set; }
    }
}
