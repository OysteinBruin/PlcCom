using System;
using System.Collections.Generic;
using System.Text;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary
{
    public class SignalModel : ISignalModel
    {
        public SignalModel()
        {
        }
        public int Db { get; set; }
        public int Byte { get; set; }
        public int Bit { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataTypeStr { get; set; }
        public DataType DataType { get; set; }// ref Enums.DatatType
        public string Address { get; set; }

        public double Value { get; set; }
        
    }
}
