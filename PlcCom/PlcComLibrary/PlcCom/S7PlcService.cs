﻿using PlcComLibrary.Common;
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
        private S7.Net.Plc _plc;

        public S7PlcService(int index, ICpuConfig config, List<IDatablockModel> datablocks)
            : base(index, config, datablocks)
        {
            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);
            _plc = new S7.Net.Plc(S7NetCpuType, config.Ip, (short)config.Rack, (short)config.Slot);
        }

        //public string LastError { get; private set; }


        public override async Task ConnectAsync()
        {
            ComState = ComState.Connecting;

            await DelayAsync(1000);

            S7.Net.CpuType S7NetCpuType = ConvertCpuType(Config.CpuType);

            try
            {
                _plc = new Plc(S7NetCpuType, "100.67.165.113"/*Config.Ip*/, (short)Config.Rack, (short)Config.Slot);
                await _plc.OpenAsync();

                if (_plc.IsConnected)
                {
                    ComState = ComState.Connected;
                }
                else
                {
                    ComState = ComState.ConnectFailed;
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
            if (_plc != null && ComState != ComState.Connected)
            {
                _plc.Close();
                // _plcReadTimer.Stop();
            }
            ComState = ComState.DisConnected;
        }

        public override async Task ReadSingleAsync(string address)
        {
            VerifyConnectedAndValidateAddress(address);

            (int dbIndex, int signalIndex) = GetIndexFromAddress(address, Datablocks);

            if (dbIndex >= 0 && signalIndex >= 0)
            {
                await _plc.ReadAsync(address);
            }
            else
            {
                throw new Exception($"Plc Read Error - Unknown error occured while attempting to read from: {address}");
            }
        }

        protected override async Task ReadDbAsync(IDatablockModel db)
        {
            VerifyConnected();

            await DelayAsync(10);
            byte[] dbBytes = await _plc.ReadBytesAsync(S7.Net.DataType.DataBlock, db.Number, db.FirstByte, db.ByteCount);

            // TODO change Debug.Assert to if and create scheduler to check if it fails several times, 
            // and throw exception + handling remove this db from read list
            //Debug.Assert(dbBytes.Length == db.ByteCount - db.FirstByte);

            List<PlcComIndexValueModel> indexValueModels = new List<PlcComIndexValueModel>();
            for (int i = 0; i < db.Signals.Count; i++)
            {
                ISignalModel s = db.Signals[i];
                int signalByteCount = s.ByteCount();
                byte[] dbBytesRange = dbBytes.Skip(s.DbByteIndex - db.FirstByte).Take(s.ByteCount()).ToArray();
                indexValueModels.Add(new PlcComIndexValueModel(Index, db.Index, s.Index, s.BytesToValue(dbBytesRange)));
            }

            PlcReadResultEventArgs args = new PlcReadResultEventArgs(indexValueModels);

            // foreach (var item in args.IndexValueList)
            // {
            //     Console.WriteLine($"");
            // }

            RaiseHasNewData(args);
        }


        public override async Task WriteSingleAsync(string address, object value)
        {
            VerifyConnectedAndValidateAddress(address);
            await _plc.WriteAsync(address, value);
        }

        public override async Task PulseBitAsync(string address)
        {
            VerifyConnected();

            if (!AddressIsBoolType(address))
            {
                throw new Exception($"Plc Write Error - Attempting to pulse a non boolean address: {address}");
            }

            await _plc.WriteAsync(address, true);
            await DelayAsync(100);
            await _plc.WriteAsync(address, false);
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
                if (Datablocks[dbIndex].Signals[signalIndex].Value == 0)
                {
                    await _plc.WriteAsync(address, true);
                }
                else
                {
                    await _plc.WriteAsync(address, false);
                }
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
