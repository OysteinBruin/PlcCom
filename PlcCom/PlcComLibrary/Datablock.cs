using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary
{
    public class Datablock : IDatablock
    {
        public Datablock()
        {
            //Config = new JsonFileConfig();
            Signals = new List<ISignalModel>();
        }

        public string Name { get; set; }
        public int Number { get; set; }
        //public IJsonFileConfig Config { get; set; }

        public List<ISignalModel> Signals { get; set; }

        public int FirstByte { get; set; }
        public int ByteCount { get; set; }
    }
}
