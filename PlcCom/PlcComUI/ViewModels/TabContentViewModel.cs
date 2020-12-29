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
	public class TabContentViewModel : Screen, IHandle<PlcReadEvent>
	{
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IEventAggregator _events;
        private readonly int _plcIndex;
        private readonly int _dbIndex;
        public TabContentViewModel(IEventAggregator events, int plcIndex, int dbIndex, string header)
		{
            _events = events;
            _plcIndex = plcIndex;
            _dbIndex = dbIndex;
			DisplayName = header;
			ContentText = $"Datablock: {header}";
            _events.Subscribe(this);
		}

		public string ContentText { get; }

        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

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
