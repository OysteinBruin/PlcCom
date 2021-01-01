using Caliburn.Micro;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;

namespace PlcComUI.ViewModels
{
	public class DatablockTabViewModel : Screen, IHandle<PlcReadEvent>
	{
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IEventAggregator _events;
        private List<SignalDisplayModel> _signals;
        private readonly int _plcIndex;
        private readonly int _dbIndex;
        public DatablockTabViewModel(IEventAggregator events, List<SignalDisplayModel> signals, int plcIndex, int dbIndex, string header)
		{
            _events = events;
            Signals = signals;
            _plcIndex = plcIndex;
            _dbIndex = dbIndex;
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
           
            Console.WriteLine($"message plc index {message.Data.PlcIndex} instance plc index {_plcIndex}loop:");
            if (message.Data.PlcIndex == _plcIndex)
            {
                foreach (var item in message.Data.PlcSignalIndexList)
                {
                    item.DatablockIndex = 0;
                    Console.WriteLine($"db index {item.DatablockIndex} signal index {item.SignalIndex} - instance db index {_dbIndex}");
                    if (item.DatablockIndex == _dbIndex)
                    {
                        Signals[item.SignalIndex].Value = item.Value;
                    }
                }
            }
        }
    }
}
