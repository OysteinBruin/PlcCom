﻿using Caliburn.Micro;
using MaterialDesignThemes.Wpf;
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
        private int _selectedLowerRange;
        private int _selectedUpperRange;

        public EditSignalViewModel(SignalDisplayModel model)
        {
            _model = model;

            SelectedLowerRange = model.RangeFrom;
            SelectedUpperRange = model.RangeTo;
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


        public int SelectedLowerRange
        {
            get => _selectedLowerRange;
            set
            {
                _selectedLowerRange = value;
                _model.RangeFrom = _selectedLowerRange;
                NotifyOfPropertyChange(() => SelectedLowerRange);
            }
        }

        public int SelectedUpperRange
        {
            get => _selectedUpperRange;
            set
            {
                _selectedUpperRange = value;
                _model.RangeTo = _selectedUpperRange;
                NotifyOfPropertyChange(() => SelectedUpperRange);
            }
        }

        public bool DoSave { get; set; }
        public void Ok()
        {
            DoSave = true;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

        public void Cancel()
        {
            DoSave = false;
            DialogHost.CloseDialogCommand.Execute(null, null);
        }

    }
}