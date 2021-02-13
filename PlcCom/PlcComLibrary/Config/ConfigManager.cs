using PlcComLibrary.Common;
using PlcComLibrary.PlcCom;
using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using PlcComLibrary.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;

namespace PlcComLibrary.Config
{

    public class ConfigManager : IConfigManager
    {
        private IJsonConfigFileParser _configParser;
        private IDatablockParser _dbParser;

        public event EventHandler ConfigsLoaded;
        public event EventHandler ConfigsLoadingProgressChanged;

        public ConfigManager(IJsonConfigFileParser configParser, IDatablockParser dbParser)
        {
            _configParser = configParser;
            _dbParser = dbParser;

        }


        /* 
         
           Parse json files first, to find saved to json db's, those not found will be parsed here :
         - Parse 

        S7 datatypes:
        Bool  1 bool
        Byte  1 byte
        Int   2 Int16
        DInt  4 Int32
        Word  2 UInt16
        DWord 4 UInt32
        Real  4 float



        public class JsonFileConfig : IJsonFileConfig
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
        
        --  db file:

           STRUCT 
              NOT_USED_0000_0004 : Array[0..2] of Int;
              ZMS : Struct   // Begin ZMS struct
                 RN : Struct   // Begin RN struct
                    Lock_ACT : Bool;   // ZMS - RN - Lockout from all machines
                    Dsbl_DW_ACT : Bool;   // ZMS - RN- Zone Management to DW - Disabled
                    Dsbl_PH_ACT : Bool;   // ZMS - RN- Zone Management to PH - Disabled
                    Dsbl_MA_ACT : Bool;   // ZMS - RN- Zone Management to MA - Disabled
                    Dsbl_CW_ACT : Bool;   // ZMS - RN - Zone Management to CW - Disabled


        Defined in CPU json:

            SignalLists": [ "2:Test_Data.db","3064:ZmsCSA_HMI_CMD.db", "3071:ZMSRN_HMI_CMD.db", "3070:ZMSRN_HMI_STAT.db" ],
	        "DiscardKeywords":[ "NOT_USED", "Spare", "STRUCT", "Struct" ]
        
        Parse:
           this class, LoadConfigs:  signals = _dbParser.ParseDb(filePath, dbNumber, jsonConfig.DiscardKeywords);

        

        */

        public List<PlcService> LoadConfigs(string path = "")
        {
            var plcServiceList = new List<PlcService>();
            var signals = new List<SignalModel>();
            
            int totalFilesToLoadCount = 0;
            List<IJsonFileConfig> jsonConfigs = _configParser.LoadConfigFiles(path);

            foreach (var jsonConfig in jsonConfigs)
            {
                totalFilesToLoadCount += jsonConfig.SignalLists.Count;
            }

            var configsProgressEventArgs = new ConfigsProgressEventArgs();
            configsProgressEventArgs.ProgressTotal = totalFilesToLoadCount;
            ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);

            foreach (var config in jsonConfigs)
            {
                ICpuConfig cpuConfig = new CpuConfig(config);
                List<IDatablockModel> datablocks = new List<IDatablockModel>();

                foreach (var dbNumberDbNameString in config.SignalLists)
                {
                    IDatablockModel datablock = new DatablockModel();

                    // signal should contain db number and db name, format : "number:name" e.g "3201:DbName"
                    List<string> dbNumberDbName = dbNumberDbNameString.Split(':').ToList();
                    int dbNumber = 0;
                    bool isParsable = false;

                    if (dbNumberDbName.Count > 0)
                    {
                        isParsable = Int32.TryParse(dbNumberDbName.First(), out dbNumber);
                    }

                    if (dbNumberDbName.Count != 2 || !isParsable)
                    {
                        throw new FormatException("Invalid file format in Json config! SignalsList must use : as separator between db number and name.");
                    }

                    string filePath = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs + dbNumberDbName.Last();



                    signals = _dbParser.ParseDb(filePath, dbNumber, config.DiscardKeywords);

                    if (signals?.Count > 0)
                    {
                        datablock.Index = datablocks.Count;
                        datablock.Signals = signals;
                        datablock.Name = dbNumberDbName.Last();
                        datablock.Number = dbNumber;
                        datablock.FirstByte = signals.First().DbByteIndex();
                        datablock.ByteCount = signals.Last().DbByteIndex() - datablock.FirstByte;
                        datablocks.Add(datablock);
                    }
                    configsProgressEventArgs.ProgressInput += 1;
                    ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);
                    Thread.Sleep(250);
                }

                int plcIndex = plcServiceList.Count;
                PlcService plcService = PlcServiceFactory.Create(plcIndex, cpuConfig, datablocks);
                plcServiceList.Add(plcService);
            }
            Thread.Sleep(2500);
            ConfigsLoaded?.Invoke(this, new EventArgs());
            return plcServiceList;
        }
    }
}
