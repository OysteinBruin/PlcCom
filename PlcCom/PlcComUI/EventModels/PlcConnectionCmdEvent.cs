using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class PlcConnectionCmdEvent
    {
        public PlcConnectionCmdEvent(int index)
        {
            Index = index;
        }

        public int Index { get; }
    }


}
