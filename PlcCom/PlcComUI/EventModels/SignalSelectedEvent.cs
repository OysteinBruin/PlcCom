using PlcComUI.Models;
using System;
using System.Collections.Generic;

namespace PlcComUI.EventModels
{
    public class SignalSelectedEvent : EventArgs
    {
        public SignalSelectedEvent(SignalDisplayModel signalSelected)
        {
            SignalSelected = signalSelected;
        }

        public SignalDisplayModel SignalSelected { get; }
    }
}
