using PlcComLibrary.Common;
using PlcComLibrary.PlcCom;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static PlcComLibrary.Common.Enums;
using System.Threading.Tasks;
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


        public List<PlcService> LoadConfigs(string path = "")
        {
            List<PlcService> plcServiceList = new List<PlcService>();
            List<ISignalModel> signals = new List<ISignalModel>();
            List<IDatablockModel> datablocks = new List<IDatablockModel>();

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
                datablocks.Clear();

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
                        datablock.FirstByte = signals.First().Byte;
                        datablock.ByteCount = signals.Last().Byte - datablock.FirstByte;
                        datablocks.Add(datablock);
                    }
                    configsProgressEventArgs.ProgressInput += 1;
                    ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);
                    Thread.Sleep(150);
                }

                int plcIndex = plcServiceList.Count;
                PlcService plcService = PlcServiceFactory.Create(plcIndex, cpuConfig, datablocks);
                plcServiceList.Add(plcService);


                //foreach (var plc in PlcServiceList)
                //{
                //    Console.WriteLine($"\n\n-----------------| Plc Index {plc.Index} Name {plc.Config.Name} Ip {plc.Config.Ip} |-------------------\n");
                //    foreach (var db in plc.Datablocks)
                //    {
                //        Console.WriteLine($"\n\tDatablock Index {db.Index} Name {db.Name} Numer {db.Number} FirstByte {db.FirstByte} ByteCount {db.ByteCount} Signal Count {db.Signals.Count} \n");
                //        foreach (var sig in db.Signals)
                //        {
                //            Console.WriteLine($"\n\t\tSignal Index {sig.Index} Name {sig.Name} Address {sig.Address} Byte {sig.Byte} Bit {sig.Bit} DB {sig.Db} DataType {sig.DataType} DataTypeStr {sig.DataTypeStr}");
                //        }
                //    }               
                //}
            }
            Thread.Sleep(2000);
            ConfigsLoaded?.Invoke(this, new EventArgs());
            return plcServiceList;
        }
    }
}
