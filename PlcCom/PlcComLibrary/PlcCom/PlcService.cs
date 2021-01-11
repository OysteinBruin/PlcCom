using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using System.Timers;

namespace PlcComLibrary.PlcCom
{
    /* https://stackoverflow.com/questions/29831066/public-event-in-abstract-class

     An often used pattern for this is something like the below (you'll see a lot of it 
    in the classes in the System.Windows.Forms namespace).

    public abstract class MyClass
    {
        public event EventHandler MyEvent;

        protected virtual void OnMyEvent(EventArgs e)
        {
            if (this.MyEvent != null)
            {
                this.MyEvent(this, e);
            }
        }
    }


    public sealed class MyOtherClass : MyClass
    {
        public int MyState { get; private set; }

        public void DoMyEvent(bool doSomething)
        {
            // Custom logic that does whatever you need to do
            if (doSomething)
            {
                OnMyEvent(EventArgs.Empty);
            }
        }

        protected override void OnMyEvent(EventArgs e)
        {
            // Do some custom logic, then call the base method
            this.MyState++;

            base.OnMyEvent(e);
        }
    }
     
     */
    public abstract class PlcService
    {
        private Enums.ComState _comState;

        protected PlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
        {
            Index = index;
            Config = config;
            Datablocks = datablocks;
        }

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

        public HashSet<IDatablockModel> MonitoredDatablocks { get; private set; } = new HashSet<IDatablockModel>();

        public virtual void StartMonitoringDb(IDatablockModel dbModel)
        {
            if (dbModel != null)
                MonitoredDatablocks.Add(dbModel);
        }

        public virtual bool StopMonitoringDb(IDatablockModel dbModel)
        {
            if (dbModel == null)
                return false;

            int numElementsAdded = MonitoredDatablocks.RemoveWhere(m => m.Name == dbModel.Name 
                            && m.Index == dbModel.Index && m.Number == dbModel.Number 
                            && m.Signals.Count == dbModel.Signals.Count );
            return numElementsAdded == 1;
        }

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

        public virtual async Task ReadDatablocksAsync()
        {
            while (MonitoredDatablocks.Count > 0)
            {
                foreach (var db in MonitoredDatablocks)
                {
                    await ReadDbAsync(db);
                }
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
