using Caliburn.Micro;
using Dragablz;
using PlcComLibrary;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.ViewModels
{
    class TabablzViewModel : Conductor<IScreen>.Collection.OneActive
    {
        private IEventAggregator _events;
        private IConfigManager _configManager;
        private List<CpuDisplayModel> _cpuList;

        public TabablzViewModel(
            IEventAggregator events, 
            IConfigManager configManager,
            IInterTabClient interTabClient,
			IInterLayoutClient interLayoutClient)
		{
            _events = events;
            _configManager = configManager;
            InterTabClient = interTabClient;
			InterLayoutClient = interLayoutClient;
            PlcList = new List<CpuDisplayModel>();


                foreach (var plc in _configManager.PlcServiceList)
                {
                    CpuDisplayModel cpuDisplayModel = new CpuDisplayModel(plc, _events);
                    PlcList.Add(cpuDisplayModel);

                    //TEST
                    foreach (var datablock in plc.Datablocks)
                    {
                        TabContentViewModel dbModel = new TabContentViewModel(datablock.Name + "[" + datablock.Number + "]");

                        List<SignalDisplayModel> signalDisplayModels = new List<SignalDisplayModel>();
                        foreach (var signal in datablock.Signals)
                        {
                            SignalDisplayModel sdm = new SignalDisplayModel(_events);
                            sdm.Name = signal.Name;
                            sdm.Address = signal.Address;
                            sdm.DataType = signal.DataType;
                            signalDisplayModels.Add(sdm);
                        }
                        dbModel.Signals = signalDisplayModels;

       
                            Items.Add(dbModel);
                        
                        
                    }
                _events.Subscribe(this);
            }
        }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Items.Count > 0)
            {
                ActivateItem(Items[0]);
            }
            
        }

        public List<CpuDisplayModel> PlcList
        {
            get { return _cpuList; }
            set
            {
                _cpuList = value;
                NotifyOfPropertyChange(() => PlcList);
            }
        }

        public IInterTabClient InterTabClient { get; }
		public IInterLayoutClient InterLayoutClient { get; }
	}
}
