using PlcComLibrary.Common;
using PlcComLibrary.PlcCom;
using PlcComLibrary.Models;
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


        // Parse json files first, to find saved to json db's, those not found will be parsed here

        public List<PlcService> LoadConfigs(string path = "")
        {
            List<PlcService> plcServiceList = new List<PlcService>();
            List<ISignalModel> signals = new List<ISignalModel>();
            

            int totalFilesToLoadCount = 0;
            List<IJsonFileConfig> jsonConfigs = _configParser.LoadConfigFiles(path);

            foreach (var jsonConfig in jsonConfigs)
            {
                totalFilesToLoadCount += jsonConfig.SignalLists.Count;
            }

                ConfigsProgressEventArgs configsProgressEventArgs = new ConfigsProgressEventArgs();
            configsProgressEventArgs.ProgressTotal = totalFilesToLoadCount;
            ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);

            foreach (var jsonConfig in jsonConfigs)
            {
                ICpuConfig cpuConfig = new CpuConfig(jsonConfig);
                List<IDatablockModel> datablocks = new List<IDatablockModel>();

                foreach (var dbNumberDbNameString in jsonConfig.SignalLists)
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

                    signals = _dbParser.ParseDb(filePath, dbNumber, jsonConfig.DiscardKeywords);

                    if (signals?.Count > 0)
                    {
                        datablock.Index = datablocks.Count;
                        datablock.Signals = signals;
                        datablock.Name = dbNumberDbName.Last();
                        datablock.Number = dbNumber;
                        datablock.FirstByte = signals.First().DbByteIndex;
                        datablock.ByteCount = signals.Last().DbByteIndex - datablock.FirstByte;
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
