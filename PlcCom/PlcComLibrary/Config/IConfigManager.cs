using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace PlcComLibrary
{
    public interface IConfigManager
    {
        List<IPlcService> PlcServiceList { get; set; }

        void LoadConfigs();

        //event EventHandler ConfigsLoaded;
        //event EventHandler ConfigsLoadingProgressChanged;
    }
}