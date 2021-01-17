using PlcComUI.Models;
using System;
using System.Collections.Generic;

namespace PlcComUI.EventModels
{
    public class NewDatablockTabEvent : EventArgs
    {
        public NewDatablockTabEvent(DatablockDisplayModel dbSelected)
        {
            DatablockSelected = dbSelected;
        }

        public DatablockDisplayModel DatablockSelected { get; }
    }
}
