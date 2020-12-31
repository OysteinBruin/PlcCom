using Caliburn.Micro;
using Dragablz;
using PlcComLibrary.PlcCom;
using PlcComLibrary.Models;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;

namespace PlcComUI.ViewModels
{
    class TabablzViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<PlcUiCmdEvent>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(TabablzViewModel));
        private IEventAggregator _events;
        private IPlcComManager _plcComManager;
        private List<CpuDisplayModel> _cpuList;

        public TabablzViewModel(
            IEventAggregator events,
            IPlcComManager plcComManager,
            IInterTabClient interTabClient,
            IInterLayoutClient interLayoutClient)
        {
            _events = events;
            _plcComManager = plcComManager;
            InterTabClient = interTabClient;
            InterLayoutClient = interLayoutClient;
            PlcList = new List<CpuDisplayModel>();
            _events.Subscribe(this);

            _plcComManager.LoadConfigs();

            foreach (var plc in _plcComManager.PlcServiceList)
            {
                CpuDisplayModel cpuDisplayModel = new CpuDisplayModel(plc, _events);
                PlcList.Add(cpuDisplayModel);

                int count = 0;
                foreach (var datablock in plc.Datablocks)
                {
                    DatablockTabViewModel dbModel = new DatablockTabViewModel(_events, plc.Index, datablock.Index, datablock.Name + "[" + datablock.Number + "]");
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
                        await _plcComManager.PlcServiceList[message.CpuIndex].WriteSingleAsync(message.Address, true);
                        break;
                    case PlcUiCmdEvent.CmdType.ButtonToggle:
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
