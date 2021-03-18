using PlcComLibrary.Config;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary.Models
{
    public class DatablockModel : IDatablockModel
    {
        public string Name { get; set; }
        public int Index { get; set; } = -1;
        public int CpuIndex { get; set; } = -1;

        public int Number { get; set; } = -1;
        public int FirstByte { get; set; }
        public int ByteCount { get; set; }

        public List<SignalModel> Signals { get; set; } = new List<SignalModel>();

        //public bool IsValid 
        //{
        //    get
        //    {
        //        return Index >= 0 && CpuIndex >= 0 && Signals.Count > 0 && 
        //            Name.Length > 0 && Number > 0 && Number < 60000;
        //    }
        //}
    }
}
