using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{

    public class PlcReadResultEventArgs : EventArgs
    {
        public PlcReadResultEventArgs(PlcComIndexModel indexModel, object value)
        {
            PlcIndexModel = indexModel;
            Value = value;

            IndexValueList.Clear();
            IndexValueList.Add(new PlcComIndexValueModel(indexModel.CpuIndex, indexModel.DbIndex, indexModel.SignalIndex, value));
        }

        public PlcReadResultEventArgs(List<PlcComIndexValueModel> models)
        {
            IndexValueList = models;
        }

        public PlcComIndexModel PlcIndexModel { get; private set; }

        public object Value  { get; set; }

        public List<PlcComIndexValueModel> IndexValueList { get; set; } = new List<PlcComIndexValueModel>();
    }
}
