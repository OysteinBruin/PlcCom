using PlcComLibrary.Common;

namespace PlcComLibrary.Models
{
    public interface ISignalModel
    {
        int Index { get; set; }
        string Address { get; set; }
        int Db { get; set; }
        int DbByteIndex { get; set; }
        int Bit { get; set; }
        Enums.DataType DataType { get; set; }
        string DataTypeStr { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        double Value { get; set; }

        double BytesToValue(byte[] bytes);
        int ByteCount();

        bool IsValid { get; }
    }
}