using Caliburn.Micro;
using Dragablz;
using PlcComLibrary.Models;
using PlcComLibrary.PlcCom;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
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


                plc.HasNewData += OnPlcHasNewData;
            }

            this.ConnectionsViewModel = new ConnectionsViewModel(events, cpuList);
            this.SignalSelectionViewModel = new SignalSelectionViewModel(events, cpuList);

            this.SignalSelectionViewModel.SignalSelected += OnSignalSelected;
            this.SignalSelectionViewModel.DatablockSelected += OnDatablockSelected;

            RealTimeGraphViewModel realTimeModel = new RealTimeGraphViewModel("Graph View");
            Items.Add(realTimeModel);
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

        private void OnDatablockSelected(object sender, EventArgs args)
        {
            var eventArgs = (DatablockSelectedEvent)args;
            DatablockDisplayModel dbModel = eventArgs.DatablockSelected;

            DatablockTabViewModel vm = new DatablockTabViewModel(_events, eventArgs.DatablockSelected.Signals, dbModel.IndexModel, dbModel.Name);

            Items.Add(vm);
            ActivateItem(Items.Last());
        }

        private void OnSignalSelected(object sender, EventArgs args)
        {
            var eventArgs = (SignalSelectedEvent)args;


        }

        private void OnPlcHasNewData(object sender, EventArgs args)
        {
            var eventArgs = (PlcReadResultEventArgs)args;
            _events.PublishOnUIThread(new PlcReadEvent(eventArgs));
        }

        public async void Handle(PlcUiCmdEvent message)
        {
            //Debug.Assert(message.CpuIndex < _configManager.PlcServiceList.Count);

            //if (_configManager.PlcServiceList[0].ComState != PlcComLibrary.Common.Enums.ComState.Connected)
            //  return;

            try
            {
                switch (message.CommandType)
                {
                    case PlcUiCmdEvent.CmdType.ButtonPulse:
                        await _plcComManager.PlcServiceList[message.CpuIndex].PulseBitAsync(message.Address);
                        break;
                    case PlcUiCmdEvent.CmdType.ButtonToggle:
                        await _plcComManager.PlcServiceList[message.CpuIndex].ToggleBitAsync(message.Address);
                        break;
                    case PlcUiCmdEvent.CmdType.Slider:
                        Debug.Assert(message.Value != null);
                        await _plcComManager.PlcServiceList[0].WriteSingleAsync(message.Address, message.Value);
                        break;
                    default:
                        break;
                }
            }
            catch (ArgumentOutOfRangeException ex)
            {
                log.Error(ex);
                _events.PublishOnUIThread(new MessageEvent($"Failed to write to Plc - " +
                    $"Cpu {_plcComManager.PlcServiceList[message.CpuIndex].Config.Name} ip: {_plcComManager.PlcServiceList[message.CpuIndex].Config.Ip}",
                    ex.Message, MessageEvent.Level.Warn));
            }
            catch (InvalidOperationException ex)
            {
                log.Error($"Failed to write to Plc - " +
                    $"Cpu index {message.CpuIndex} ", ex);
                _events.PublishOnUIThread(new MessageEvent($"Failed to write to Plc - " +
                    $"Cpu {_plcComManager.PlcServiceList[message.CpuIndex].Config.Name} ip: {_plcComManager.PlcServiceList[message.CpuIndex].Config.Ip}",
                    ex.Message, MessageEvent.Level.Warn));
            }
            catch (Exception ex)
            {
                log.Error(ex);
                _events.PublishOnUIThread(new MessageEvent($"Failed to write to Plc - " +
                    $"Cpu {_plcComManager.PlcServiceList[message.CpuIndex].Config.Name} ip: {_plcComManager.PlcServiceList[message.CpuIndex].Config.Ip}",
                    ex.Message, MessageEvent.Level.Warn));
            }

        }
    }
}
