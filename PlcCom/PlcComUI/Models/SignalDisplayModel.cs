//using PlcComUI.EventModels;
using Caliburn.Micro;
using PlcComUI.Domain;
using PlcComUI.EventModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.Models
{
    public class SignalDisplayModel : INotifyPropertyChanged
    {
        private IEventAggregator _events;
        private (int lower, int upper) _range;
        private object _value;
        private bool _isBool;

        public SignalDisplayModel(IEventAggregator events)
        {
            _events = events;
            PulseCommand = new RelayCommand<object>(OnPulseCommand);
            ToggleCommand = new RelayCommand<object>(OnToggleCommand);
            SliderCommand = new RelayCommand<object>(OnSliderCommand);
        }

        public int CpuIndex { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string DataTypeStr { get; set; }

        private DataType _dataType;

        public DataType DataType
        {
            get => _dataType;
            set
            {
                _dataType = value;
                if (_dataType == DataType.Bit)
                {
                    IsBoolType = true;
                }
                else
                {
                    IsBoolType = false;
                }

                OnPropertyChanged(nameof(DataType));
                OnPropertyChanged(nameof(IsBoolType));
            }
        }// ref Enums.DatatType

        public string Address { get; set; }
        public int Db { get; set; }
        public int Byte { get; set; }
        public int Bit { get; set; }
        public int RangeFrom { get; set; }

        public int RangeTo { get; set; }

        public (int lower, int upper) Range
        {
            get => _range;
            set
            {
                _range = value;
                RangeFrom = _range.lower;
                RangeTo = _range.upper;
            }
        }

        public bool HasRange
        {
            get
            {
                return RangeTo - RangeFrom > 1.0f;
            }
        }

        public string Suffix { get; set; }

        public object Value
        {
            get => _value;
            set 
            {
                if (value != _value)
                {
                    _value = value;
                    OnPropertyChanged(nameof(Value));
                }
            }
        }
        
        public bool IsBoolType
        {
            get => _isBool;
            set
            {
                _isBool = value;
                OnPropertyChanged(nameof(IsBoolType));
            }
        }

        public bool CanActivatePulseCmd { get; private set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        public RelayCommand<object> PulseCommand { get; set; }
        public RelayCommand<object> ToggleCommand { get; set; }
        public RelayCommand<object> SliderCommand { get; set; }

        private void OnPulseCommand(object parameter)
        {
            var values = (object[])parameter;
            var cpuIndex = (int)values[0];
            var address = (string)values[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonPulse, cpuIndex, address));
        }

        private void OnToggleCommand(object parameter)
        {
            var values = (object[])parameter;
            var cpuIndex = (int)values[0];
            var address = (string)values[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonToggle, cpuIndex, address));
        }

        private void OnSliderCommand(object parameter)
        {
            var values = (object[])parameter;
            var cpuIndex = (int)values[0];
            var address = (string)values[1];
            var value = (object)values[2];
            Console.WriteLine($"SignalDisplayModel.OnSliderCommand plc index {cpuIndex} address {address} value {value}");
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.Slider, cpuIndex, address, value));
        }

    }
}
