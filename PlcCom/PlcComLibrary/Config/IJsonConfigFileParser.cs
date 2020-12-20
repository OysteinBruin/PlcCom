using System.Collections.Generic;

namespace PlcComLibrary.Config
{
    public interface IJsonConfigFileParser
    {
        List<IJsonFileConfig> LoadConfigFiles();
    }
}