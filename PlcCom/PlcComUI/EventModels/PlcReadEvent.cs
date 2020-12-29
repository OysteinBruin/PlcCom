using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class PlcReadEvent
    {
        public PlcReadEvent(PlcReadResultEventArgs evenArgs)
        {
            Data = evenArgs;
        }

        public enum CmdType
        {
            ButtonPulse,
            ButtonToggle,
            Slider
        }

        public PlcReadResultEventArgs Data { get; }
    }
}
