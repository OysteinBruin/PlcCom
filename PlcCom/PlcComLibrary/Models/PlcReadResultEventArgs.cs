using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models
{

    public class PlcReadResultEventArgs : EventArgs
    {
        public PlcReadResultEventArgs(PlcComIndexValueModel model)
        {  
            if (model == null)
                throw new ArgumentException("PlcComIndexValueModel can not be null"); 

            IndexValueList.Clear();
            IndexValueList.Add(model);
        }

        public PlcReadResultEventArgs(List<PlcComIndexValueModel> models)
        {
            IndexValueList = models;
        }

        public List<PlcComIndexValueModel> IndexValueList { get; set; } = new List<PlcComIndexValueModel>();
    }
}
