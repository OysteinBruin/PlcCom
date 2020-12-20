using PlcComLibrary.Common;

namespace PlcComLibrary
{
    public interface ISignalModel
    {
        string Address { get; set; }
        int Db { get; set; }
        int Byte { get; set; }
        int Bit { get; set; }
        Enums.DataType DataType { get; set; }
        string DataTypeStr { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        double Value { get; set; }

    }
}