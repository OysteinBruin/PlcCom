using Caliburn.Micro;
using PlcComLibrary.Common;
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

namespace PlcComUI.Models
{
    public class BoolSignalDisplayModel : SignalDisplayModel
    {
        private object _value;

        public BoolSignalDisplayModel(IEventAggregator events)
            : base(events)
        {
            PulseCommand = new RelayCommand<object>(OnPulseCommand);
            ToggleCommand = new RelayCommand<object>(OnToggleCommand);
        }


        public override object Value
        {
            get => _value;
            set
            {

                bool val = (bool)value;
                _value = val;

                ValueStr = ((val == true) ? "true" : "false");
                OnPropertyChanged(nameof(Value));
            }
        }

        public RelayCommand<object> PulseCommand { get; set; }
        public RelayCommand<object> ToggleCommand { get; set; }

        private void OnPulseCommand(object parameter)
        {
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 2);
            var cpuIndex = (int)paramArray[0];
            var address = (string)paramArray[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonPulse, cpuIndex, address));
        }

        private void OnToggleCommand(object parameter)
        {
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 2);
            var cpuIndex = (int)paramArray[0];
            var address = (string)paramArray[1];
            _events.PublishOnUIThread(new PlcUiCmdEvent(PlcUiCmdEvent.CmdType.ButtonToggle, cpuIndex, address));
        }
    }
}