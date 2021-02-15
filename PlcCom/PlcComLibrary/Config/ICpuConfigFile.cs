using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface ICpuConfigFile
    {
        string Name { get; set; }
        string Ip { get; set; }
        int Rack { get; set; }
        int Slot { get; set; }
        string CpuType { get; set; }
        List<string> SignalLists { get; set; }
        List<string> DiscardKeywords { get; set; }
    }
}