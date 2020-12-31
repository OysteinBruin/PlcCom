using Caliburn.Micro;
using Dragablz;
using PlcComLibrary.Models;
using PlcComLibrary.PlcCom;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class PlcComViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<PlcUiCmdEvent>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlcComViewModel));
        private IEventAggregator _events;
        private IPlcComManager _plcComManager;

        public PlcComViewModel(IEventAggregator events,
            IPlcComManager plcComManager,
            IInterTabClient interTabClient,
            IInterLayoutClient interLayoutClient)
        {
            _events = events;
            _plcComManager = plcComManager;
            InterTabClient = interTabClient;
            InterLayoutClient = interLayoutClient;

            _events.Subscribe(this);
            _plcComManager.LoadConfigs();

            List<CpuDisplayModel> cpuList = new List<CpuDisplayModel>();

            foreach (var plc in _plcComManager.PlcServiceList)
            {
                CpuDisplayModel cpuDisplayModel = new CpuDisplayModel(plc, _events);
                cpuList.Add(cpuDisplayModel);

                int count = 0;
                foreach (var datablock in plc.Datablocks)
                {
                    TabContentViewModel dbModel = new TabContentViewModel(_events, plc.Index, datablock.Index, datablock.Name + "[" + datablock.Number + "]");
                    RealTimeGraphViewModel realTimeModel = new RealTimeGraphViewModel($"RealTimeGraph View {count + 1}");

                    List<SignalDisplayModel> signalDisplayModels = new List<SignalDisplayModel>();
                    foreach (var signal in datablock.Signals)
                    {
                        SignalDisplayModel sdm = new SignalDisplayModel(signal.Index, _events);
                        sdm.Name = signal.Name;
                        sdm.Address = signal.Address;
                        sdm.DataType = signal.DataType;
                        sdm.Value = signal.Value;
                        signalDisplayModels.Add(sdm);
                    }
                    dbModel.Signals = signalDisplayModels;

                    if (count % 2 == 0)
                    {
                        Items.Add(dbModel);
                    }
                    else
                    {
                        Items.Add(dbModel);
                        //Items.Add(realTimeModel);
                    }
                    count++;
                }
                plc.HasNewData += OnPlcHasNewData;
            }



            this.ConnectionsViewModel = new ConnectionsViewModel(events);
            this.SignalSelectionViewModel = new SignalSelectionViewModel(events);
        }

        public ConnectionsViewModel ConnectionsViewModel { get; set; }
        public SignalSelectionViewModel SignalSelectionViewModel { get; set; }

        protected override void OnActivate()
        {
            base.OnActivate();
            if (Items.Count > 0)
            {
                ActivateItem(Items[0]);
            }
        }

        public IInterTabClient InterTabClient { get; }
        public IInterLayoutClient InterLayoutClient { get; }

        private void OnPlcHasNewData(object sender, EventArgs args)
        {
            var eventArgs = (PlcReadResultEventArgs)args;
            _events.PublishOnUIThread(new PlcReadEvent(eventArgs));
        }

        public void Handle(PlcUiCmdEvent message)
        {
            throw new NotImplementedException();
        }
    }
}
