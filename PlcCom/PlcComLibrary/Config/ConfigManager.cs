using PlcComLibrary.Common;
using PlcComLibrary.PlcCom;
using PlcComLibrary.Models;
using PlcComLibrary.Models.Signal;
using PlcComLibrary.Factories;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using log4net;

namespace PlcComLibrary.Config
{

    public class ConfigManager : IConfigManager
    {
        private IJsonConfigFileParser _configParser;
        private IDatablockParser _dbParser;
        private static readonly ILog log = LogManager.GetLogger(typeof(ConfigManager));

        public event EventHandler ConfigsLoaded;
        public event EventHandler ConfigsLoadingProgressChanged;

        public ConfigManager(IJsonConfigFileParser configParser, IDatablockParser dbParser)
        {
            _configParser = configParser;
            _dbParser = dbParser;
        }

        public List<PlcService> LoadConfigs(string path = "")
        {
            log.Info($"LoadConfigs - path {path}");
            var plcServiceList = new List<PlcService>();
            var signals = new List<SignalModel>();
            
            int totalFilesToLoadCount = 0;
            List<ICpuConfigFile> cpuConfigFiles = _configParser.LoadConfigFiles(path);

            foreach (var jsonConfig in cpuConfigFiles)
            {
                totalFilesToLoadCount += jsonConfig.SignalLists.Count;
            }

            var configsProgressEventArgs = new ConfigsProgressEventArgs();
            configsProgressEventArgs.ProgressTotal = totalFilesToLoadCount;
            ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);

            foreach (var config in cpuConfigFiles)
            {
                ICpuConfig cpuConfig = new CpuConfig(config);
                List<DatablockModel> datablocks = new List<DatablockModel>();

                foreach (var dbNumberDbNameString in config.SignalLists)
                {
                    DatablockModel datablock = new DatablockModel();

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
                        string errorStr = "Invalid file format in Json config!SignalsList must use : as separator between db number and name.";
                        log.Error(errorStr);
                        throw new FormatException(errorStr);
                    }

                    string filePath = AppDomain.CurrentDomain.BaseDirectory + Constants.BaseDirectorySubDirs + dbNumberDbName.Last();
                    var signalContextList = _dbParser.ParseDb(filePath, dbNumber, config.DiscardKeywords);

                    for (int i = 0; i < signalContextList.Count; i++)
                    {
                        signalContextList[i].CpuIndex = plcServiceList.Count;
                        signalContextList[i].DbIndex = datablocks.Count;
                        signalContextList[i].Index = i;
                        signalContextList[i].DbNumber = dbNumber;
                        signals.Add(SignalFactory.Create(signalContextList[i]));
                    }

                    if (signals?.Count > 0)
                    {
                        datablock.CpuIndex = plcServiceList.Count;
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

                PlcService plcService = PlcServiceFactory.Create(plcServiceList.Count, cpuConfig, datablocks);
                plcServiceList.Add(plcService);
            }
            Thread.Sleep(2500);
            ConfigsLoaded?.Invoke(this, new EventArgs());
            return plcServiceList;
        }
    }
}
