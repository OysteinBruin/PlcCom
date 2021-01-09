//using PlcComUI.EventModels;
using Caliburn.Micro;
using PlcComLibrary.Models;
using PlcComUI.Domain;
using PlcComUI.EventModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.Models
{
    public class SignalDisplayModel : INotifyPropertyChanged, IEquatable<SignalDisplayModel>, ISignalDisplayModel
    {
        private IEventAggregator _events;
        private (int lower, int upper) _range;
        private double _value;
        private bool _isBool;

        public SignalDisplayModel(PlcComIndexModel indexModel, IEventAggregator events)
        {
            IndexModel = indexModel;
            Index = indexModel.SignalIndex;
            _events = events;
            PulseCommand = new RelayCommand<object>(OnPulseCommand);
            ToggleCommand = new RelayCommand<object>(OnToggleCommand);
            SliderCommand = new RelayCommand<object>(OnSliderCommand);
        }

        public PlcComIndexModel IndexModel { get; set; }
        public int Index { get; set; }
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
        public int RangeFrom { get; set; } = 0;

        public int RangeTo { get; set; } = 110;

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

        public double Value
        {
            get => _value;
            set
            {

                if (value != _value)
                {
                    _value = value;
                    ValueStr = String.Format("{0:0.00}", _value);
                    OnPropertyChanged(nameof(Value));
                }
            }
        }

        private string _valueStr;

        public string ValueStr
        {
            get => _valueStr;
            set
            {
                if (value != _valueStr)
                {
                    _valueStr = value;
                    OnPropertyChanged(nameof(ValueStr));
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
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 2);
            var indexModel = (PlcComIndexModel)paramArray[0];
            var address = (string)paramArray[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonPulse, indexModel.CpuIndex, address));
        }

        private void OnToggleCommand(object parameter)
        {
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 2);
            var indexModel = (PlcComIndexModel)paramArray[0];
            var address = (string)paramArray[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonToggle, indexModel.CpuIndex, address));
        }

        private void OnSliderCommand(object parameter)
        {
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 3);
            var indexModel = (PlcComIndexModel)paramArray[0];
            var address = (string)paramArray[1];
            var value = (object)paramArray[2];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.Slider, indexModel.CpuIndex, address, value));
        }

        public bool Equals(SignalDisplayModel other)
        {
            if (other == null)
                return false;

            return this.IndexModel.Equals(other.IndexModel) &&
                   this.Name == other.Name &&
                   this.Address == other.Address;
        }
    }
}
