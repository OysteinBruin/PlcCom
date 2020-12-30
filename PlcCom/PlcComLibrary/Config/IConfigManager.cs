using PlcComLibrary.PlcCom;
using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IConfigManager
    {
        List<IPlcService> LoadConfigs();

        //event EventHandler ConfigsLoaded;
        //event EventHandler ConfigsLoadingProgressChanged;
    }
}