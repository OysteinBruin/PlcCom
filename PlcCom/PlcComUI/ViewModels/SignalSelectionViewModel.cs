using Caliburn.Micro;
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
        public SignalSelectionViewModel(IEventAggregator events)
        {
            _events = events;
        }
    }
}
