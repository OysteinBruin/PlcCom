using PlcComLibrary.Config;
using System.Collections.Generic;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcComManager
    {
        IConfigManager ConfigManager { get; }
        List<PlcService> PlcServiceList { get; set; }

        void LoadConfigs();

        bool GetIsAnyServicesBusy();
    }
}