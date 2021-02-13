using PlcComLibrary.Config;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary.Models
{
    public class DatablockModel : IDatablockModel
    {
        public DatablockModel()
        {
            Index = -1;
            Signals = new List<SignalModel>();
            Name = String.Empty;
            Number = -1;
        }
        public DatablockModel(int index, List<SignalModel> signals, string name = "", int number = -1)
        {
            Index = index;
            Signals = signals;
            Name = name;
            Number = number;            
        }
        public int Index { get; set; }
        public string Name { get; set; }
        public int Number { get; set; }
        //public IJsonFileConfig Config { get; set; }

        public List<SignalModel> Signals { get; set; }

        public bool IsValid 
        {
            get
            {
                return Index >= 0 && Signals.Count > 0 && 
                    Name.Length > 0 && Number > 0 && Number < 60000;
            }
            
        }
        public int FirstByte { get; set; }
        public int ByteCount { get; set; }
        
    }
}
