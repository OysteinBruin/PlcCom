using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Common
{
    public static class SignalExtensions
    {
        /// <summary>
        /// Gets the value of the byte index in the belonging datablock
        /// </summary>
        /// <param name="model"></param>
        /// <returns>The byte index if parse found valid, else -1</returns>
        public static int DbByteIndex(this SignalModel model)
        {
            // numeric example DB100.DBD124
            // bool example    DB100.DBX240.1

            string[] subStrings =  model.Address.Split('.');
            subStrings[1].Remove(0, 3);

            int output;
            bool parseOk = Int32.TryParse(subStrings[1], out output);

            if (parseOk)
                return output;

            return -1;
        }

        /// <summary>
        /// Returns the result of required fields validation
        /// </summary>
        /// <param name="model"></param>
        /// <returns>true if required field are found valid, otherwise false.</returns>
        public static bool IsValid(this SignalModel model)
        {
            return model.Db > 0 && model.Name.Length > 0 && model.Address.Length > 7;
        }

        /*
        
        private Int16 BytesToInt16(byte[] bytes)
        {
            if (bytes.Length != 2)
            {
                // TODO: throw exception and handle it
                return -1;
            }
            byte tmp = bytes[0];
            bytes[0] = bytes[1];
            bytes[1] = tmp;

            return System.BitConverter.ToInt16(bytes, 0);
        }
        */
    }
}
