using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{
    public class PlcSignalIndex
    {
        public PlcSignalIndex()
        {}
        public PlcSignalIndex(int plcIndex, int dbIndex, int signalIndex)
        {
            PlcIndex = plcIndex;
            DatablockIndex = dbIndex;
            SignalIndex = signalIndex;
        }
        public int PlcIndex { get; set; }
        public int DatablockIndex { get; set; }
        public int SignalIndex { get; set; }
    }

    public class PlcReadResultEventArgs : EventArgs
    {
        private readonly IList<PlcSignalIndex> _plcSignalIndexList;
        
        public PlcReadResultEventArgs(IList<PlcSignalIndex> plcSignalIndexList)
        {
            _plcSignalIndexList = plcSignalIndexList;
        }

        public PlcReadResultEventArgs(int plcIndex, int dbIndex, int signalIndex)
        {
            var psi = new PlcSignalIndex(plcIndex, dbIndex, signalIndex);
            _plcSignalIndexList = new List<PlcSignalIndex>();
            _plcSignalIndexList.Add(psi);
        }

        public IList<PlcSignalIndex> PlcSignalIndexList
        {
            get { return _plcSignalIndexList; }
        }
    }
}
