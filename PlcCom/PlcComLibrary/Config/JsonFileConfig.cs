using Newtonsoft.Json;
using System;
using System.Collections.Generic;
using System.Text;

namespace PlcComLibrary.Config
{
    public class JsonFileConfig : ICpuConfigFile
    {
        public string Name { get; set; }

        public string Ip { get; set; }
        public int Rack { get; set; }
        public int Slot { get; set; }

        public string CpuType { get; set; }
        [JsonProperty("SignalLists")]
        public List<string> SignalLists { get; set; }
        [JsonProperty("DiscardKeywords")]
        public List<string> DiscardKeywords { get; set; }
        
    }
}



