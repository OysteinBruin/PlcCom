using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.EventModels
{
    public class ComStateChangedEvent
    {
        public ComStateChangedEvent(int cpuIndex, ComState comState)
        {
            CpuIndex = cpuIndex;
            ComState = comState;
        }

        public int CpuIndex { get; set; }
        public ComState ComState { get; set; }
    }
}
