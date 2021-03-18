using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;

namespace PlcComLibrary.Common
{
    public static class Constants
    {
        public static string BaseDirectorySubDirs { get => "/Configs/Signals/"; }
        public static string JsonExtension { get => ".json"; }
        public static string S7DbExtension { get => ".db"; }
        public static string S7AwlExtension { get => ".awl"; }
        public static string S7UdtExtension { get => ".udt"; }
        public static string S7DbArrayKeyword { get => "Array["; }
        public static string S7DbStructKeyword { get => "Struct"; }

        public static int LastBitInByte { get => 7; }

        public static string SignalAddressRegExp { get => @"^\bdb[1-9][0-9]{0,4}\b.\bdbx[0-9]{1,4}\b.[0-7]|\bdb[1-9][0-9]{0,4}\b.\bdb[bdw][0-9]{1,4}$"; }
        public static string SignalAddressBoolRegExp { get => @"^\bdb[0-9]{1,4}\b.\bdbx[0-9]{1,4}\b.[0-7]$"; }
        public static string SignalAddressNumericRegExp { get => @"^\bdb[0-9]{1,4}\b.\bdb[bdw][0-9]{1,4}$"; }

        // https://openautomationsoftware.com/knowledge-base/siemens-address-syntax/
        //public static readonly (string dataTypeStr, string specifier)[] S7DataTypeDataSpecifierLookup = {
        //    ("Bool",  "X"),
        //    ("Byte",  "B"),
        //    ("Int",   "W"),
        //    ("DInt",  "D"),
        //    ("Word",  "W"),
        //    ("DWord", "D"),
        //    ("Real",  "D") 
        //};

        // https://stackoverflow.com/questions/313324/declare-a-dictionary-inside-a-static-class
        public static readonly Dictionary<string, string> S7DataTypeSpecifiers
            = new Dictionary<string, string> {
            { "Bool",  "X" },
            { "Byte",  "B" },
            { "Int",   "W" },
            { "UInt",  "W" },
            { "DInt",  "D" },
            { "Word",  "W" },
            { "DWord", "D" },
            { "Real",  "D" }
        };

        public static readonly Dictionary<string, int> S7DataTypesByteSize
            = new Dictionary<string, int> {
            { "Bool",  1 },
            { "Byte",  1 },
            { "Int",   2 },
            { "UInt",  2 },
            { "DInt",  4 },
            { "Word",  2 },
            { "DWord", 4 },
            { "Real",  4 }
        };

        public static readonly IList<string> S7DataTypes = new List<string>() {
            "Bool",
            "Byte",
            "Int",
            "UInt",
            "DInt",
            "Word",
            "DWord",
            "Real"
        };
    }
}
