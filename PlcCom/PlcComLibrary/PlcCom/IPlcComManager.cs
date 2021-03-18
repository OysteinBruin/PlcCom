using PlcComLibrary.Config;
using System;
using System.Collections.Generic;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcComManager
    {
        IConfigManager ConfigManager { get; }
        List<PlcService> PlcServiceList { get; set; }

        event EventHandler AboutToLoadConfigs;
        event EventHandler ConfigsLoaded;

        void LoadConfigs();
        bool GetIsAnyServicesBusy();
    }
}