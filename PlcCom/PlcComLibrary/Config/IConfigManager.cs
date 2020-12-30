using PlcComLibrary.PlcCom;
using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IConfigManager
    {
        List<IPlcService> PlcServiceList { get; set; }

        void LoadConfigs();

        //event EventHandler ConfigsLoaded;
        //event EventHandler ConfigsLoadingProgressChanged;
    }
}