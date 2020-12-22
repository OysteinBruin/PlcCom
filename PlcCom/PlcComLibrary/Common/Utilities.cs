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

        public bool AddressIsBoolType(string address)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressBoolRegExp, RegexOptions.IgnoreCase);

            if (regex.IsMatch(address))
            {
                return true;
            }
            return false;
        }

        public (int dbIndex, int signalIndex) GetSignalIndexFromAddress(string address, List<IDatablock> datablocks)
        {
            // Validate address with regular expression
            var regex = new Regex(Constants.SignalAddressRegExp, RegexOptions.IgnoreCase);

            if (!regex.IsMatch(address))
            {
                return (-1, -1);
            }

            // Get db number from adddress
            int dbNumber;
            address = address.Remove(2); // Remove "db" 
            List<string> strList =  address.Split('.').ToList();

            bool successfullyParsed = int.TryParse(strList[0], out dbNumber);
            if (successfullyParsed)
            {
                // Find the signal in Datablocks list
                for (int i = 0; i < datablocks.Count; i++)
                {
                    if (datablocks[i].Number == dbNumber)
                    {
                        for (int j = 0; j < datablocks[i].Signals.Count; j++)
                        {
                            if (datablocks[i].Signals[j].Address == address)
                            {
                                return (i, j);
                            }
                        }
                    }
                }
            }

            return (-1, -1);
        }

        public bool VerifyPlcAddressStr(string address, List<IDatablock> datablocks)
        {
            (int dbIndex, int signalIndex) = GetSignalIndexFromAddress(address, datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                return true;
            }
            return false;
        }
    }
}
