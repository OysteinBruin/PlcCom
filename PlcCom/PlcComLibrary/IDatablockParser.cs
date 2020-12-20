﻿using PlcComLibrary.Config;
using System.Collections.Generic;

namespace PlcComLibrary
{
    public interface IDatablockParser
    {
        List<ISignalModel> ParseDb(string path, int dbNumber, List<string> discardKeywords);
        int FirstByte { get; }
        int ByteCount { get; }
    }
}