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

        public IConfigManager ConfigManager  { get { return _configManager; } }

        public void LoadConfigs()
        {
            PlcServiceList = _configManager.LoadConfigs();
        }

        public bool GetIsAnyServicesBusy()
        {
            bool output = true;

            foreach (var plcService in PlcServiceList)
            {
                output = (plcService.MonitoredDatablocks.Count == 0);
            }
            return output;
        }

        public List<PlcService> PlcServiceList { get; set; } = new List<PlcService>();
    }
}
