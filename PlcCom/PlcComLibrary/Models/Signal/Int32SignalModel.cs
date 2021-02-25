using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models.Signal
{
    public class Int32SignalModel : SignalModel
    {
        public Int32SignalModel()
        {

        }
        public Int32SignalModel(SignalModelContext ctx)
            : base(ctx)
        {
        }

        public override int ByteCount => 4;

        public override object BytesToValue(byte[] bytes)
        {
            if (bytes.Length != sizeof(Int32))
            {
                // TODO: throw exception and handle it
                return -1.0f;
            }
            byte tmp = bytes[0];
            bytes[0] = bytes[3];
            bytes[3] = tmp;
            tmp = bytes[1];
            bytes[1] = bytes[2];
            bytes[2] = tmp;

            return System.BitConverter.ToInt32(bytes, 0);
        }
    }
}
