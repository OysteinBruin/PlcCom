using PlcComLibrary.Config;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public class PlcComManager : IPlcComManager
    {
        private IConfigManager _configManager;
        public PlcComManager(IConfigManager configManager)
        {
            _configManager = configManager;

        }

        public void LoadConfigs()
        {
            PlcServiceList = _configManager.LoadConfigs();
        }

        public List<IPlcService> PlcServiceList { get; set; } = new List<IPlcService>();
    }
}
