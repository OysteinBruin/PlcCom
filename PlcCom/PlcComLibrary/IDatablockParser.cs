using PlcComLibrary.Config;
using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using System.Collections.Generic;

namespace PlcComLibrary
{
    public interface IDatablockParser
    {
        List<SignalModel> ParseDb(string path, int dbNumber, List<string> discardKeywords);
        int FirstByte { get; }
        int ByteCount { get; }
    }
}