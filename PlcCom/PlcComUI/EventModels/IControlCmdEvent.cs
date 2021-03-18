using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public interface IControlCmdEvent
    {
        int CpuIndex { get; set; }
        string Address { get; set; }
        object Value { get; set; }
    }
}
