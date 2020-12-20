using PlcComLibrary.Common;

namespace PlcComLibrary.Config
{
    public interface ICpuConfig
    {
        string Name { get; set; }
        Enums.S7CpuType CpuType { get; set; }
        string Ip { get; set; }
        int Rack { get; set; }
        int Slot { get; set; }
    }
}