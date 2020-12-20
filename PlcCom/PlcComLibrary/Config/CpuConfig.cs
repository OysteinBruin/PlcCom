using System;
using System.Collections.Generic;
using System.Text;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary.Config
{
    public class CpuConfig : ICpuConfig
    {
        public CpuConfig(IJsonFileConfig dbConfig)
        {
            Name = dbConfig.Name;
            Ip = dbConfig.Ip;
            Rack = dbConfig.Rack;
            Slot = dbConfig.Slot;
            // TODO: Implement a more bullet proof way of parsing S7CpuType
            CpuType = (S7CpuType)Enum.Parse(typeof(S7CpuType), dbConfig.CpuType);
        }
        public string Name { get; set; }
        public string Ip { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }
        public S7CpuType CpuType { get; set; }
    }
}
