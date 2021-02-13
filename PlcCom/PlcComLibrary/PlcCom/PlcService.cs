using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using System.Timers;

namespace PlcComLibrary.PlcCom
{
    /* 
     https://stackoverflow.com/questions/29831066/public-event-in-abstract-class
     https://stackoverflow.com/questions/4890915/is-there-a-task-based-replacement-for-system-threading-timer
     https://stackoverflow.com/questions/12796148/system-threading-timer-in-c-sharp-it-seems-to-be-not-working-it-runs-very-fast
    */

    public abstract class PlcService
    {
        private Enums.ComState _comState;
        protected System.Threading.Timer _plcReadWriteTimer;
        protected int _interval = 20;
        

        protected PlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
        {
            Index = index;
            Config = config;
            Datablocks = datablocks;
            _plcReadWriteTimer = new System.Threading.Timer(PlcReadWriteCallback, null, Timeout.Infinite, Timeout.Infinite);
        }

        protected abstract void PlcReadWriteCallback(Object state);
        protected bool ReadWriteTimerIsRunning { get; set; }

        public int Index { get; set; }

        public Enums.ComState ComState
        {
            get
            {
                return _comState;
            }
            protected set
            {
                _comState = value;
                ComStateChanged?.Invoke(this, new EventArgs());
            }
        }

        public ICpuConfig Config { get; protected set; }

        public List<IDatablockModel> Datablocks { get; protected set; }

        public List<IDatablockModel> MonitoredDatablocks { get; private set; } = new List<IDatablockModel>();

        public event EventHandler ComStateChanged;

        protected void RaiseComStateChanged(EventArgs e)
        {
            ComStateChanged?.Invoke(this, e);
        }

        public event EventHandler HasNewData;

        protected void RaiseHasNewData(EventArgs e)
        {
            HasNewData?.Invoke(this, e);
        }

        public virtual async Task ConnectAsync(ICpuConfig config)
        {
            Config = config;
            await ConnectAsync();
        }
        public abstract Task ConnectAsync();

        public abstract void DisConnect();

        public abstract Task ReadSingleAsync(string address);


        /// <summary>
        /// Add or removes a datablock from continously reading data from the repective plc datablcok
        /// </summary>
        /// <param name="add"></param>
        /// <param name="dbModel"></param>
        public virtual void AddOrRemoveDb(bool add, IDatablockModel dbModel)
        {
            if (dbModel == null)
                return;
           
            if (add == true)
            {
                MonitoredDatablocks.Add(dbModel);
            }
            else 
            {
                MonitoredDatablocks.RemoveAll(x => x.Name == dbModel.Name && 
                                                    x.Index == dbModel.Index);
            }

            if (MonitoredDatablocks.Count > 0 && !ReadWriteTimerIsRunning)
            {
                ReadWriteTimerIsRunning = true;
                _plcReadWriteTimer.Change(_interval, Timeout.Infinite);
            }
            else if (MonitoredDatablocks.Count == 0)
            {
                ReadWriteTimerIsRunning = false;
                _plcReadWriteTimer.Change(Timeout.Infinite, Timeout.Infinite);
            }
        }

        protected abstract Task ReadDbAsync(IDatablockModel db);

        public abstract Task WriteSingleAsync(string address, object value);

        public abstract Task PulseBitAsync(string address);
        public abstract Task ToggleBitAsync(string address);


        protected async Task DelayAsync(int ms)
        {
            await Task.Delay(ms);
        }

        protected bool AddressIsBoolType(string address)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressBoolRegExp, RegexOptions.IgnoreCase);

            if (regex.IsMatch(address))
            {
                return true;
            }
            return false;
        }

        protected (int dbIndex, int signalIndex) GetIndexFromAddress(string address, List<IDatablockModel> datablocks)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(address))
            {
                return (-1, -1);
            }

            // Get db number from adddress
            int dbNumber;
            string modifiedAddress = address.Remove(0, 2); // Remove "db" 
            List<string> strList = modifiedAddress.Split('.').ToList();

            bool successfullyParsed = int.TryParse(strList[0], out dbNumber);
            if (successfullyParsed)
            {
                // Find the signal in Datablocks list
                for (int i = 0; i < datablocks.Count; i++)
                {
                    if (datablocks[i].Number == dbNumber)
                    {
                        for (int j = 0; j < datablocks[i].Signals.Count; j++)
                        {
                            if (datablocks[i].Signals[j].Address == address)
                            {
                                return (i, j);
                            }
                        }
                    }
                }
            }

            return (-1, -1);
        }

        protected void VerifyConnected()
        {
            if (ComState != Enums.ComState.Connected)
            {
                throw new InvalidOperationException("Com Error - Connect to CPU before attempting to read or write data.");
            }
        }

        protected bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks)
        {
            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                return true;
            }
            return false;
        }

        protected void VerifyConnectedAndValidateAddress(string address)
        {
            VerifyConnected();
            if (!VerifyPlcAddressStr(address, Datablocks))
            {
                throw new Exception("");
            }
        }
    }
}
