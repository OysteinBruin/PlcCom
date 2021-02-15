using PlcComLibrary.Models;
using System.Collections.Generic;
using System.ComponentModel;

namespace PlcComUI.Models
{
    public interface IDatablockDisplayModel
    {
        string Name { get; set; }
        int Index { get; set; }
        int CpuIndex { get; set; }
        
        int Number { get; set; }
        
        int FirstByte { get; set; }
        int ByteCount { get; set; }
        List<SignalDisplayModel> Signals { get; set; }

        bool IsValid { get; }
        string NumberStr { get; }

        event PropertyChangedEventHandler PropertyChanged;

        void EmitPropertyChanged(string propName);
    }
}