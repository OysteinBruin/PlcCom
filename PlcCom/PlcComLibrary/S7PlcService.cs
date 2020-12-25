using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary
{
    public class S7PlcService : IPlcService
    {
        private S7.Net.Plc _plc;
        private ComState _comState;

        public S7PlcService(int index, CpuConfig config, List<IDatablockModel> datablocks)
        {
            Index = index;
            Config = config;
            Datablocks = datablocks;

            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);

            _plc = new S7.Net.Plc(S7NetCpuType, config.Ip, (short)config.Rack, (short)config.Slot);
        }
        public int Index { get; set; }

        public event EventHandler HasNewData;
        public event EventHandler ComStateChanged;

        public async Task ConnectAsync(ICpuConfig config)
        {
            Config = config;
            await ConnectAsync();
        }

        public async Task ConnectAsync()
        {
            ComState = ComState.Connecting;

            await DelayAsync(1000);

            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);

            try
            {
                _plc = new Plc(S7NetCpuType, Config.Ip, (short)Config.Rack, (short)Config.Slot);
                await _plc.OpenAsync();

                if (_plc.IsConnected)
                {
                    ComState = ComState.Connected;
                }
                else
                {
                    ComState = ComState.ConnectFailed;
                }
            }
            catch (Exception ex)
            {
                LastError = ex.Message;
                ComState = ComState.ConnectFailed;
            }
        }

        public void DisConnect()
        {
            if (_plc != null && _comState != ComState.Connected)
            {
                _plc.Close();
                // _plcReadTimer.Stop();
            }
            ComState = ComState.DisConnected;
        }

        public async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnectedAndValidateAddress(address);

            await _plc.WriteAsync(address, value);
        }

        public async Task ToggleBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to toggle a non boolean address: {address}");
            }

            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                if (Datablocks[dbIndex].Signals[signalIndex].Value == 0)
                {
                    await _plc.WriteAsync(address, true);
                }
                else
                {
                    await _plc.WriteAsync(address, true);
                }
            }
            else
            {
                throw new Exception($"Plc Write Error - Unknown error occured while attempting to toggle: {address}");
            }
        }

        public async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean address: {address}");
            }

            await _plc.WriteAsync(address, true);
            await DelayAsync(100);
            await _plc.WriteAsync(address, false);
        }

        public async Task ReadSingleAsync(string address)
        {
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
               await _plc.ReadAsync(address);
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        public async Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();
            //byte[] bytes = new byte;
            byte[] bytes = await _plc.ReadBytesAsync(S7.Net.DataType.DataBlock, db.Number, db.FirstByte, db.ByteCount);

            Debug.Assert(bytes.Length == db.ByteCount - db.FirstByte);
            for (int i = 0; i < db.Signals.Count; i++)
            {

            }
        }

        public string LastError { get; private set; }

        public ICpuConfig Config { get; private set; }

        public List<IDatablockModel> Datablocks { get; private set; }

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
                throw new Exception("Com Error - Connect to CPU before attempting to read or write data.");
            }
        }


        private async Task DelayRandomAsync(int minMs, int maxMs)
        {
            Random rnd = new Random();
            int delayMs = rnd.Next(minMs, maxMs);
            await Task.Delay(delayMs);
        }

        private S7.Net.CpuType ConvertCpuType(S7CpuType s7CpuType)
        {
            // Convert UnitTestLibrary.DataAccess.S7CpuType (data.S7CpuType) to S7.Net.CpuType to use
            // as input parameter for Plc instance
            bool result = false;
            result = Enum.TryParse(Enum.GetName(typeof(S7CpuType), s7CpuType), out S7.Net.CpuType S7NetCpuType);

            if (!result)
            {
                throw new Exception($"Plc Connect Error - Invalid Cpu Type: {Enum.GetName(typeof(S7CpuType), s7CpuType)}");
            }

            return S7NetCpuType;
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
            address = address.Remove(2); // Remove "db" 
            List<string> strList = address.Split('.').ToList();

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
