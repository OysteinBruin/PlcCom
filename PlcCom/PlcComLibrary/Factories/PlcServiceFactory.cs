using PlcComLibrary.Config;
using PlcComLibrary.Models;
using PlcComLibrary.PlcCom;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Factories
{
    public static class PlcServiceFactory
    {
        public static PlcService Create(int index, ICpuConfig config, List<DatablockModel> datablocks)
        {
            if (AssemblyEnvironment.IsDevelopment())
            {
                return new SimulatedPlcService(index, config, datablocks);
            }

            return new S7PlcService(index, config, datablocks);
        }
    } 
}
