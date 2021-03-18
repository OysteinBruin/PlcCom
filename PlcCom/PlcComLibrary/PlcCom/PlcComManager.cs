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

        public event EventHandler AboutToLoadConfigs;
        public event EventHandler ConfigsLoaded;

        public void LoadConfigs()
        {
            AboutToLoadConfigs?.Invoke(this, new EventArgs());
            
            PlcServiceList = _configManager.LoadConfigs();

            ConfigsLoaded?.Invoke(this, new EventArgs());
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

        public void DisconnectAll()
        {
            foreach (var plcService in PlcServiceList)
            {
                plcService.DisConnect();
            }
        }

        public List<PlcService> PlcServiceList { get; set; } = new List<PlcService>();
    }
}
