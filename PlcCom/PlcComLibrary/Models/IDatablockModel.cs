using PlcComLibrary.Config;
using PlcComLibrary.Models.Signal;
using System.Collections.Generic;

namespace PlcComLibrary.Models
{
    public interface IDatablockModel
    {
        string Name { get; set; }
        int Index { get; set; }
        int Number { get; set; }

        int FirstByte { get; set; }
        int ByteCount { get; set; }

        List<SignalModel> Signals { get; set; }
    }
}