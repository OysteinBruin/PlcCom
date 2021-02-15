using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.Models
{
    public class DatablockDisplayModel : INotifyPropertyChanged, IEquatable<DatablockDisplayModel>, IDatablockDisplayModel
    {
        public string Name { get; set; }
        public int Index { get; set; }
        public int CpuIndex { get; set; }
        
        public int Number { get; set; }
        
        public int FirstByte { get; set; }
        public int ByteCount { get; set; }
        
        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

        public bool IsValid
        {
            get
            {
                return Index >= 0 && CpuIndex >= 0 && Signals.Count > 0 &&
                    Name.Length > 0 && Number > 0 && Number < 60000;
            }
        }

        public string NumberStr
        {
            get => $"DB{Number}";
        }

        

        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public bool Equals(DatablockDisplayModel other)
        {
            if(other == null)
                return false;

            return this.Index.Equals(other.Index) &&
                   this.CpuIndex.Equals(other.CpuIndex) &&
                   this.Name == other.Name &&
                   this.Signals.Count == other.Signals.Count;
        }
    }
}
