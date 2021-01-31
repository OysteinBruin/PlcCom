using S7.Net;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.PlcCom
{
    public class PlcToggleTask : IPlcComTask
    {
        public PlcToggleTask(string address, double value)
        {
            Address = address;
            Value = value;
        }
        public string Address { get; set; }
        public double Value { get; set; }

        public async Task Execute(Plc plc)
        {
            Console.WriteLine("BEG PlcToggleTask");
            if (Value == 0)
            {
                await plc.WriteAsync(Address, true);
            }
            else
            {
                await plc.WriteAsync(Address, false);
            }
            Console.WriteLine("END PlcToggleTask");
        }
    }
}
