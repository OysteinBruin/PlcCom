using PlcComLibrary.Common;
using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary
{
    public class FakePlcService : IPlcService
    {
        private ComState _comState;

        public FakePlcService(ICpuConfig config, List<IDatablock> datablocks)
        {
            Config = config;
            Datablocks = datablocks;
        }

        public string LastError { get; private set; }

        public ICpuConfig Config { get; private set; }

        public List<IDatablock> Datablocks { get; private set; }

        public event EventHandler ComStateChanged;
        public event EventHandler HasNewData;

        public async Task Connect(ICpuConfig config)
        {
            Config = config;
            await Connect();
        }

        public async Task Connect()
        {
            await DelayAsync(1000);
            ComState = ComState.Connected;
        }

        public void DisConnect()
        {
            ComState = ComState.DisConnected;
        }

        public async Task Write(string address, object value)
        {
            VerifyConnection();
            await DelayAsync(500);



        }

        

        public Task PulseBit(string address)
        {
            VerifyConnection();
            throw new NotImplementedException();
        }

        public Task<object> Read(string address)
        {
            VerifyConnection();
            throw new NotImplementedException();
        }

        public Task<IDatablock> ReadDb(IDatablock db)
        {
            VerifyConnection();
            throw new NotImplementedException();
        }

        public Task ToggleBit(string address)
        {
            VerifyConnection();
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

        private void VerifyConnection()
        {
            if (ComState != ComState.Connected)
            {
                throw new Exception("Com Error - Connect to CPU before attempting to read or write data.");
            }
        }
        private async Task DelayAsync(int ms)
        {
            await Task.Delay(ms);
        }
    }
}
