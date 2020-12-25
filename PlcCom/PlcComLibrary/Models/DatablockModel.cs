using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary.Models
{
    public class DatablockModel : IDatablockModel
    {
        public DatablockModel(int index)
        {
            Index = index;
            //Config = new JsonFileConfig();
            Signals = new List<ISignalModel>();
        }
        public int Index { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        //public IJsonFileConfig Config { get; set; }

        public List<ISignalModel> Signals { get; set; }

        public int FirstByte { get; set; }
        public int ByteCount { get; set; }
        
    }
}
