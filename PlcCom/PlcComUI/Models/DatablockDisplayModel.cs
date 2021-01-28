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
        public DatablockDisplayModel(PlcComIndexModel indexModel)
        {
            IndexModel = indexModel;
            Index = indexModel.DbIndex;
        }

        public int Index { get; set; }
        public PlcComIndexModel IndexModel { get; set; }

        public string Name { get; set; }

        public int Number { get; set; }
        public string NumberStr
        {
            get => $"DB{Number}";
        }

        public int FirstByte { get; set; }
        public int ByteCount { get; set; }

        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();
        

        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public bool Equals(DatablockDisplayModel other)
        {
            if(other == null)
                return false;

            return this.IndexModel.Equals(other.IndexModel) &&
                   this.Name == other.Name &&
                   this.Signals.Count == other.Signals.Count;
        }
    }
}
