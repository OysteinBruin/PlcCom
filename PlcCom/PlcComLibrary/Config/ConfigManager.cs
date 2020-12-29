using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.IO;
using static PlcComLibrary.Common.Enums;
using System.Threading.Tasks;

namespace PlcComLibrary
{
    public class ConfigManager : IConfigManager
    {
        private IJsonConfigFileParser _configParser;
        private IDatablockParser _dbParser;

        //public event EventHandler ConfigsLoaded;
        //public event EventHandler ConfigsLoadingProgressChanged;

        public ConfigManager(IJsonConfigFileParser configParser, IDatablockParser dbParser)
        {
            _configParser = configParser;
            _dbParser = dbParser;
            PlcServiceList = new List<IPlcService>();
            

        }

        public void LoadConfigs()
        {
            PlcServiceList.Clear();
            List<IDatablockModel> datablocks = new List<IDatablockModel>();
            ConfigsProgressEventArgs configsProgressEventArgs = new ConfigsProgressEventArgs();
            //ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);

            List<IJsonFileConfig> jsonConfigs = _configParser.LoadConfigFiles();


            foreach (var jsonConfig in jsonConfigs)
            {
                ICpuConfig cpuConfig = new CpuConfig(jsonConfig);
                


                foreach (var signalList in jsonConfig.SignalLists)
                {

                    // signal should contain db number and db name, format : "number:name" e.g "3201:DbName"
                    List<string> dbNumberDbName = signalList.Split(':').ToList();
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

                    List<ISignalModel> signals = _dbParser.ParseDb(filePath, dbNumber, jsonConfig.DiscardKeywords);
                    //Console.WriteLine($"\n\nAfter _dbParser.ParseDb filePath {filePath} signals: {signals.Count}");
                        foreach (var signal in signals)
                        {
                            //Console.WriteLine($"\tsignal {signal.Name} address: {signal.Address}");
                        }
                    //Console.WriteLine("\n\n");

                    if (signals.Count > 0)
                    {
                        int dbIndex = datablocks.Count;
                        IDatablockModel datablock = new DatablockModel(dbIndex);
                        datablock.Number = dbNumber;
                        datablock.Name = dbNumberDbName.Last();
                        datablock.Signals = signals;
                        datablock.FirstByte = signals.First().Byte;
                        datablock.ByteCount = signals.Last().Byte - datablock.FirstByte;
                        //Console.WriteLine($"\n\nAfter assignment: datablock.Signals = signals;  datablock.Signals,Count = {datablock.Signals.Count}");
                        //foreach (var signal in datablock.Signals)
                        //{
                        //    Console.WriteLine($"\tsignal {signal.Name} address: {signal.Address}");
                        //}
                        //Console.WriteLine("\n\n");

                        datablocks.Add(datablock);

                        //Console.WriteLine("\n\nAfter datablocks.Add(datablock);");
                        //foreach (var db in datablocks)
                        //{
                        //    Console.WriteLine($"\n\n\t{db.Name} signals: {db.Signals.Count}");
                        //    foreach (var signal in db.Signals)
                        //    {
                        //        Console.WriteLine($"\tsignal {signal.Name} address: {signal.Address}");
                        //    }
                        //}
                        //Console.WriteLine("\n\n");

                        //configsProgressEventArgs.ProgressInput += 1;
                        //ConfigsLoadingProgressChanged?.Invoke(this, configsProgressEventArgs);
                    }
                }

                int plcIndex = PlcServiceList.Count;
                PlcServiceList.Add(new SimulatedPlcService(plcIndex, cpuConfig, datablocks));

                //ConfigsLoaded?.Invoke(this, new EventArgs());
            }
        }

        public List<IPlcService> PlcServiceList { get; set; }
    }
}
