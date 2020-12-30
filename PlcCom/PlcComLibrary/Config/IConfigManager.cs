using PlcComLibrary.PlcCom;
using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IConfigManager
    {
        List<IPlcService> LoadConfigs(string path = "");

        //event EventHandler ConfigsLoaded;
        //event EventHandler ConfigsLoadingProgressChanged;
    }
}