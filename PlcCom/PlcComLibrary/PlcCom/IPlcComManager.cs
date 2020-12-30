using System.Collections.Generic;

namespace PlcComLibrary.PlcCom
{
    public interface IPlcComManager
    {
        List<IPlcService> PlcServiceList { get; set; }

        void LoadConfigs();
    }
}