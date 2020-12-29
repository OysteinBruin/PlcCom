using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;
using log4net;

namespace PlcComLibrary
{
    public class SimulatedPlcService : IPlcService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private ComState _comState;

        public SimulatedPlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
        {
            Index = index;
            Config = config;
            Datablocks = datablocks;
        }

        public event EventHandler ComStateChanged;
        public event EventHandler HasNewData;
        public int Index { get; set; }
        public string LastError { get; private set; }

        public ICpuConfig Config { get; private set; }

        public List<IDatablockModel> Datablocks { get; private set; }

        public async Task ConnectAsync(ICpuConfig config)
        {
            Config = config;
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            ComState = ComState.Connecting;
            await DelayAsync(1000);
            ComState = ComState.Connected;
        }

        public void DisConnect()
        {
            ComState = ComState.DisConnected;
        }

        public async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnected();
            await DelayAsync(10);

            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = (double)value;
                Console.WriteLine($"write value: {Datablocks[dbIndex].Signals[signalIndex].Value}");
                log.Debug($"write value: {value}");
                PlcReadResultEventArgs args = new PlcReadResultEventArgs(this.Index, dbIndex, signalIndex, (double)value);
                HasNewData?.Invoke(this, args);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        

        public async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean address: {address}");
            }

            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                
                PlcReadResultEventArgs args = new PlcReadResultEventArgs(this.Index, dbIndex, signalIndex, 1.0f);
                await DelayAsync(100);
                Datablocks[dbIndex].Signals[signalIndex].Value = 1.0f;
                HasNewData?.Invoke(this, args);
                await DelayAsync(500);
                args.PlcSignalIndexList[0].Value = 0.0f;
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                HasNewData?.Invoke(this, args);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
            

        }

        public async Task ReadSingleAsync(string address)
        {
            await DelayAsync(500);
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                PlcReadResultEventArgs args = new PlcReadResultEventArgs(this.Index, dbIndex, signalIndex, 0.0f);
                HasNewData?.Invoke(this, args);
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        public Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();
            throw new NotImplementedException();
        }

        public Task ToggleBitAsync(string address)
        {
            VerifyConnected();
            throw new NotImplementedException();
        }

        

        public ComState ComState
        {
            get
            {
                return _comState;
            }
            private set
            {
                _comState = value;
                ComStateChanged?.Invoke(this, new EventArgs());
            }
        }
        private void VerifyConnectedAndValidateAddress(string address)
        {
            VerifyConnected();
            if (!VerifyPlcAddressStr(address, Datablocks))
            {
                throw new Exception("");
            }
        }

        private void VerifyConnected()
        {
            if (ComState != ComState.Connected)
            {
                throw new InvalidOperationException("Com Error - Connect to CPU before attempting to read or write data.");
            }
        }
        private async Task DelayAsync(int ms)
        {
            await Task.Delay(ms);
        }

        private bool AddressIsBoolType(string address)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressBoolRegExp, RegexOptions.IgnoreCase);

            if (regex.IsMatch(address))
            {
                return true;
            }
            return false;
        }

        private (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablockModel> datablocks)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(address))
            {
                return (-1, -1);
            }

            // Get db number from adddress
            int dbNumber;
            string modifiedAddress = address.Remove(0,2); // Remove "db" 
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

        private bool VerifyPlcAddressStr(string address, List<IDatablockModel> datablocks)
        {
            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
