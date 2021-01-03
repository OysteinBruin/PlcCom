using PlcComLibrary.PlcCom;
using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IConfigManager
    {
        List<PlcService> LoadConfigs(string path = "");

        //event EventHandler ConfigsLoaded;
        //event EventHandler ConfigsLoadingProgressChanged;
    }
}