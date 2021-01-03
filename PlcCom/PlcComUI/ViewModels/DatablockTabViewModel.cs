using Caliburn.Micro;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using PlcComLibrary.Models;

namespace PlcComUI.ViewModels
{
	public class DatablockTabViewModel : Screen, IHandle<PlcReadEvent>
	{
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IEventAggregator _events;
        private List<SignalDisplayModel> _signals;
        private readonly PlcComIndexModel _indexModel;
        public DatablockTabViewModel(IEventAggregator events, List<SignalDisplayModel> signals, PlcComIndexModel indexModel, string header)
		{
            _events = events;
            Signals = signals;
            _indexModel = indexModel;
            DisplayName = header;
            _events.Subscribe(this);
		}

        public List<SignalDisplayModel> Signals
        {
            get => _signals;
            set 
            {
                _signals = value;
                NotifyOfPropertyChange(()=> Signals);
            }
        }

        public int Number { get; set; }
        public string NumberStr
        {
            get => $"DB{Number}";
        }

        public void Handle(PlcReadEvent message)
        {

                foreach (var item in message.Data.IndexValueList)
                {
                    if (item.CpuIndex == _indexModel.CpuIndex && item.DbIndex == _indexModel.DbIndex)
                    {
                        Signals[item.SignalIndex].Value = item.Value;
                    }
                }
        }
    }
}
