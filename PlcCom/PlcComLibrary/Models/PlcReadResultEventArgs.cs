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
        
        public PlcReadResultEventArgs(IList<PlcSignalIndex> plcSignalIndexList)
        {
            _plcSignalIndexList = plcSignalIndexList;
        }

        public PlcReadResultEventArgs(int plcIndex, int dbIndex, int signalIndex, double value)
        {
            var psi = new PlcSignalIndex(plcIndex, dbIndex, signalIndex, value);
            _plcSignalIndexList = new List<PlcSignalIndex>();
            _plcSignalIndexList.Add(psi);
        }

        public IList<PlcSignalIndex> PlcSignalIndexList
        {
            get { return _plcSignalIndexList; }
        }
    }
}
