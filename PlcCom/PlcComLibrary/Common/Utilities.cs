using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;

namespace PlcComLibrary.Common
{
    public class Utilities : IUtilities
    {
        public List<string> LoadAppConfigFiles()
        {
            string path = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs;
            /// TODOTODOTODOTODOTODOTODOTODO   TODO: Handle File exceptions
            FileInfo fi = new FileInfo(path);
            List<string> appFiles = new List<string>();

            if (!fi.Directory.Exists)
            {
                //TODO - Directory.CreateDirectory -  Maybe create the dirs
                return appFiles;
            }
            appFiles = Directory.GetFiles(path).ToList();
            return appFiles;
        }

        public void VerifyPlcAddressString(string address, List<IDatablock> datablocks)
        {

        }

        private ISignalModel ParseAddressString(string address)
        {
            SignalModel signal = new SignalModel();
            var regex = new Regex(Constants.SignalAddressBoolRegExp);

            MatchCollection matchCollection = regex.Matches(address);

            if (matchCollection.Count < 1)
            {
                throw new Exception("");
            }

            return signal;
        }


        /*
        db20
        string Address { get; set; }
        int Db { get; set; }
        int Byte { get; set; }
        int Bit { get; set; }
        Enums.DataType DataType { get; set; }
        string DataTypeStr { get; set; }
        string Description { get; set; }
        string Name { get; set; }
        double Value { get; set; }

        */


    }
}
