using Caliburn.Micro;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class ConnectionsViewModel : Screen
    {
        private IEventAggregator _events;
        private List<CpuDisplayModel> _cpuList;


        public ConnectionsViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public List<CpuDisplayModel> CpuList
        {
            get => _cpuList;
            set
            {
                _cpuList = value;
                NotifyOfPropertyChange(() => CpuList);
            }
        }
    }
}
