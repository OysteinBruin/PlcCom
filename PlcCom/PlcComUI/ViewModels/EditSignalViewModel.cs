using Caliburn.Micro;
using PlcComUI.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.ViewModels
{
    public class EditSignalViewModel : Screen
    {
        private SignalDisplayModel _model;

        public EditSignalViewModel(SignalDisplayModel model)
        {
            _model = model;
        }

        

        public SignalDisplayModel Model
        {
            get { return _model; }
            set 
            { 
                _model = value;
                NotifyOfPropertyChange(() => Model);
            }
        }

    }
}
