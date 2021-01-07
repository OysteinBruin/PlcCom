using Newtonsoft.Json;
using PlcComLibrary.Common;
using System;
using System.Collections.Generic;
using System.IO;
using System.Text;

namespace PlcComLibrary.Config
{
    public class JsonConfigFileParser : IJsonConfigFileParser
    {
        private IUtilities _utils;
        public JsonConfigFileParser(IUtilities utils)
        {
            _utils = utils;
        }

        public int GetConfigFilesCount(string path)
        {
            int output = 0;
            foreach (string fileName in _utils.LoadAppConfigFiles(path))
            {
                if (fileName.EndsWith(Constants.JsonExtension))
                {
                    output++;
                }
            }
            return output;
        }

        public List<IJsonFileConfig> LoadConfigFiles(string path)
        {
            List<IJsonFileConfig> output = new List<IJsonFileConfig>();
            foreach (string fileName in _utils.LoadAppConfigFiles(path))
            {
                if (fileName.EndsWith(Constants.JsonExtension))
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string json = reader.ReadToEnd();
                        output.Add(JsonConvert.DeserializeObject<JsonFileConfig>(json));
                    }
                }
            }
            return output;
        }
    }
}
