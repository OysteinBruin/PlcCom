using Caliburn.Micro;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class ConnectionsViewModel : Screen
    {
        private IEventAggregator _events;
        
        public ConnectionsViewModel(IEventAggregator events, List<CpuDisplayModel> cpuList)
        {
            _events = events;
            CpuList = cpuList;
        }

        public List<CpuDisplayModel> CpuList;
    }
}
