using Caliburn.Micro;
using PlcComLibrary.Models;
using PlcComLibrary.PlcCom;
using PlcComUI.Domain;
using PlcComUI.EventModels;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;
using static PlcComLibrary.Common.Enums;

namespace PlcComUI.Models
{
    public class CpuDisplayModel : INotifyPropertyChanged
    {
        private string _name;
        PlcService _plcService;
        private string _connectionName;
        private string _ipAddress;
        private int _rack;
        private int _slot;
        private S7CpuType _selectedCpuType;
        private IEventAggregator _events;
        private bool _isConnecting;
        private bool _isConnected;
        private System.Timers.Timer _connectButtonStateTimer;
        private string _connectButtonText = "CONNECT";

        public CpuDisplayModel(PlcService plcService, IEventAggregator events)
        {
            _plcService = plcService;
            _plcService.ComStateChanged += OnPlcComStateChanged;
            Index = _plcService.Index;
            _events = events;
            S7CpuType = Enum.GetValues(typeof(S7CpuType)).Cast<S7CpuType>().ToList();
            PlcConnectionCmd = new AsyncCommand<object>(OnPlcConnectionCmd);

            _connectButtonStateTimer = new System.Timers.Timer();
            _connectButtonStateTimer.Elapsed += onConnectButtonStateTimerElapsed;
            _connectButtonStateTimer.Interval = 50;

            Name = _plcService.Config.Name;
            IpAddress = _plcService.Config.Ip;
            Rack = _plcService.Config.Rack;
            Slot = _plcService.Config.Slot;
            SelectedCpuType = _plcService.Config.CpuType;

            foreach (var db in _plcService.Datablocks)
            {
                DatablockDisplayModel dbModel = new DatablockDisplayModel(new PlcComIndexModel(_plcService.Index,db.Index, -1));
                dbModel.Name = db.Name;
                dbModel.Number = db.Number;
                List<SignalDisplayModel> signalDisplayModels = new List<SignalDisplayModel>();
                foreach (var sig in db.Signals)
                {
                    //
                    SignalDisplayModel sdm = new SignalDisplayModel(new PlcComIndexModel(_plcService.Index, db.Index, sig.Index), _events);
                    sdm.Name = sig.Name;
                    sdm.Description = sig.Description;
                    sdm.Address = sig.Address;
                    sdm.DataType = sig.DataType;
                    sdm.DataTypeStr = sig.DataTypeStr;
                    sdm.Db = sig.Db;
                    sdm.Byte = sig.Byte;
                    sdm.Bit = sig.Bit;
                    sdm.Value = sig.Value;
                    signalDisplayModels.Add(sdm);
                }
                dbModel.Signals = signalDisplayModels;

                Datablocks.Add(dbModel);
            }
        }

        public List<DatablockDisplayModel> Datablocks { get; set; } = new List<DatablockDisplayModel>();
        public int Index { get; set; }
        public string Name
        {
            get => _connectionName;
            set
            {
                _connectionName = value;
                EmitPropertyChanged(nameof(Name));
            }
        }

        public string IpAddress
        {
            get { return _ipAddress; }
            set
            {
                _ipAddress = value;
                EmitPropertyChanged(nameof(IpAddress));
                EmitPropertyChanged(nameof(CanConnect));
            }
        }

        public int Rack
        {
            get { return _rack; }
            set
            {
                _rack = value;
                EmitPropertyChanged(nameof(Rack));
                EmitPropertyChanged(nameof(CanConnect));
            }
        }


        public int Slot
        {
            get { return _slot; }
            set
            {
                _slot = value;
                EmitPropertyChanged(nameof(Slot));
                EmitPropertyChanged(nameof(CanConnect));
            }
        }

        // https://stackoverflow.com/questions/47480725/caliburn-micro-enum-binding-in-combobox
        public S7CpuType SelectedCpuType
        {
            get { return _selectedCpuType; }
            set
            {
                EmitPropertyChanged(nameof(SelectedCpuType));
                EmitPropertyChanged(nameof(CanConnect));
            }
        }

        public IReadOnlyList<S7CpuType> S7CpuType { get; set; }

        public bool CanConnect
        {
            get
            {
                bool validForm = ValidateConnectForm();
                return validForm;
            }
        }

        public bool IsConnecting
        {
            get { return _isConnecting; }
            set
            {
                _isConnecting = value;
                EmitPropertyChanged(nameof(IsConnecting));
                EmitPropertyChanged(nameof(ConnectButtonText));
            }
        }

        public bool IsConnected
        {
            get { return _isConnected; }
            set
            {
                _isConnected = value;
                EmitPropertyChanged(nameof(IsConnected));
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public async Task Connect()
        {
            if (_plcService.ComState == ComState.DisConnected)
            {

                _plcService.Config.CpuType = SelectedCpuType;
                _plcService.Config.Ip = IpAddress;
                _plcService.Config.Rack = Rack;
                _plcService.Config.Slot = Slot;

                await _plcService.ConnectAsync();
            }
            else
            {
                _plcService.DisConnect();
            }
        }

        public string ConnectButtonText
        {
            get => _connectButtonText;
            set
            {
                _connectButtonText = value;
                EmitPropertyChanged(nameof(ConnectButtonText));
            }
        }

        public AsyncCommand<object> PlcConnectionCmd { get; set; }

        public Predicate<object> CanActivatePlcConnectionCmd { get; private set; }

        private async Task OnPlcConnectionCmd(object obj)
        {
            string connOrDisconnStr = "connect";

            try
            {


                if (_plcService.ComState == PlcComLibrary.Common.Enums.ComState.Connected || IsConnecting)
                {
                    connOrDisconnStr = "disconnect";
                    _plcService.DisConnect();
                }
                else
                {
                    await _plcService.ConnectAsync();
                }
            }
            //catch (S7Net.PlcException)
            //{

            //    throw;
            //}
            catch (Exception ex)
            {
                _events.PublishOnUIThread(new MessageEvent($"Failed to {connOrDisconnStr} Plc - " +
                    $"Cpu {_plcService.Config.Name} ip: {_plcService.Config.Ip}",
                    ex.Message, MessageEvent.Level.Warn));
            }
            //_events.PublishOnUIThread(new PlcConnectionCmdEvent(index));
        }


        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }

        private bool ValidateConnectForm()
        {
            if (!ValidateIp(IpAddress))
            {
                return false;
            }

            if (Rack >= 0 && Rack < 255 &&
                Slot >= 0 && Slot < 255 && S7CpuType != null)
            {
                return true;
            }

            //NotifyOfPropertyChange(() => CanConnect);
            return false;
        }

        private bool ValidateIp(string ipStr)
        {
            if (ipStr == null)
            {
                return false;
            }

            // https://stackoverflow.com/questions/4890789/regex-for-an-ip-address
            Regex ipRegex = new Regex(@"\b\d{1,3}\.\d{1,3}\.\d{1,3}\.\d{1,3}\b");
            MatchCollection result = ipRegex.Matches(ipStr);
            if (result.Count == 0)
            {
                return false;
            }

            return true;
        }

        private void OnPlcComStateChanged(object sender, EventArgs e)
        {
            switch (_plcService.ComState)
            {
                case ComState.DisConnected:
                    ConnectButtonText = "CONNECT";
                    IsConnecting = false;
                    break;
                case ComState.Connecting:
                    IsConnecting = true;
                    ConnectButtonText = "CONNECTING ...";
                    break;
                case ComState.ConnectFailed:
                    ConnectButtonText = "FAILED";
                    IsConnecting = false;
                    _connectButtonStateTimer.Interval = 2000;
                    _connectButtonStateTimer.Start();
                    break;
                case ComState.Connected:
                    ConnectButtonText = "CONNECTED";
                    IsConnecting = false;
                    // _events.PublishOnUIThread(new ConnectedEvent(ActiveProject.Name)); // Update Test View Name
                    _connectButtonStateTimer.Interval = 2000;
                    _connectButtonStateTimer.Start();
                    break;
                case ComState.LostConnection:
                    // TODO : Show lost connection dialog
                    ConnectButtonText = "LOST CONN!";
                    IsConnecting = false;
                    _connectButtonStateTimer.Interval = 2000;
                    _connectButtonStateTimer.Start();
                    break;
                default:
                    break;
            }
        }

        private void onConnectButtonStateTimerElapsed(object sender, ElapsedEventArgs e)
        {
            switch (_plcService.ComState)
            {
                case ComState.DisConnected:
                    ConnectButtonText = "CONNECT";
                    break;
                case ComState.Connecting:
                    break;
                case ComState.ConnectFailed:
                    _plcService.DisConnect();
                    _connectButtonStateTimer.Stop();
                    break;
                case ComState.Connected:
                    ConnectButtonText = "DISCONNECT";
                    _connectButtonStateTimer.Stop();

                    break;
                case ComState.LostConnection:
                    _plcService.DisConnect();
                    _connectButtonStateTimer.Stop();
                    break;
                default:
                    break;
            }
        }
    }
}
