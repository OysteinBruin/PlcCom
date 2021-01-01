using PlcComUI.Models;
using System;
using System.Collections.Generic;

namespace PlcComUI.EventModels
{
    public class DatablockSelectedEvent : EventArgs
    {
        public DatablockSelectedEvent(DatablockDisplayModel dbSelected)
        {
            DatablockSelected = dbSelected;
        }

        public DatablockDisplayModel DatablockSelected { get; }
    }
}
