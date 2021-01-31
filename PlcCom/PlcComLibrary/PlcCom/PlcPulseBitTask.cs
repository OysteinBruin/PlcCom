using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public class PlcPulseBitTask : IPlcComTask
    {
        public PlcPulseBitTask(string address)
        {
            Address = address;
        }
        public string Address { get; set; }
        public double Value { get; set; }

        public async Task Execute(S7.Net.Plc plc)
        {
            Console.WriteLine("BEG PlcPulseBitTask");
            await plc.WriteAsync(Address, true);
            await Task.Delay(100);
            await plc.WriteAsync(Address, false);
            Console.WriteLine("END PlcPulseBitTask");
        }
    }
}
