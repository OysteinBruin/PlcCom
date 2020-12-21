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

        public static int LastBitInByte { get => 7; }
        public static int SignalAddressStrMinLength { get => 8; } // db1.dbd7
        public static string SignalAddressBoolRegExp { get => @"^\bdb[0-9]{1,4}\b.\bdbx[0-9]{1,4}\b.[0-7]$"; }
        public static string SignalAddressNumericRegExp { get => @"^\bdb[0-9]{1,4}\b.\bdb[bdw][0-9]{1,4}$"; }

        public static readonly (Enums.DataType dataType, string dataTypeStr, int byteSize)[] DataTypeLookup = {
            (Enums.DataType.Bit, "Bool", 0),
            (Enums.DataType.Byte, "Byte", 1),
            (Enums.DataType.Word, "Word", 2),
            (Enums.DataType.DWord, "DWord", 4),
            (Enums.DataType.Byte, "Int", 2),
            (Enums.DataType.Word, "DInt", 4),
            (Enums.DataType.Real, "Real", 4),
            (Enums.DataType.Array, S7DbArrayKeyword, 0)
        };
    }
}
