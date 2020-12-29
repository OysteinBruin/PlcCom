using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{

    public class PlcReadResultEventArgs : EventArgs
    {
        private readonly IList<PlcSignalIndex> _plcSignalIndexList;
        
        public PlcReadResultEventArgs(int plcIndex,  IList<PlcSignalIndex> plcSignalIndexList)
        {
            PlcIndex = plcIndex;
            _plcSignalIndexList = plcSignalIndexList;
        }

        public PlcReadResultEventArgs(int plcIndex, int dbIndex, int signalIndex, double value)
        {
            PlcIndex = plcIndex;
            var psi = new PlcSignalIndex(dbIndex, signalIndex, value);
            _plcSignalIndexList = new List<PlcSignalIndex>();
            _plcSignalIndexList.Add(psi);
        }

        public int PlcIndex { get; }
        public IList<PlcSignalIndex> PlcSignalIndexList
        {
            get { return _plcSignalIndexList; }
        }
    }
}
