//using PlcComUI.EventModels;
using Caliburn.Micro;
using PlcComLibrary.Common;
using PlcComLibrary.Models;
using PlcComUI.Domain;
using System.ComponentModel;

namespace PlcComUI.Models
{
    public abstract class SignalDisplayModel
    {
        protected IEventAggregator _events;
        private string _valueStr;
        protected SignalDisplayModel(PlcComIndexModel indexModel, IEventAggregator events)
        {
            IndexModel = indexModel;
            Index = indexModel.SignalIndex;
            _events = events;
        }
        public int Index { get; set; }
        public string Name { get; set; }
        public string Description { get; set; }
        public string Address { get; set; }
        public int Db { get; set; }
        public abstract object Value { get; set; }
        public string DataTypeStr { get; set; }

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
        public PlcComIndexModel IndexModel { get; set; }

        /// <summary>
        /// 
        /// </summary>
        public string Suffix { get; set; }


        public bool Equals(SignalDisplayModel other)
        {
            if (other == null)
                return false;

            return this.IndexModel.Equals(other.IndexModel) &&
                   this.Name == other.Name &&
                   this.Address == other.Address;
        }

        public event PropertyChangedEventHandler PropertyChanged;
        public void OnPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}