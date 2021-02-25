//using System;
//using System.Collections.Generic;
//using System.Linq;
//using System.Text;
//using System.Threading.Tasks;

//namespace PlcComUI.EventModels
//{
//    public class PlcUiCmdEvent
//    {
//        public PlcUiCmdEvent(CmdType cmdType, int cpuIndex, string address, object value = null)
//        {
//            CommandType = cmdType;
//            CpuIndex = cpuIndex;
//            Address = address;
//            Value = value;
//        }
        
//        public enum CmdType
//        {
//            ButtonPulse,
//            ButtonToggle,
//            Slider
//        }

//        public CmdType CommandType { get; }
//        public int CpuIndex { get; }
//        public string Address { get; set; }

//        public object Value { get; }

        
//    }
//}
