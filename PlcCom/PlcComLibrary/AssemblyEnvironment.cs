using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary
{
    public class AssemblyEnvironment
    {
        private static readonly bool _isDevelopment = false;
        public static bool IsDevelopment()
        {
            return _isDevelopment;
        }
    }
}
