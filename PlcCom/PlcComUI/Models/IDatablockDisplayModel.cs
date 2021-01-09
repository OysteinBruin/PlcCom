using PlcComLibrary.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlcComUI.Models
{
    public interface IDatablockDisplayModel
    {
        int Index { get; set; }
        PlcComIndexModel IndexModel { get; set; }
        string Name { get; set; }
        int Number { get; set; }
        string NumberStr { get; }
        List<SignalDisplayModel> Signals { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        void EmitPropertyChanged(string propName);
    }
}