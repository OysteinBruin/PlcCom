using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public static class PlcServiceFactory
    {
        public static PlcService Create(int index, ICpuConfig config, List<IDatablockModel> datablocks)
        {
            if (AssemblyEnvironment.IsDevelopment())
            {
                return new SimulatedPlcService(index, config, datablocks);
            }

            return new S7PlcService(index, config, datablocks);
        }
    } 
}
