using PlcComLibrary.Common;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models.Signal
{
    public class BoolSignalModel : SignalModel
    {
        public BoolSignalModel(ISignalModelContext ctx)
            : base(ctx)
        {
            Bit = ctx.BitNumber;
        }

        public override int ByteCount => 1; 

        public override object BytesToValue(byte[] boolByte)
        {
            if (boolByte.Length != 1)
            {
                // TODO: throw exception and handle it
                return -1.0;
            }

            int mask = 1 << Bit;
            bool retVal = (boolByte[0] & mask) != 0;

            if (retVal == false)
                return false;

            return true;
        }

        private int Bit { get; }
    }
}
