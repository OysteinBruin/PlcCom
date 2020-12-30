using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcService
    {
        int Index { get; set; }
        Enums.ComState ComState { get; }
        string LastError { get; }

        ICpuConfig Config { get; }
        List<IDatablockModel> Datablocks { get; }

        event EventHandler ComStateChanged;
        event EventHandler HasNewData;

        Task ConnectAsync();
        Task ConnectAsync(ICpuConfig config);
        Task WriteSingleAsync(string address, object value);
        Task ToggleBitAsync(string address);
        Task PulseBitAsync(string address);
        Task ReadSingleAsync(string address);
        Task ReadDbAsync(IDatablockModel db);
        void DisConnect();
    }
}