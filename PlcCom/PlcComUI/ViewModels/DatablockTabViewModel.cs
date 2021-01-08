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


namespace PlcComUI.ViewModels
{
	public class DatablockTabViewModel : Screen, IHandle<PlcReadEvent>, IHandle<ComStateChangedEvent>
    {
        private static readonly log4net.ILog log = log4net.LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private IEventAggregator _events;
        private DatablockDisplayModel _displayModel;
        private bool _isConnected;
        private bool _monitorCb;
        private bool _enableWriteCb;
        private System.Windows.Forms.Timer _simTmer;
        List<double> _randomValueList = new List<double>();


        public DatablockTabViewModel(IEventAggregator events, DatablockDisplayModel displayModel, bool isConnected)
		{
            _events = events;
            _displayModel = displayModel;
            _events.Subscribe(this);
            IsConnected = isConnected;

            _simTmer = new System.Windows.Forms.Timer();
            _simTmer.Interval = 250;
        }

        public event EventHandler MonitoringStateChanged;

        protected override void OnViewLoaded(object view)
        {
            base.OnViewLoaded(view);

            // WriteEnableChecked = Properties.Settings.Default.SettingsMain. WriteEnableChecked;

            
        }

        protected override void OnActivate()
        {
            base.OnActivate();


            Random rnd = new Random();

            foreach (var item in Signals)
            {
                _randomValueList.Add(rnd.Next(1, 250));
            }


            _simTmer.Tick += (sender, e) => {

                for (int i = 0; i < Signals.Count; i++)
                {
                    if (i % 2 == 0 && Signals[i].DataType != PlcComLibrary.Common.Enums.DataType.Bit)
                    {
                        Signals[i].Value = _randomValueList[i] += 0.68f;

                        if (Signals[i].Value > Signals[i].RangeTo)
                        {
                            Signals[i].RangeTo = (int)Signals[i].Value + 25;
                        }

                        if (_randomValueList[i] > rnd.Next(150, 600))
                        {
                            _randomValueList[i] = 0;
                        }
                    }
                }
            };
            _simTmer.Start();
        }
        protected override void OnDeactivate(bool close)
        {
            base.OnDeactivate(close);
        }

        public async void EditSignal(object model)
        {
            Debug.Assert(model is SignalDisplayModel);
            var signalDisplayModel = (SignalDisplayModel)model;

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

        public List<SignalDisplayModel> Signals
        {
            get => _displayModel.Signals;
            set 
            {
                _displayModel.Signals = value;
                NotifyOfPropertyChange(()=> Signals);
            }
        }

        public string Name
        {
            get => _displayModel.Name;
            set
            {
                _displayModel.Name = value;
                NotifyOfPropertyChange(() => Name);
            }
        }

        public int Number 
        {
            get => _displayModel.Number;
            set
            {
                _displayModel.Number = value;
                NotifyOfPropertyChange(() => Signals);
            }
        }

        public string NumberStr
        {
            get => _displayModel.NumberStr;
        }

        public bool IsConnected
        {
            get => _isConnected;
            set
            {
                if (value.Equals(_isConnected)) return;
                _isConnected = value;

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
            foreach (var item in message.Data.IndexValueList)
            {
                if (item.CpuIndex == _displayModel.IndexModel.CpuIndex && item.DbIndex == _displayModel.IndexModel.DbIndex)
                {
                    Signals[item.SignalIndex].Value = item.Value;
                }
            }
        }

        public void Handle(ComStateChangedEvent message)
        {
            if (message.CpuIndex == _displayModel.IndexModel.CpuIndex )
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
