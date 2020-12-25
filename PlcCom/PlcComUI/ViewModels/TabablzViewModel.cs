using Caliburn.Micro;
using Dragablz;
using PlcComLibrary;
using PlcComLibrary.Models;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.ViewModels
{
    class TabablzViewModel : Conductor<IScreen>.Collection.OneActive, IHandle<PlcUiCmdEvent>
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

                int count = 0;
                foreach (var datablock in plc.Datablocks)
                {
                    TabContentViewModel dbModel = new TabContentViewModel(datablock.Name + "[" + datablock.Number + "]");
                    RealTimeGraphViewModel realTimeModel = new RealTimeGraphViewModel($"RealTimeGraph View {count+1}");

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

                    if (count%2 == 0)
                    {
                        Items.Add(dbModel);
                    }
                    else
                    {
                        Items.Add(realTimeModel);
                    }
                    count++;
                }
                _events.Subscribe(this);
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
            int plcIndex = eventArgs.PlcSignalIndexList[0].PlcIndex;
            int dbIndex = eventArgs.PlcSignalIndexList[0].DatablockIndex;
            int signalIndex = eventArgs.PlcSignalIndexList[0].DatablockIndex;
            Console.WriteLine($"TabablzViewModel.OnPlcHasNewData plc {plcIndex} db {dbIndex} " +
                $"sig {signalIndex} value {(double)_configManager.PlcServiceList[plcIndex].Datablocks[dbIndex].Signals[signalIndex].Value}");
            //var e = (CmdArgs)args;
            //switch (e.Cmd)
            //{
            //    case CmdArgs.CmdEnum.Retrieve:
            //        HandleTestDataRetrieved();
            //        break;
            //    case CmdArgs.CmdEnum.TestRunning:
            //        HandleTestRunning();
            //        break;
            //    case CmdArgs.CmdEnum.Cancel:
            //        UnitTestControlsViewModel.IsRunnningTest = false;
            //        updateTestSets(_s7PlcService.TestSets);
            //        break;
            //    default:
            //        break;
            //}
        }


        public void Handle(PlcUiCmdEvent message)
        {
            //Debug.Assert(message.CpuIndex < _configManager.PlcServiceList.Count);

            //if (_configManager.PlcServiceList[0].ComState != PlcComLibrary.Common.Enums.ComState.Connected)
              //  return;

            try
            {
                switch (message.CommandType)
                {
                    case PlcUiCmdEvent.CmdType.ButtonPulse:
                        _configManager.PlcServiceList[message.CpuIndex].WriteSingleAsync(message.Address, true);
                        break;
                    case PlcUiCmdEvent.CmdType.ButtonToggle:
                        break;
                    case PlcUiCmdEvent.CmdType.Slider:
                        Debug.Assert(message.Value != null);
                        _configManager.PlcServiceList[0].WriteSingleAsync(message.Address, message.Value);
                        break;
                    default:
                        break;
                }
            }
            //catch (S7Net.PlcException)
            //{

            //    throw;
            //}
            catch (Exception ex)
            {
                _events.PublishOnUIThread(new MessageEvent($"Failed to write to Plc - " +
                    $"Cpu {_configManager.PlcServiceList[message.CpuIndex].Config.Name} ip: {_configManager.PlcServiceList[message.CpuIndex].Config.Ip}",
                    ex.Message, MessageEvent.Level.Warn));
            }

        }
    }


}
