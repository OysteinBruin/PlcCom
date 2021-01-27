using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComUI.EventModels
{
    public class OpenGraphViewEvent : EventArgs
    {
        public OpenGraphViewEvent(GraphViewType viewType)
        {
            ViewType = viewType;
        }
            
        public GraphViewType ViewType { get; set; }

        public enum GraphViewType
        {
            SingleGraphs,
            MultiGraph
        }
    }
}
