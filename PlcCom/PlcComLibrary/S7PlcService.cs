using PlcComLibrary.Config;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary
{
    public class S7PlcService : IPlcService
    {
        private S7.Net.Plc _plc;
        private ComState _comState;


        public event EventHandler HasNewData;
        public event EventHandler ComStateChanged;

        public S7PlcService(ICpuConfig config, List<IDatablock> datablocks)
        {
            Config = config;
            Datablocks = datablocks;
            //_plc = new S7.Net.Plc(_config.CpuType, _config.Ip, (short)_config.Rack, (short)_config.Slot);
        }

        public async Task Connect(ICpuConfig config)
        {
            Config = config;
            await Connect();
        }

        public async Task Connect()
        {
            ComState = ComState.Connecting;
            S7CpuType s7CpuType = Config.CpuType;

            await DelayAsync(1000);
            try
            {
                // Convert UnitTestLibrary.DataAccess.S7CpuType (data.S7CpuType) to S7.Net.CpuType to use
                // as input parameter for Plc instance
                bool result = false;
                result = Enum.TryParse(Enum.GetName(typeof(S7CpuType), s7CpuType), out S7.Net.CpuType S7NetCpuType);

                if (!result)
                {
                    throw new Exception($"Plc Connect Error - Invalid Cpu Type: {Enum.GetName(typeof(S7CpuType), s7CpuType)}");
                }

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

        

        public async Task Write(string address, object value)
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Plc Write Error - Connect to CPU before attempting to write.");
            }
            await _plc.WriteAsync(address, value);
        }

        public async Task ToggleBit(string address)
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Plc Write Error - Connect to CPU before attempting to write.");
            }
            var read = await _plc.ReadAsync(address);

            try
            {
                bool boolValue = (bool)read;
                if (boolValue)
                {
                    await _plc.WriteAsync(address, true);
                }
                else
                {
                    await _plc.WriteAsync(address, true);
                }
            }
            catch(InvalidCastException)
            {
                throw new Exception($"Plc Write Error - Cannot toggle a none boolean variable: {address}");
            }
            catch (Exception)
            {

                throw new Exception($"Plc Write Error - Unknown error occured while attempting to toggle: {address}");
            }
        }

        public async Task PulseBit(string address)
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Plc Write Error - Connect to CPU before attempting to write.");
            }
            await _plc.WriteAsync(address, true);
            await DelayAsync(100);
            await _plc.WriteAsync(address, false);
        }

        public async Task<object> Read(string address)
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Plc Read Error - Connect to CPU before attempting to read.");
            }
            return await _plc.ReadAsync(address);
        }

        public async Task<IDatablock> ReadDb(IDatablock db)
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Com Error - Connect to CPU before attempting to read.");
            }
            //byte[] bytes = new byte;
            byte[] bytes = await _plc.ReadBytesAsync(S7.Net.DataType.DataBlock, db.Number, db.FirstByte, db.ByteCount);

            Debug.Assert(bytes.Length == db.ByteCount - db.FirstByte);
            for (int i = 0; i < db.Signals.Count; i++)
            {

            }
            return db;
        }

        public string LastError { get; private set; }

        public ICpuConfig Config { get; private set; }

        public List<IDatablock> Datablocks { get; private set; }

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

        private async Task DelayRandomAsync(int minMs, int maxMs)
        {
            Random rnd = new Random();
            int delayMs = rnd.Next(minMs, maxMs);
            await Task.Delay(delayMs);
        }

        private async Task DelayAsync(int ms)
        {
            await Task.Delay(ms);
        }
    }
}
