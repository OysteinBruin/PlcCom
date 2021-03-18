using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace PlcComLibrary.Models.Signal
{
    public class Int16SignalModel : SignalModel
    {
        public Int16SignalModel()
        {

        }
        public Int16SignalModel(SignalModelContext ctx)
            : base(ctx)
        {
        }

        public override int ByteCount => 2;

        public override object BytesToValue(byte[] bytes)
        {
            if (bytes.Length != sizeof(Int16))
            {
                // TODO: throw exception and handle it
                return -1.0f;
            }
            byte tmp = bytes[0];
            bytes[0] = bytes[1];
            bytes[1] = tmp;
            

            return System.BitConverter.ToInt16(bytes, 0);
        }
    }
}
