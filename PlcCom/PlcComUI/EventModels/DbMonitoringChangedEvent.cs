using PlcComUI.Models;
using System;
using System.Collections.Generic;

namespace PlcComUI.EventModels
{
    public class DbMonitoringChangedEvent : EventArgs
    {
        public DbMonitoringChangedEvent(DatablockDisplayModel dbSelected, bool state)
        {
            Datablock = dbSelected;
            DoMonitor = state;
        }

        public DatablockDisplayModel Datablock { get; }
        public bool DoMonitor { get; set; }
    }
}
