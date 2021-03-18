using Caliburn.Micro;
using PlcComLibrary.Models;
using PlcComUI.Domain;
using PlcComUI.EventModels;
using System;
using System.Diagnostics;

namespace PlcComUI.Models
{
    public class NumericSignalModel : SignalDisplayModel
    {

        private (int lower, int upper) _range;
        private object _value;
        private int _rangeFrom;
        private int _rangeTo;
        

        private bool _isUsingFixedRange;

        public NumericSignalModel(IEventAggregator events)
            : base(events)
        {
            SliderCommand = new RelayCommand<object>(OnSliderCommand);

            RangeFrom = 0;
            RangeTo = 100;
        }

        public override object Value
        {
            get => _value;
            set
            {
                //_value = value;

                //if (!IsUsingFixedRange)
                //{
                //    int val = (int)value;
                //    if (val < RangeFrom)
                //    {
                //        RangeFrom -= 10;
                //    }
                //    else if (val > RangeTo)
                //    {
                //        RangeTo += 10;
                //    }
                //}
                //ValueStr = String.Format("{0:0.00}", _value);
                //OnPropertyChanged(nameof(Value));
            }
        }

        public int RangeFrom
        {
            get => _rangeFrom;
            set
            {
                _rangeFrom = value;
                OnPropertyChanged(nameof(RangeFrom));
            }
        }

        public int RangeTo
        {
            get => _rangeTo;
            set
            {
                _rangeTo = value;
                OnPropertyChanged(nameof(RangeTo));
            }
        }

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

        public bool IsUsingFixedRange
        {
            get => _isUsingFixedRange;
            set
            {
                _isUsingFixedRange = value;
                OnPropertyChanged(nameof(IsUsingFixedRange));
            }
        }

        public RelayCommand<object> SliderCommand { get; set; }

        private void OnSliderCommand(object parameter)
        {
            var paramArray = (object[])parameter;
            Debug.Assert(paramArray.Length == 3);
            var cpuIndex = (int)paramArray[0];
            var address = (string)paramArray[1];
            var value = (object)paramArray[2];
            _events.PublishOnUIThread(
                new SliderCmdEvent { 
                    CpuIndex = cpuIndex,
                    Address = address,
                    Value = value
            });
        }
    }
}
