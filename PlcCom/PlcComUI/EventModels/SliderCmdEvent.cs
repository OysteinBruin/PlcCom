using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class SliderCmdEvent : IControlCmdEvent
    {
        public int CpuIndex { get; set; }
        public string Address { get; set; }
        public object Value { get; set; }
    }
}
