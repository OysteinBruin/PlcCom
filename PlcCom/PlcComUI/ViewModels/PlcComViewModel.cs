using AutoMapper;
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
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.ViewModels
{
    public class PlcComViewModel : Conductor<IScreen>.Collection.OneActive, 
                                   IHandle<PlcUiCmdEvent>, 
                                   IHandle<DbMonitoringChangedEvent>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(typeof(PlcComViewModel));
        private IEventAggregator _events;
        private IMapper _mapper;
        private IPlcComManager _plcComManager;

        public PlcComViewModel(IEventAggregator events,
            IPlcComManager plcComManager,
            IMapper mapper,
            IInterTabClient interTabClient,
            IInterLayoutClient interLayoutClient)
        {
            _events = events;
            _plcComManager = plcComManager;
            _mapper = mapper;
            InterTabClient = interTabClient;
            InterLayoutClient = interLayoutClient;

            this.ConnectionsViewModel = new ConnectionsViewModel(_events);
            this.SignalSelectionViewModel = new SignalSelectionViewModel(_events);

            this.SignalSelectionViewModel.SignalSelected += OnSignalSelected;
            this.SignalSelectionViewModel.DatablockSelected += OnDatablockSelected;
            this.SignalSelectionViewModel.GraphViewSelected += OnGraphViewSelected;

            _events.Subscribe(this);
        }

        protected override void OnInitialize()
        {
            base.OnInitialize();
            List<CpuDisplayModel> cpuList = new List<CpuDisplayModel>();

            foreach (var plc in _plcComManager.PlcServiceList)
            {
                CpuDisplayModel cpuDisplayModel = new CpuDisplayModel(plc, _events);
                cpuList.Add(cpuDisplayModel);

                plc.ComStateChanged += OnPlcComStateChanged;
                plc.HasNewData += OnPlcHasNewData;
            }

           

            this.ConnectionsViewModel.CpuList = cpuList;
            this.SignalSelectionViewModel.CpuList = cpuList;

            WelcomeTabViewModel welcomeModel = new WelcomeTabViewModel();
            Items.Add(welcomeModel);
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
            var eventArgs = (NewDatablockTabEvent)args;
            DatablockDisplayModel dbModel = eventArgs.DatablockSelected;

            bool isConnected = (_plcComManager.PlcServiceList[dbModel.IndexModel.CpuIndex].ComState == ComState.Connected);
            DatablockTabViewModel vm = new DatablockTabViewModel(_events, dbModel, isConnected);

            //DatablockTabView view = new DatablockTabView();
            //DatablockTabViewModel vm = new DatablockTabViewModel(_events, dbModel);
            //ViewModelBinder.Bind(vm, view, null);

            Items.Add(vm);
            ActivateItem(Items.Last());
        }

        private void OnSignalSelected(object sender, EventArgs args)
        {
            var eventArgs = (SignalSelectedEvent)args;


        }

        private void OnGraphViewSelected(object sender, EventArgs args)
        {
            var eventArgs = (OpenGraphViewEvent)args;
            if (eventArgs != null)
            {
                if (eventArgs.ViewType == OpenGraphViewEvent.GraphViewType.MultiGraph)
                {
                    var vm = new RealTimeGraphViewModel();
                    Items.Add(vm);
                }
                else
                {
                    var vm = new SingleGraphCollectionViewModel();
                    Items.Add(vm);
                }
            }
            
            ActivateItem(Items.Last());

        }

        private void OnPlcComStateChanged(object sender, EventArgs args)
        {
            Debug.Assert(sender is PlcService);
            PlcService plcService = (PlcService)sender;

           _events.PublishOnUIThread(new ComStateChangedEvent(plcService.Index, plcService.ComState));
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

        public void Handle(DbMonitoringChangedEvent message)
        {
            int cpuIndex = message.Datablock.IndexModel.CpuIndex;
            Debug.Assert(cpuIndex >= 0 && cpuIndex < _plcComManager.PlcServiceList.Count);
            var plc = _plcComManager.PlcServiceList[cpuIndex];

            try
            {
                DatablockDisplayModel ddm = message.Datablock;
                var mappedDbModel = _mapper.Map(ddm, ddm.GetType(), typeof(IDatablockModel));
                Debug.Assert(mappedDbModel is IDatablockModel);

                plc.AddOrRemoveDb(message.DoMonitor, mappedDbModel as IDatablockModel);
            }
            catch (AutoMapperMappingException ex)
            {
                string errorMsg = $"Failed to add {message.Datablock.Name} to the monitoring list for cpu {plc.Config.Name}";
                if (!message.DoMonitor)
                {
                    errorMsg = $"Failed to remove {message.Datablock.Name} from the monitoring list for cpu {plc.Config.Name}";
                }
                    
                _events.PublishOnUIThread(new MessageEvent(errorMsg, ex.Message, MessageEvent.Level.Warn));
            }
            catch (Exception ex)
            {
                _events.PublishOnUIThread(new MessageEvent("Unknown error occured while attempting to add " +
                    $"or remove datablock {message.Datablock.Name} to/from {plc.Config.Name}", ex.Message, MessageEvent.Level.Warn));
            }
        }
    }
}
