using PlcComLibrary.Config;
using System.Collections.Generic;

namespace PlcComLibrary
{
    public interface IDatablock
    {
        string Name { get; set; }
        int Number { get; set; }

        int FirstByte { get; set; }
        int ByteCount { get; set; }

        List<ISignalModel> Signals { get; set; }
    }
}