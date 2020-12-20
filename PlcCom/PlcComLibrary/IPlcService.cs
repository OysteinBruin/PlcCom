using PlcComLibrary.Common;
using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcComLibrary
{
    public interface IPlcService
    {
        Enums.ComState ComState { get; }
        string LastError { get; }

        ICpuConfig Config { get; }
        List<IDatablock> Datablocks { get; }

        event EventHandler ComStateChanged;
        event EventHandler HasNewData;

        Task Connect();
        Task Connect(ICpuConfig config);
        Task Write(string address, object value);
        Task ToggleBit(string address);
        Task PulseBit(string address);
        Task<object> Read(string address);
        Task<IDatablock> ReadDb(IDatablock db);
        void DisConnect();
    }
}