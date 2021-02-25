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
            // numeric example DB100.DBD124         Target: 124
            // bool example    DB100.DBX240.1       Target: 240

            string[] subStrings =  model.Address.Split('.');

            if (subStrings.Length < 2)
            {
                return -1;
            }

            string indexStr = subStrings[1].Remove(0, 3);

            int output;
            bool parseOk = Int32.TryParse(indexStr, out output);

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
            // Length: Least possible length example: DB1.DBD0
            return model.Db > 0 && model.Name.Length > 0 && model.Address.Length >= 8;
        }
    }
}
