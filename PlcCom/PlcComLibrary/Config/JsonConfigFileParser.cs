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
        public List<IJsonFileConfig> LoadConfigFiles()
        {
            List<IJsonFileConfig> jsonConfigDataList = new List<IJsonFileConfig>();
            foreach (string fileName in _utils.LoadAppConfigFiles())
            {
                if (fileName.EndsWith(Constants.JsonExtension))
                {
                    using (StreamReader reader = new StreamReader(fileName))
                    {
                        string json = reader.ReadToEnd();
                        jsonConfigDataList.Add(JsonConvert.DeserializeObject<JsonFileConfig>(json));
                    }
                }
            }
            return jsonConfigDataList;
        }
    }
}
