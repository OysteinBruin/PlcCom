using PlcComUI.Models;
using System;
using System.Collections.Generic;

namespace PlcComUI.EventModels
{
    public class DbAddTotabEvent : EventArgs
    {
        public DbAddTotabEvent(DatablockDisplayModel dbSelected)
        {
            DatablockSelected = dbSelected;
        }

        public DatablockDisplayModel DatablockSelected { get; }
    }
}
