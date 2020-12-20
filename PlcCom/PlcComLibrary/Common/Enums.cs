using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary.Common
{
    public class Enums
    {
        public enum ComState
        {
            DisConnected,
            Connecting,
            ConnectFailed,
            Connected,
            LostConnection
        }

        public enum S7CpuType
        {
            S71500,
            S71200,
            S7400,
            S7300,
            S7200
        }

        /// <summary>
        /// Types
        /// </summary>
        public enum DataType
        {
            /// <summary>
            /// S7 Bit variable type (bool)
            /// </summary>
            Bit,

            /// <summary>
            /// S7 Byte variable type (8 bits)
            /// </summary>
            Byte,

            /// <summary>
            /// S7 Word variable type (16 bits, 2 bytes)
            /// </summary>
            Word,

            /// <summary>
            /// S7 DWord variable type (32 bits, 4 bytes)
            /// </summary>
            DWord,

            /// <summary>
            /// S7 Int variable type (16 bits, 2 bytes)
            /// </summary>
            Int,

            /// <summary>
            /// DInt variable type (32 bits, 4 bytes)
            /// </summary>
            DInt,

            /// <summary>
            /// Real variable type (32 bits, 4 bytes)
            /// </summary>
            Real,

            /// <summary>
            /// Array variable type (bytes = datatype bytes x size)
            /// </summary>
            Array
        }
    }
}
