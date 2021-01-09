using Caliburn.Micro;
using PlcComUI.EventModels;
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

        public SignalSelectionViewModel(IEventAggregator events)
        {
            _events = events;
        }

        public event EventHandler DatablockSelected;
        public event EventHandler SignalSelected;

        public List<CpuDisplayModel> CpuList
        {
            get => _cpuList;
            set
            {
                _cpuList = value;
                NotifyOfPropertyChange(() => CpuList);
            }
        }

        public void TreeViewItemDoubleClicked(object treeViewItemObj)
        {
            // TODO - fix bug: if treeViewItemObj is SignalDisplayModel, treeViewItemObj is also DatablockDisplayModel
            // Both events are triggered

            //if (treeViewItemObj.GetType() == typeof(DatablockDisplayModel) && treeViewItemObj.GetType() != typeof(SignalDisplayModel))
            //{
            //    DatablockSelected?.Invoke(this, new EventArgs());
            //}
            //else if (treeViewItemObj.GetType() == typeof(SignalDisplayModel))
            //{
            //    SignalSelected?.Invoke(this, new EventArgs());
            //}

            if (treeViewItemObj is DatablockDisplayModel ddm)
            {
                DatablockSelected?.Invoke(this, new DbAddTotabEvent(ddm));
            }
            else if (treeViewItemObj is SignalDisplayModel sdm)
            {
                SignalSelected?.Invoke(this, new SignalSelectedEvent(sdm));
            }
        }
    }
}
