using PlcComLibrary.Common;
using PlcComLibrary.Config;
using PlcComLibrary.Models;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Text.RegularExpressions;
using System.Threading.Tasks;
using static PlcComLibrary.Common.Enums;
using log4net;
using System.Threading;
using System.Diagnostics;

namespace PlcComLibrary.PlcCom
{
    public class SimulatedPlcService : PlcService
    {
        private static readonly ILog log = LogManager.GetLogger(System.Reflection.MethodBase.GetCurrentMethod().DeclaringType);
        private static Random _random = new Random();
        private List<SimulatedSignal> _simulatedSignals = new List<SimulatedSignal>();
  

        public SimulatedPlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
            : base(index, config, datablocks)
        {
            foreach (var db in datablocks)
            {
                if (db.Signals.Count > _simulatedSignals.Count)
                {
                    foreach (var signal in db.Signals)
                    {
                        _simulatedSignals.Add(new SimulatedSignal(signal.Index));
                    }
                }  
            }
        }

        public string LastError { get; private set; }

        public override async Task ConnectAsync(ICpuConfig config)
        {
            Config = config;
            await ConnectAsync();
        }

        public override async Task ConnectAsync()
        {
            ComState = ComState.Connecting;
            await DelayAsync(1000);
            ComState = ComState.Connected;
        }

        public override void DisConnect()
        {
            ComState = ComState.DisConnected;
        }

        protected async override void PlcReadWriteCallback(object state)
        {
            Stopwatch watch = new Stopwatch();
            watch.Start();

            if (MonitoredDatablocks.Count > 0)
            {
                for (int i = MonitoredDatablocks.Count - 1; i >= 0; i--)
                {
                    await ReadDbAsync(MonitoredDatablocks[i]);
                }
            }

            _plcReadWriteTimer.Change(Math.Max(0, _interval - watch.ElapsedMilliseconds), Timeout.Infinite);

        }

        public override async Task ReadSingleAsync(string address)
        {
            await DelayAsync(500);
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                PlcReadResultEventArgs args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                RaiseHasNewData(args);
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        protected override async Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();
            List<PlcComIndexValueModel> indexValueModels = new List<PlcComIndexValueModel>();

            for (int i = 0; i < db.Signals.Count; i++)
            {
                ISignalModel s = db.Signals[i];

                double simValue = 0;
                if (s.DataType == DataType.Bit)
                {
                    bool boolVal = _simulatedSignals[s.Index].RandomBool();
                    simValue = boolVal == true ? 1.0 : 0.0;
                }
                else
                {
                    if (i%3 == 0)
                    {
                        simValue = _simulatedSignals[s.Index].Sine();
                    }
                    else
                    {
                        simValue = _simulatedSignals[s.Index].RandomFloat();
                    }
                }
                if (i % 3 == 0)
                {
                    indexValueModels.Add(new PlcComIndexValueModel(Index, db.Index, s.Index, simValue));
                }
            }

            PlcReadResultEventArgs args = new PlcReadResultEventArgs(indexValueModels);
            RaiseHasNewData(args);

            await DelayAsync(100);
        }



        public override async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnected();
            await DelayAsync(10);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                Datablocks[dbIndex].Signals[signalIndex].Value = (double)value;
                Console.WriteLine($"write value: {Datablocks[dbIndex].Signals[signalIndex].Value}");
                log.Debug($"write value: {value}");

                PlcReadResultEventArgs args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), (double)value);
                RaiseHasNewData(args);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean signal: {address}");
            }

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                
                var writeHighArgs = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 1.0f);
                await DelayAsync(100);
                Datablocks[dbIndex].Signals[signalIndex].Value = 1.0f;
                RaiseHasNewData(writeHighArgs);
                await DelayAsync(500);
                //args.Value = 0.0f;

                var writeLowArgs = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                Datablocks[dbIndex].Signals[signalIndex].Value = 0;
                RaiseHasNewData(writeLowArgs);
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }

        public override async Task ToggleBitAsync(string address)
        {
            VerifyConnected();
            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean signal: {address}");
            }

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                await DelayAsync(100);
                if (Datablocks[dbIndex].Signals[signalIndex].Value > 0.0f)
                {
                   
                    var args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 0.0f);
                    Datablocks[dbIndex].Signals[signalIndex].Value = 0.0f;
                    RaiseHasNewData(args);
                }
                else
                {
                    var args = new PlcReadResultEventArgs(new PlcComIndexModel(this.Index, dbIndex, signalIndex), 1.0f);
                    Datablocks[dbIndex].Signals[signalIndex].Value = 1.0f;
                    RaiseHasNewData(args);
                }
            }
            else
            {
                throw new ArgumentOutOfRangeException();
            }
        }
    }
}
