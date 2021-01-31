using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using S7.Net;
using System;
using System.Collections.Generic;
using System.Diagnostics;
using System.Linq;
using System.Text.RegularExpressions;
using System.Threading;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;

namespace PlcComLibrary.PlcCom
{
    public class S7PlcService : PlcService
    {
        private S7.Net.Plc _plcReader;
        private S7.Net.Plc _plcWriter;
        private static readonly SemaphoreSlim _semaphoreSlim = new SemaphoreSlim(1, 3);
        private List<IPlcComTask> _collectedPlcTasks = new List<IPlcComTask>();
        private List<IPlcComTask> _collectedPlcTasksToExecute = new List<IPlcComTask>();


        public S7PlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
            : base(index, config, datablocks)
        {
            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);
            _plcReader = new S7.Net.Plc(S7NetCpuType, config.Ip, (short)config.Rack, (short)config.Slot);
            _plcWriter = new S7.Net.Plc(S7NetCpuType, config.Ip, (short)config.Rack, (short)config.Slot);
        }

        //public string LastError { get; private set; }
        public override async Task ConnectAsync()
        {
            ComState = ComState.Connecting;

            await DelayAsync(1000);

            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);

            try
            {
                string ip = "100.67.173.169";
                _plcReader = new Plc(S7NetCpuType, ip/*Config.Ip*/, (short)Config.Rack, (short)Config.Slot);
                _plcWriter = new Plc(S7NetCpuType, ip/*Config.Ip*/, (short)Config.Rack, (short)Config.Slot);
                await _plcReader.OpenAsync();
                await _plcWriter.OpenAsync();

                if (_plcReader.IsConnected && _plcWriter.IsConnected)
                {
                    ComState = ComState.Connected;
                }
                else
                {
                    ComState = ComState.ConnectFailed;
                    if (_plcReader.IsConnected)
                        _plcReader.Close();
                    if (_plcWriter.IsConnected)
                        _plcWriter.Close();
                }
            }
            catch (Exception)
            {
                //LastError = ex.Message;
                ComState = ComState.ConnectFailed;
            }
        }

        public override void DisConnect()
        {
            if (_plcReader != null && _plcWriter != null)
            {
                _plcReader.Close();
                _plcWriter.Close();
                // _plcReadTimer.Stop();
            }
            ComState = ComState.DisConnected;
            ComState = ComState.DisConnected;
        }

        protected async override void PlcReadWriteCallback(object state)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();
            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine("BEG PlcReadWriteCallback ");
            }
            

            //foreach (var task in _collectedPlcTasksToExecute)
            //{
            //    Console.WriteLine($"\t task.Execute address {task.Address}");
            //    await task.Execute(_plcReader);
            //}

            if (MonitoredDatablocks.Count > 0)
            {
                for (int i = MonitoredDatablocks.Count - 1; i >= 0; i--)
                {
                    await ReadDbAsync(MonitoredDatablocks[i]);
                }
            }

            
            _plcReadWriteTimer.Change(Math.Max(0, _interval - watch.ElapsedMilliseconds), Timeout.Infinite);
            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine($"END PlcReadWriteCallback elapsed ms {watch.ElapsedMilliseconds}");
            }
            //_collectedPlcTasksToExecute = _collectedPlcTasks;
            //_collectedPlcTasks.Clear();
        }

        public override async Task ReadSingleAsync(string address)
        {
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                await _semaphoreSlim.WaitAsync();
                await _plcReader.ReadAsync(address);
                _semaphoreSlim.Release();
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        protected override async Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();

            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine($"BEG ReadDbAsync sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
            }
            //await _semaphoreSlim.WaitAsync(5);
            byte[] dbBytes = await _plcReader.ReadBytesAsync(S7.Net.DataType.DataBlock, db.Number, db.FirstByte, db.ByteCount);

            // TODO change Debug.Assert to if and create scheduler to check if it fails several times, 
            // and throw exception + handling remove this db from read list
            //Debug.Assert(dbBytes.Length == db.ByteCount - db.FirstByte);
            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine($"\t sec bytes received {dbBytes.Length}");
            }
            List<PlcComIndexValueModel> indexValueModels = new List<PlcComIndexValueModel>();
            for (int i = 0; i < db.Signals.Count; i++)
            {
                ISignalModel s = db.Signals[i];
                int signalByteCount = s.ByteCount();
                byte[] dbBytesRange = dbBytes.Skip(s.DbByteIndex - db.FirstByte).Take(s.ByteCount()).ToArray();
                indexValueModels.Add(new PlcComIndexValueModel(Index, db.Index, s.Index, s.BytesToValue(dbBytesRange)));
                if (_collectedPlcTasksToExecute.Count > 0)
                {
                    Console.WriteLine($"/t received value {s.BytesToValue(dbBytesRange)}");
                }
            }
            PlcReadResultEventArgs args = new PlcReadResultEventArgs(indexValueModels);

            // foreach (var item in args.IndexValueList)
            // {
            //     Console.WriteLine($"");
            // }
            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine($"ReadDbAsync {db.Name} sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
            }

            RaiseHasNewData(args);
            //_semaphoreSlim.Release();
            if (_collectedPlcTasksToExecute.Count > 0)
            {
                Console.WriteLine($"END ReadDbAsync sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
            }
        }


        public override async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnectedAndValidateAddress(address);
            await _semaphoreSlim.WaitAsync();
            await _plcWriter.WriteAsync(address, value);
            _semaphoreSlim.Release();
        }

        public override async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean address: {address}");
            }

            //await DelayAsync(5);

            //    _collectedPlcTasks.Add(new PlcPulseBitTask(address));



            Console.WriteLine($"PulseBitAsync BEG sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
            await _semaphoreSlim.WaitAsync();
            await _plcWriter.WriteAsync(address, true);
            await DelayAsync(100);
            await _plcWriter.WriteAsync(address, false);
            _semaphoreSlim.Release();
            Console.WriteLine($"PulseBitAsync END sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
        }

        public override async Task ToggleBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to toggle a non boolean address: {address}");
            }

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                // await DelayAsync(5);
                // double val = Datablocks[dbIndex].Signals[signalIndex].Value;

                //_collectedPlcTasks.Add(new PlcToggleTask(address, val));

                Console.WriteLine($"ToggleBitAsync BEG sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
                await _semaphoreSlim.WaitAsync(5);
                var readVal = await _plcWriter.ReadAsync(address);
                bool r = (bool)readVal;

                

                await _plcWriter.WriteAsync(address, !r);
          
                _semaphoreSlim.Release();
                Console.WriteLine($"ToggleBitAsync END sec {System.DateTime.Now.Second} ms {System.DateTime.Now.Millisecond}");
            }
            else
            {
                throw new Exception($"Plc Write Error - Unknown error occured while attempting to toggle: {address}");
            }
        }

        private S7.Net.CpuType ConvertCpuType(S7CpuType s7CpuType)
        {
            // Convert PlcComLibrary.Common.S7CpuType (data.S7CpuType) to S7.Net.CpuType to use
            // as input parameter for Plc instance
            bool result = false;
            result = Enum.TryParse(Enum.GetName(typeof(S7CpuType), s7CpuType), out S7.Net.CpuType S7NetCpuType);

            if (!result)
            {
                throw new Exception($"Plc Connect Error - Invalid Cpu Type: {Enum.GetName(typeof(S7CpuType), s7CpuType)}");
            }

            return S7NetCpuType;
        }
    }
}
