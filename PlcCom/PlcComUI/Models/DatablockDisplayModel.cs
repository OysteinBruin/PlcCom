﻿using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.Models
{
    public class DatablockDisplayModel : INotifyPropertyChanged
    {
        private string _name;
        public List<SignalDisplayModel> Signals { get; set; } = new List<SignalDisplayModel>();

        public int Number { get; set; }
        public string NumberStr
        {
            get => $"DB{Number}";
        }
        public string Name { get; set; }

        public event PropertyChangedEventHandler PropertyChanged;
        public void EmitPropertyChanged(string propName)
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propName));
        }
    }
}