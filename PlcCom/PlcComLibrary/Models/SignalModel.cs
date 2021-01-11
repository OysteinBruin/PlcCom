using System;
using System.Collections.Generic;
using System.Text;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary.Models
{
    public class SignalModel : ISignalModel
    {
        public SignalModel(int index )
        {
            Index = index;
        }
        public int Index { get; set; }
        public int Db { get; set; }
        public int DbByteIndex { get; set; }
        public int Bit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataTypeStr { get; set; }
        public DataType DataType { get; set; }// ref Enums.DatatType
        public string Address { get; set; }

        public double Value { get; set; }


        // TODO Consider refactoring SignalModel to derived types for each type (BoolSignalModel, Int32SignalModel etc) and change Value type to object.
        public double BytesToValue(byte[] bytes)
        {
            // TODO - Verify below code
            switch (DataType)
            {
                case DataType.Bit:
                    return ByteToBool(bytes);
                case DataType.Byte:
                    return bytes[0];
                case DataType.Word:
                    return (double)BytesToInt16(bytes);
                case DataType.DWord:
                    return (double)BytesToInt32(bytes);
                case DataType.Int:
                    return (double)BytesToInt16(bytes);
                case DataType.DInt:
                    return (double)BytesToInt32(bytes);
                case DataType.Real:
                    return BytesToFloat(bytes);
                case DataType.Array:
                    return 0.0;
            }

            return 0.0;
        }

        public int ByteCount()
        {
            // TODO - Verify below code
            switch (DataType)
            {
                case DataType.Bit:
                    return 1;
                case DataType.Byte:
                    return 1;
                case DataType.Word:
                    return 2;
                case DataType.DWord:
                    return 4;
                case DataType.Int:
                    return 2;
                case DataType.DInt:
                    return 4;
                case DataType.Real:
                    return 4;
                case DataType.Array:
                    return 0;
                default: 
                    return 0;
            }
        }

        public bool IsValid 
        { 
            get 
            {
                return (Db > 0 && Db < 10000 && Name.Length > 0 && Address.Length > 7);
            }
        }

        private Int32 BytesToInt32(byte[] bytes)
        {
            Int32 d = 0;

            if (bytes.Length == sizeof(Int32))
            {
                for (int i = 0; i < sizeof(Int32); ++i)
                {
                    d |= bytes[i] << (i * 8);
                }
            }

            return d;
        }

        private Int16 BytesToInt16(byte[] bytes)
        {
            Int32 d = 0;

            if (bytes.Length == sizeof(Int16))
            {
                for (int i = 0; i < sizeof(Int16); ++i)
                {
                    d |= bytes[i] << (i * 8);
                }
            }

            return (Int16)d;
        }

        private float BytesToFloat(byte[] bytes)
        {
            Int32 d = BytesToInt32(bytes);

            // TODO: To be verified
            return (float)d;
        }

        private double ByteToBool(byte[] boolByte)
        {
            if (boolByte?[0] == 0)
                return 0.0;

            return 1.0;
        }
    }
}
