using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

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

        
    }
}
