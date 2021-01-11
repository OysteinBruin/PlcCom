//using PlcComUI.EventModels;
using PlcComLibrary.Common;
using PlcComLibrary.Models;
using PlcComUI.Domain;
using System.ComponentModel;

namespace PlcComUI.Models
{
    public interface ISignalDisplayModel
    {
        string Address { get; set; }
        int Bit { get; set; }
        int DbByteIndex { get; set; }
        bool CanActivatePulseCmd { get; }
        Enums.DataType DataType { get; set; }
        string DataTypeStr { get; set; }
        int Db { get; set; }
        string Description { get; set; }
        bool HasRange { get; }
        int Index { get; set; }
        PlcComIndexModel IndexModel { get; set; }
        bool IsBoolType { get; set; }
        string Name { get; set; }
        RelayCommand<object> PulseCommand { get; set; }
        (int lower, int upper) Range { get; set; }
        int RangeFrom { get; set; }
        int RangeTo { get; set; }
        RelayCommand<object> SliderCommand { get; set; }
        string Suffix { get; set; }
        RelayCommand<object> ToggleCommand { get; set; }
        double Value { get; set; }
        string ValueStr { get; set; }

        event PropertyChangedEventHandler PropertyChanged;

        bool Equals(SignalDisplayModel other);
        void OnPropertyChanged(string propName);
    }
}