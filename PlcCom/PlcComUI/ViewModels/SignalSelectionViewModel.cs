using Caliburn.Micro;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class SignalSelectionViewModel : Screen
    {
        private IEventAggregator _events;
        private List<CpuDisplayModel> _cpuList;

        public SignalSelectionViewModel(IEventAggregator events, List<CpuDisplayModel> cpuList)
        {
            _events = events;
            CpuList = cpuList;
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

        //public ObservableCollection<CpuDisplayModel> CpuList
        //{
        //    get { return _cpuList; }
        //    set
        //    {
        //        _test_cpuListSets = value;
        //        NotifyOfPropertyChange(() => CpuList); // TODO - is this notification call nesacassery
        //    }
        //}
    }
}
