using System;
using System.Collections.Generic;
using System.Text;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary.Config
{
    public class CpuConfig : ICpuConfig
    {
        public CpuConfig(ICpuConfigFile configFile)
        {
            Name = configFile.Name;
            Ip = configFile.Ip;
            Rack = configFile.Rack;
            Slot = configFile.Slot;
            // TODO: Implement a more bullet proof way of parsing S7CpuType
            CpuType = (S7CpuType)Enum.Parse(typeof(S7CpuType), configFile.CpuType);
        }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }
        public S7CpuType CpuType { get; set; }
    }
}
