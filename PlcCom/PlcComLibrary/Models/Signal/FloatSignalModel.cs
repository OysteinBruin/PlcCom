using System;
using System.Collections.Generic;
using System.Text;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary.Models.Signal
{
    public class FloatSignalModel : SignalModel
    {
        public FloatSignalModel()
        {

        }
        public FloatSignalModel(SignalModelContext ctx)
            : base(ctx)
        {
        }

        public override int ByteCount => 4;

        public override object BytesToValue(byte[] bytes)
        {
            if (bytes.Length != sizeof(float))
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

            return BitConverter.ToSingle(bytes, 0);
        }
    }
}
       
