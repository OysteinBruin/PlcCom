﻿using Caliburn.Micro;
using PlcComUI.EventModels;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using log4net;
using PlcComLibrary.Models;
using System.Diagnostics;
using PlcComUI.Views;
using MaterialDesignThemes.Wpf;
using static PlcComLibrary.Common.Enums;
using System.Collections.ObjectModel;

namespace PlcComUI.ViewModels
{
	public class DatablockTabViewModel : Screen, IHandle<PlcReadEvent>, IHandle<ComStateChangedEvent>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IEventAggregator _events;
        private DatablockDisplayModel _datablockDisplayModel;

        private bool _isConnected;
        private bool _monitorCb;
        private bool _enableWriteCb;


        public DatablockTabViewModel(IEventAggregator events, DatablockDisplayModel displayModel, bool isConnected)
		{
            _events = events;
            _datablockDisplayModel = displayModel;
            Signals = new BindableCollection<SignalDisplayModel>(displayModel.Signals);
            DisplayName = displayModel.Name;
            _events.Subscribe(this);
            IsConnected = isConnected;
        }

        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public async void EditSignal(object model)
        {
            Debug.Assert(model is SignalDisplayModel);
            var signalDisplayModel = (NumericSignalModel)model;

            if (Signals.Contains(model))
            {
                var view = new EditSignalView();
                var viewModel = new EditSignalViewModel(signalDisplayModel);
                ViewModelBinder.Bind(viewModel, view, null);

                await DialogHost.Show(view, "MainDialogHost");

                if (viewModel.DoSave)
                {
                    int index = Signals.IndexOf(viewModel.Model);
                   if (index >= 0)
                   {
                        Signals[index] = viewModel.Model;
                   }
                }
            }

        }

        public BindableCollection<SignalDisplayModel> Signals { get; set; }

        public string Name
        {
            get => _datablockDisplayModel.Name;
            set
            {
                _datablockDisplayModel.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public int Number 
        {
            get => _datablockDisplayModel.Number;
            set
            {
                _datablockDisplayModel.Number = value;
                NotifyOfPropertyChange(() => Signals);
            }
        }

        public string NumberStr
        {
            get => _datablockDisplayModel.NumberStr;
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value.Equals(_isConnected)) return;
                _isConnected = value;

                if (value == false)
                {
                    MonitorCb = false;
                }

                NotifyOfPropertyChange(() => IsConnected);
            }
        }

        public bool MonitorCb
        {
            get => _monitorCb;
            set
            {
                if (value.Equals(_monitorCb))
                {
                    return;
                }

                _monitorCb = value;

                if (value == false && EnableWriteCb)
                {
                    EnableWriteCb = false;
                }

                _events.PublishOnUIThread(new DbMonitoringChangedEvent(_datablockDisplayModel, _monitorCb));
                NotifyOfPropertyChange(() => MonitorCb);
            }
        }

        public bool EnableWriteCb
        {
            get => _enableWriteCb;
            set
            {
                if (value.Equals(_enableWriteCb)) return;
                _enableWriteCb = value;

                NotifyOfPropertyChange(() => EnableWriteCb);
            }
        }

        public void Handle(PlcReadEvent message)
        {
            //Console.Write('\n');
            foreach (var item in message.Data.IndexValueList)
            {
                if (item.CpuIndex == _datablockDisplayModel.CpuIndex && item.DbIndex == _datablockDisplayModel.Index)
                {
                   // Console.Write($" | index {item.SignalIndex} value {item.Value}");

                    Signals[item.SignalIndex].Value = item.Value;
                }
            }
            //Console.Write($" | time sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
        }

        public void Handle(ComStateChangedEvent message)
        {
            if (message.CpuIndex == _datablockDisplayModel.CpuIndex )
            {
                if (message.ComState == ComState.Connected)
                {
                    IsConnected = true;
                }
                else
                {
                    IsConnected = false;
                }
            }
        }
    }
}
