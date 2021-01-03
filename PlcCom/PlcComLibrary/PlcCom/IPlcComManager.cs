using System.Collections.Generic;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcComManager
    {
        List<PlcService> PlcServiceList { get; set; }

        void LoadConfigs();
    }
}