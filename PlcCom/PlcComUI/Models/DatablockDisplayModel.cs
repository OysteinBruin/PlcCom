using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.Models
{
    public class DatablockDisplayModel : INotifyPropertyChanged, IDatablockDisplayModel
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

        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}
