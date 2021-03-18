using PlcComLibrary.Config;
using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using System.Collections.Generic;

namespace PlcComLibrary.DbParser
{
    public interface IDatablockParser
    {
        List<string> ReadS7DbFile(string path);
        List<SignalModelContext> ParseDb(List<string> fileLines, IList<string> discardKeywords);
    }
}