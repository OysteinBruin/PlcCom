using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IJsonConfigFileParser
    {
        int GetConfigFilesCount(string path = "");
        List<IJsonFileConfig> LoadConfigFiles(string path = "");
    }
}